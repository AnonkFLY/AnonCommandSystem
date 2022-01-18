using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;

namespace AnonCommandSystem
{
    public static class CommandUtil
    {
        public static string startColor = "<color=white>";
        public static string currentParameterColor = "<color=red>";
        public static string remainingParameterColor = "<color=grey>";
        public static string optionalParameterColor = "<color=grey>";
        public static string overColor = "</color>";
        /// <summary>
        /// compara <xx:xx> and input
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static bool CompareParameterOnInput(ParameterStruct para)
        {
            switch (para.paraType)
            {
                case ParameterType.Required:
                    return CompareToRequiredParameter(para);
                case ParameterType.Optional:
                    return ComparaToOptionalParameter(para);
                case ParameterType.SyntaxOptions:
                    para.AddCompletion(para.strType);
                    return para.strType == para.strValue;
            }
            DebugLog("Error:Not Found Parameter Type");
            return false;
        }
        public static bool CompareToRequiredParameter(ParameterStruct para)
        {
            var parsedList = CommandParser.parameterParsed.GetInvocationList();

            foreach (Func<ParameterStruct, string, bool> item in parsedList)
            {
                if (item(para, para.strValue))
                {
                    return true;
                }
            }
            if (para.t != null)
            {
                return ReflectionCompareValue(para);
            }
            DebugLog("Error:The parameter type is wrong, only [int|float|string|bool|byte] type, if you want to customize the type, please use [AddCustomParameterParsing] in <CommandParser>");
            return false;
        }
        public static bool ComparaToOptionalParameter(ParameterStruct para)
        {
            var parameter = para.strType;
            var optionalParameter = new List<string>(parameter.Split('|', ':'));
            //if [destroy|replace|moved:mode] on public string mode;
            if (optionalParameter.Count >= 2)
            {
                para.AddCompletion(optionalParameter.ToArray());
                return optionalParameter.Contains(para.strValue);
            }
            else
            {
                return ReflectionCompareValue(para);
            }
        }
        /// <summary>
        /// Parsing like this "<string|entityName>"
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ParameterStruct GetParameterStruct(string type)
        {
            ParameterStruct par = new ParameterStruct();
            switch (type[0])
            {
                case '<':
                    par.paraType = ParameterType.Required;
                    break;
                case '[':
                    par.paraType = ParameterType.Optional;
                    break;
                default:
                    par.paraType = ParameterType.SyntaxOptions;
                    par.strType = type;
                    return par;
            }
            var strs = type.Split(new char[] { ':', '<', '>', '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            par.parameterName = strs[1];
            par.strType = strs[0];
            if (CommandParser.parameterDict.TryGetValue(strs[0], out var getValue))
                par.t = Type.GetType(getValue);
            else
                par.t = Type.GetType(strs[0]);
            if (getValue != null)
                par.strType = getValue;
            return par;
        }
        /// <summary>
        /// get type and input analysis
        /// </summary>
        /// <param name="para"></param>
        /// <param name="input"></param>
        /// <returns>TryValue(bool)</returns>
        public static bool ReflectionCompareValue(ParameterStruct para)
        {
            if (para.t == typeof(string))
                return false;
            var parameters = new object[] { para.strValue, null };
            if (!para.t.ContainsGenericParameters)
                parameters[1] = Activator.CreateInstance(para.t);
            var method = GetStaticTryParse(para.t);
            if (method == null)
            {
                var methods = GetParameterInterface(para.t);
                method = methods[0];
                para.AddCompletion((string[])methods[1].Invoke(parameters[1], new object[] { para.strValue }));
            }
            if (method != null)
            {
                parameters[0] = method.Invoke(parameters[1], parameters);
                para.getValue = parameters[1];
            }
            else
                return false;
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
        public static bool SetValueAble(CommandStruct command, ParameterStruct para)
        {
            switch (para.paraType)
            {
                case ParameterType.SyntaxOptions:
                    return para.strType == para.strValue;
                case ParameterType.Required:
                    return ReflectionSetValue(command, para.parameterName, para.getValue);
                case ParameterType.Optional:
                    if (para.getValue != null) return ReflectionSetValue(command, para.parameterName, para.getValue);
                    else if (!string.IsNullOrEmpty(para.strValue)) return ReflectionSetValue(command, para.parameterName, para.strValue);
                    else return true;
                default: return false;
            }
        }
        public static ReturnCommandData DefaultAnalysis(CommandStruct commandType, string preInput, ExecutionTarget target = null)
        {
            var resultData = new ReturnCommandData();
            var promptBuilder = new StringBuilder();
            var defaultCommad = $"{startColor}{commandType.command} ";
            for (int i = 0; i < commandType.parameters.Length; i++)
            {
                var item = commandType.parameters[i];
                ResetAnalysis(promptBuilder, defaultCommad);
                //获取一种语法
                var parsingData = ParsingCommandOnLine(preInput, item);
                if (parsingData == null)
                {
                    parsingData = new ParsingData();
                    parsingData.indexExecute = -1;
                    parsingData.parsingResult = "No command found with corresponding syntax";
                    continue;
                }
                var currentPara = parsingData.currentPara;
                //UnityEngine.Debug.Log($"read {parsingData.parsIndex}");
                if (parsingData.IsParameterParseSucceeded())
                {
                    if (parsingData.ParaList.Length == parsingData.parsIndex)
                        promptBuilder.Append($"{item} {overColor}");
                    else
                        PromptInterpolatedColor(promptBuilder, parsingData.parsIndex, item, remainingParameterColor);
                    parsingData.indexExecute = i;
                    parsingData.parsingResult = $"{parsingData.parsIndex} arguments successfully parsed";
                    resultData.SetParsingData(parsingData);
                }
                else
                {
                    //参数未全
                    PromptInterpolatedColor(promptBuilder, parsingData.parsIndex, item, currentParameterColor);
                }
                resultData.SetParsingData(parsingData);
                resultData.AddPrompt(promptBuilder.ToString());
                if (currentPara != null && !string.IsNullOrEmpty(currentPara.strValue))
                {
                    //Get the completion
                    //UnityEngine.Debug.Log($"尝试获取{currentPara.strType}补全");
                    if (currentPara.completionList != null)
                        resultData.AddCompletion(currentPara.completionList.ToArray());
                }
            }
            if (resultData.parsingData != null)
                resultData.parsingData.target = target;
            return resultData;
        }
        public static ParsingData DefaultExecute(CommandStruct commandType, ParsingData data)
        {
            if (data.indexExecute != -1)
            {
                for (int i = 0; i < data.ParaList.Length; i++)
                {
                    if (!SetValueAble(commandType, data.ParaList[i]))
                    {
                        data.indexExecute = -1;
                        data.parsingResult = $"No corresponding parameter variable found on {data.ParaList[i].parameterName}";
                        break;
                    }
                }
            }
            return data;
        }
        public static ParsingData ParsingCommandOnLine(string input, string para)
        {
            var paraStrs = para.Split(' ');
            var paraList = GetParameterStructs(paraStrs);
            var inputList = GetInputStruct(input);
            var exceed = paraList.Length < inputList.Length;
            if (exceed || (exceed && inputList[inputList.Length - 1] != " "))
                return null;
            int i;
            string debug = "No parameter parsing";
            ParameterStruct currentPara = null;
            ParsingData resultData = new ParsingData();
            for (i = 0; i < inputList.Length && i < paraList.Length; i++)
            {
                currentPara = paraList[i];
                paraList[i].strValue = inputList[i];
                if (!CompareParameterOnInput(currentPara))
                {
                    debug = $"Parsing {inputList[i]} on {paraStrs[i]} faild";
                    resultData.indexExecute = -1;
                    break;
                }
            }
            resultData.ParaList = paraList;
            resultData.parsingResult = debug;
            resultData.parsIndex = i;
            resultData.currentPara = currentPara;
            //if inputlength < paralength
            if (resultData.indexExecute != -1 && !resultData.IsParameterParseSucceeded())
            {
                resultData.parsingResult = "Missing parameters";
                resultData.indexExecute = -1;
            }

            return resultData;
        }
        public static ParameterStruct[] GetParameterStructs(string[] paraStrs)
        {
            var resultList = new ParameterStruct[paraStrs.Length];
            for (int i = 0; i < paraStrs.Length; i++)
            {
                resultList[i] = GetParameterStruct(paraStrs[i]);
            }
            return resultList;
        }
        public static string[] GetInputStruct(string input)
        {
            string[] result;
            var strs = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int length = strs.Length;
            if (input[input.Length - 1] == ' ')
            {
                result = new string[length];
                result[length - 1] = " ";
            }
            else
                result = new string[length - 1];
            Array.ConstrainedCopy(strs, 1, result, 0, length - 1);
            //UnityEngine.Debug.Log($"Input is [{string.Join(" ", result)}]");
            return result;
        }
        private static void ResetAnalysis(StringBuilder builder, string str)
        {
            builder.Clear();
            builder.Append(str);
        }
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
        public static void PromptInterpolatedColor(StringBuilder t, int index, string source, string currentColor)
        {
            var strs = source.Split(' ');
            for (int i = 0; i < strs.Length; i++)
            {
                if (i == index)
                {
                    t.Append(overColor);
                    t.Append(currentColor);
                    t.Append($"{strs[i]} ");
                    t.Append(overColor);
                    t.Append(remainingParameterColor);
                    continue;
                }
                t.Append($"{strs[i]} ");
            }
            t.Append(overColor);
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
