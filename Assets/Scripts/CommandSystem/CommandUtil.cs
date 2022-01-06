using System.Linq;
using System;
using System.Reflection;
using System.Text;

namespace CommandSystem
{
    public static class CommandUtil
    {
        public static bool GetTryValueType<T>(T command, string input, string parameter, bool isExe = false) where T : CommandStruct
        {
            if (string.IsNullOrEmpty(input))
                return false;
            if (!(parameter[0] == '<' || parameter[0] == '['))
                return input == parameter;
            var para = GetParameterStruct(parameter);
            if (para.t != null)
            {
                var result = ReflectionValue(para, input);
                var able = (bool)result[1];
                if (able && isExe)
                {
                    command.GetType().GetField(para.parameterName).SetValue(command, result[0]);
                }
                return able;
            }
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
            par.t = Type.GetType(CommandParser.parameterDict[str[1]]);
            return par;
        }
        /// <summary>
        /// get type and input analysis
        /// </summary>
        /// <param name="para"></param>
        /// <param name="input"></param>
        /// <returns>Value and TryValue(bool)</returns>
        public static object[] ReflectionValue(ParameterStruct para, string input)
        {
            var result = new object[2];
            if (para.t == typeof(string))
            {
                result[0] = input;
                result[1] = !string.IsNullOrEmpty(input);
                return result;
            }
            var parameters = new object[] { input, Activator.CreateInstance(para.t) };
            var method = para.t.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder
             , new Type[] { typeof(string), para.t.MakeByRefType() }
             , new ParameterModifier[] { new ParameterModifier(2) });
            if (method != null)
            {
                result[1] = method.Invoke(null, parameters);
            }
            result[0] = parameters[1];
            return result;
        }

        public static ReturnCommandData DefaultAnalysis<T>(T commandType, string preInput) where T : CommandStruct
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
                    if (inputStrs.Length > i && paraStrs.Length > i && GetTryValueType<T>(commandType, inputStrs[i], paraStrs[i]))
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
        public static ExecuteData DefaultExecute<T>(T commandType, string preInput) where T : CommandStruct
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
                    if (inputStrs.Length > i && paraStrs.Length > i && GetTryValueType<T>(commandType, inputStrs[i], paraStrs[i], true))
                    {
                        DebugLog($"Parsed {inputStrs[i]} on {paraStrs[i]} successfully");
                    }
                    else
                    {
                        var debug = $"Parsing {inputStrs[i]} on {paraStrs[i]} failed";
                        backData.resultStr = debug;
                        backData.indexExecute = item;
                        break;
                    }
                }
                if (i >= paraStrs.Length)
                {
                    backData.resultStr = "解析成功";
                    backData.indexExecute = item;
                    return backData;
                }
            }
            return backData;
        }
        public static void DebugLog(object obj)
        {
            UnityEngine.Debug.Log(obj);
        }
    }
}
