using System.Linq;
using System;
using System.Reflection;
using System.Text;

namespace CommandSystem
{
    public static class CommandUtil
    {
        public static bool GetValueType<T>(T command, string input, string parameter) where T : CommandStruct
        {
            if (string.IsNullOrEmpty(input))
                return false;
            if (!(parameter[0] == '<' || parameter[0] == '['))
                return input == parameter;
            // UnityEngine.Debug.Log("ExeDown");
            var para = GetParameterStruct(parameter);
            //获取类型，是否可以转，可以就转并反射赋值
            if (para.t != null)
            {
                return (bool)ReflectionValue(para, input)[1];
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
            //            UnityEngine.Debug.Log(str[1]);
            par.t = Type.GetType(CommandParser.parameterDict[str[1]]);
            return par;
        }
        //get type and input analysis
        public static object[] ReflectionValue(ParameterStruct para, string input)
        {
            var result = new object[2];
            if (para.t == typeof(string))
            {
                result[0] = input;
                result[1] = true;
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
        public static ReturnCommandData DefaultAnalysis<T>(T commandType, ReturnCommandData resultData) where T : CommandStruct
        {
            var result = new StringBuilder($"<color=white>{commandType.command} ");
            var inputStrs = ($"{resultData.preInput}").Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            foreach (var item in commandType.parameter)
            {
                # region Append a Line
                var paraStrs = ($"{commandType.command} {item}").Split(' ');
                if (paraStrs.Length < inputStrs.Length - 1)
                    continue;
                int i = 1;
                for (; i < paraStrs.Length; i++)
                {
                    if (inputStrs.Length > i && paraStrs.Length > i && GetValueType<T>(commandType, inputStrs[i], paraStrs[i]))
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
    }
}