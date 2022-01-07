using System.Linq;
using System;
using System.Reflection;
using System.Text;

namespace CommandSystem
{
    public static class CommandUtil
    {

        public static bool GetTryValueType(CommandStruct command, string input, string parameter, bool isExe = false)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            if (!(parameter[0] == '<' || parameter[0] == '['))
                return input == parameter;
            var para = GetParameterStruct(parameter);
            var parsedList = CommandParser.parameterParsed.GetInvocationList();
            foreach (Func<ParameterStruct, string, bool> item in parsedList)
            {
                if (item(para, input))
                {
                    if (isExe)
                        ReflectionSetValue(command, para);
                    return true;
                }
            }
            if (para.t != null)
            {
                var able = ReflectionTryValue(para, input);
                if (able && isExe)
                {
                    ReflectionSetValue(command, para);
                }
                return able;
            }
            DebugLog("Error:The parameter type is wrong, only [int|float|string] type, if you want to customize the type, please use [AddCustomParameterParsing] in <CommandParser>");
            return false;
        }

        /// <summary>
        /// like this "<string|entityName>"
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ParameterStruct GetParameterStruct(string type)
        {
            ParameterStruct par = new ParameterStruct();
            var str = type.Split(':', '<', '>', '[', ']');
            //UnityEngine.Debug.Log(str[1]);
            par.parameterName = str[2];
            if (CommandParser.parameterDict.TryGetValue(str[1], out var getValue))
                par.t = Type.GetType(getValue);
            if (getValue != null)
                par.tString = getValue;
            else
                par.tString = str[1];

            return par;
        }
        /// <summary>
        /// get type and input analysis
        /// </summary>
        /// <param name="para"></param>
        /// <param name="input"></param>
        /// <returns>TryValue(bool)</returns>
        public static bool ReflectionTryValue(ParameterStruct para, string input)
        {
            var parameters = new object[] { input, Activator.CreateInstance(para.t) };
            var method = para.t.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder
             , new Type[] { typeof(string), para.t.MakeByRefType() }
             , new ParameterModifier[] { new ParameterModifier(2) });
            if (method == null)
            {
                method = para.t.GetMethod("TryParse",
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    CallingConventions.Any,
                    new Type[] { typeof(string), typeof(object).MakeByRefType() },
                    null);
            }
            if (method != null)
            {
                DebugLog($"{parameters[0]} and {parameters[1]}");
                parameters[0] = method.Invoke(para.t.Assembly.CreateInstance(para.tString), parameters);
                DebugLog($"{parameters[0]} and {parameters[1]}");
                para.getValue = parameters[1];
            }
            else
            {
                return false;
            }
            return (bool)parameters[0];
        }
        public static void ReflectionSetValue(CommandStruct command, ParameterStruct para)
        {
            try
            {
                command.GetType().GetField(para.parameterName).SetValue(command, para.getValue);
            }
            catch
            {
                DebugLog($"Error:{command.ToString()}:Type of conversion '{para.tString}',Value '{para.getValue}'");
            }
        }
        public static ReturnCommandData DefaultAnalysis(CommandStruct commandType, string preInput)
        {
            var resultData = new ReturnCommandData();
            resultData.preInput = preInput;
            var result = new StringBuilder($"<color=white>{commandType.command} ");
            var inputStrs = preInput.Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            foreach (var item in commandType.parameter)
            {
                # region Append a Line
                var paraStrs = ($"{commandType.command} {item}").Split(' ');
                if (paraStrs.Length < inputStrs.Length - 1)
                    continue;
                int i = 1;
                for (; i < paraStrs.Length; i++)
                {
                    if (inputStrs.Length > i && paraStrs.Length > i && GetTryValueType(commandType, inputStrs[i], paraStrs[i]))
                        result.Append($" {paraStrs[i]}");
                    else
                    {
                        result.Append("</color><color=red>");
                        result.Append($" {paraStrs[i]}");
                        i++;
                        break;
                    }
                }
                if (inputStrs.Length > i)
                {
                    result.Clear();
                    result.Append($"<color=white>{commandType.command} ");
                    continue;
                }
                result.Append("</color>");
                for (; i < paraStrs.Length; i++)
                {
                    result.Append($" {paraStrs[i]}");
                }
                # endregion
                resultData.AddCompletion(result.ToString());
                result.Clear();
                result.Append($"<color=white>{commandType.command} ");
            }
            return resultData;
        }
        public static ExecuteData DefaultExecute(CommandStruct commandType, string preInput)
        {
            var backData = new ExecuteData();
            var inputStrs = preInput.Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            var length = commandType.parameter.Length;
            for (int item = 0; item < length; item++)
            {
                var paraStrs = ($"{commandType.command} {commandType.parameter[item]}").Split(' ');
                if (paraStrs.Length < inputStrs.Length - 1)
                    continue;
                int i = 1;
                for (; i < paraStrs.Length; i++)
                {
                    if (inputStrs.Length > i && paraStrs.Length > i && GetTryValueType(commandType, inputStrs[i], paraStrs[i], true))
                    {
                        DebugLog($"Parsed {inputStrs[i]} on {paraStrs[i]} successfully");
                    }
                    else
                    {
                        string debug;
                        if (inputStrs.Length > i && paraStrs.Length > i)
                        {
                            debug = $"Parsing {inputStrs[i]} on {paraStrs[i]} failed";
                        }
                        else
                        {
                            debug = "Missing parameters";
                        }
                        backData.resultStr = debug;
                        backData.indexExecute = item;
                        i++;
                        break;
                    }
                }
                if (i >= paraStrs.Length)
                {
                    backData.resultStr = $"Parsing {i} successfully";
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
        //     //TODO:优化上面两个像坨屎的解析函数
        // }
        public static void DebugLog(object obj)
        {
            UnityEngine.Debug.Log(obj);
        }
    }
}
