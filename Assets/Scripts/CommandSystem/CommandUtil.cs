using System.Linq;
using System;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;

namespace CommandSystem
{
    public static class CommandUtil
    {
        public static string startColor = "<color=white>";
        public static string currentParameterColor = "<color=red>";
        public static string remainingParameterColor = "<color=grey>";
        public static string optionalParameterColor = "<color=grey>";
        public static string overColor = "</color>";
        public static bool BaseParameterParsing(string input, string parameter, out ParameterStruct para)
        {
            para = GetParameterStruct(parameter, input);
            if (string.IsNullOrEmpty(input))
                return false;
            return true;
        }
        public static bool GetTryValueType<T>(T obj, ParameterStruct para, ReturnCommandData returnData = null, bool isSetValue = false)
        {
            if (para.type == ParameterType.SyntaxOptions)
            {
                return para.tType == para.tValue;
            }
            var parsedList = CommandParser.parameterParsed.GetInvocationList();
            foreach (Func<ParameterStruct, string, bool> item in parsedList)
            {
                if (item(para, para.tValue))
                {
                    if (isSetValue)
                        return ReflectionSetValue(obj, para.parameterName, para.getValue);
                    return true;
                }
            }
            if (para.t != null)
            {
                var able = ReflectionTryValue(para, para.tValue, returnData);
                if (able && isSetValue)
                {
                    return ReflectionSetValue(obj, para.parameterName, para.getValue);
                }
                return able;
            }
            DebugLog("Error:The parameter type is wrong, only [int|float|string|bool|byte] type, if you want to customize the type, please use [AddCustomParameterParsing] in <CommandParser>");
            return false;
        }
        /// <summary>
        /// Parsing like this "<string|entityName>"
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ParameterStruct GetParameterStruct(string type, string input)
        {
            ParameterStruct par = new ParameterStruct();
            par.tValue = input;
            par.tType = type;
            switch (type[0])
            {
                case '<':
                    par.type = ParameterType.Required;
                    break;
                case '[':
                    par.type = ParameterType.Optional;
                    break;
                default:
                    par.type = ParameterType.SyntaxOptions;
                    return par;
            }
            var str = type.Split(':', '<', '>', '[', ']').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            par.parameterName = str[1];
            par.tType = str[0];
            if (CommandParser.parameterDict.TryGetValue(str[0], out var getValue))
                par.t = Type.GetType(getValue);
            else
                par.t = Type.GetType(str[0]);
            if (getValue != null)
                par.tType = getValue;
            return par;
        }
        /// <summary>
        /// get type and input analysis
        /// </summary>
        /// <param name="para"></param>
        /// <param name="input"></param>
        /// <returns>TryValue(bool)</returns>
        public static bool ReflectionTryValue(ParameterStruct para, string input, ReturnCommandData data = null)
        {
            //[bool,value]
            var parameters = new object[] { input, null };
            if (!para.t.ContainsGenericParameters)
                parameters[1] = Activator.CreateInstance(para.t);
            var method = GetStaticTryParse(para.t);
            if (method == null)
            {
                var methods = GetParameterInterface(para.t);
                method = methods[0];
                if (data != null)
                {
                    data.SetCompletion((string[])methods[1].Invoke(parameters[1], new object[] { input }));
                }
            }
            if (method != null)
            {
                parameters[0] = method.Invoke(parameters[1], parameters);
                para.getValue = parameters[1];
            }
            else
            {
                return false;
            }
            return (bool)parameters[0];
        }
        public static bool ReflectionSetValue<T>(T obj, string parameterName, object value)
        {
            try
            {
                obj.GetType().GetField(parameterName).SetValue(obj, value);
                return true;
            }
            catch (System.Exception e)
            {
                DebugLog($"Error:{e}");
                return false;
            }
        }
        public static ReturnCommandData DefaultAnalysis(CommandStruct commandType, string preInput)
        {
            var resultData = new ReturnCommandData();
            resultData.preInput = preInput;
            var result = new StringBuilder($"{startColor}{commandType.command} ");
            var inputStrs = preInput.Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            foreach (var item in commandType.parameters)
            {
                # region Append a Line
                var paraStrs = ($"{commandType.command} {item}").Split(' ');
                if (paraStrs.Length < inputStrs.Length)
                    continue;
                int i = 1;
                for (; i < paraStrs.Length; i++)
                {
                    bool isLength = inputStrs.Length > i && paraStrs.Length > i;
                    if (isLength && BaseParameterParsing(inputStrs[i], paraStrs[i], out var para) && GetTryValueType(commandType, para, resultData))
                    {
                        result.Append($" {paraStrs[i]}");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(startColor))
                            result.Append(overColor);
                        result.Append(currentParameterColor);
                        result.Append($" {paraStrs[i]}");
                        i++;
                        break;
                    }
                }
                if (inputStrs.Length > i)
                {
                    result.Clear();
                    result.Append($"{startColor}{commandType.command} ");
                    continue;
                }
                result.Append(overColor);
                result.Append(remainingParameterColor);
                for (; i < paraStrs.Length; i++)
                {
                    result.Append($" {paraStrs[i]}");
                }
                result.Append(overColor);
                # endregion
                resultData.AddPrompt(result.ToString());
                result.Clear();
                result.Append($"{startColor}{commandType.command} ");
            }
            return resultData;
        }
        public static ExecuteData DefaultExecute(CommandStruct commandType, string preInput, ExecutionTarget target = null)
        {
            var backData = new ExecuteData();
            var inputStrs = preInput.Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            var length = commandType.parameters.Length;
            for (int item = 0; item < length; item++)
            {
                var paraStrs = ($"{commandType.command} {commandType.parameters[item]}").Split(' ');
                if (paraStrs.Length < inputStrs.Length)
                {
                    if (item == length - 1)
                    {
                        backData.resultStr = "No command found with corresponding syntax";
                        backData.indexExecute = -1;
                    }
                    continue;
                }
                int i = 1;
                //DebugLog($"Parsing {item} command parameter");
                for (; i < paraStrs.Length; i++)
                {
                    bool isLength = inputStrs.Length > i && paraStrs.Length > i;
                    if (isLength && BaseParameterParsing(inputStrs[i], paraStrs[i], out var para) && GetTryValueType(commandType, para, isSetValue: true))
                    {
                        //DebugLog($"Parsed {inputStrs[i]} on {paraStrs[i]} successfully in {i}");
                    }
                    else
                    {
                        //DebugLog("Parsing failed");
                        string debug;
                        if (inputStrs.Length > i && paraStrs.Length > i)
                        {
                            debug = $"Parsing {inputStrs[i]} on {paraStrs[i]} failed";
                        }
                        else
                        {
                            //if()
                            debug = "Missing parameters";
                        }
                        backData.resultStr = debug;
                        backData.indexExecute = -1;
                        //i++;
                        break;
                    }
                }
                if (i >= paraStrs.Length)
                {
                    //DebugLog("Parsed!");
                    backData.resultStr = $"Successfully parsed {i} parameters";
                    backData.indexExecute = item;
                    return backData;
                }
            }
            return backData;
        }
        // public static int ParsingCommandOnLine(CommandStruct commandType, string[] input, string[] para)
        // {
        //     int i = 1;
        //     for (; i < para.Length; i++)
        //     {
        //         if (input.Length > i && para.Length > i && GetTryValueType(commandType, input[i], para[i], true))
        //         {
        //             DebugLog($"Parsed {input[i]} on {para[i]} successfully");
        //         }
        //         else
        //         {
        //             DebugLog($"Parsing {input[i]} on {para[i]} failed");
        //             i++;
        //         }
        //     }
        //     return i;
        //     //TODO:优化上面两个像坨屎的解析函数失败，有生之年
        // }
        private static MethodInfo GetStaticTryParse(Type type)
        {
            return type.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder
             , new Type[] { typeof(string), type.MakeByRefType() }
             , new ParameterModifier[] { new ParameterModifier(2) });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns> TryParse and GetParameterCompletion</returns>
        private static MethodInfo[] GetParameterInterface(Type type)
        {
            var parameterList = new MethodInfo[2];
            parameterList[0] = type.GetMethod("TryParse",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                CallingConventions.Any,
                new Type[] { typeof(string), typeof(object).MakeByRefType() },
                null);
            parameterList[1] = type.GetMethod("GetParameteCompletion",
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    CallingConventions.Any,
                    new Type[] { typeof(string) },
                    null);
            return parameterList;
        }
        /// <summary>
        /// Change it to suit you
        /// </summary>
        /// <param name="obj"></param>
        public static void DebugLog(object obj)
        {
            UnityEngine.Debug.Log(obj);
        }
    }
}
