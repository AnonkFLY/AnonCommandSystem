using System.Collections.Generic;
using System;
using System.Reflection;
using System.Text;

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
            if (commandType.parameters == null)
                return null;
            var resultData = new ReturnCommandData();
            var promptBuilder = new StringBuilder();
            var defaultCommad = $"{startColor}{commandType.command} ";
            preInput = preInput.Replace(commandType.command, "");
            var inputList = GetInputStruct(preInput);
            for (int i = 0; i < commandType.parameters.Length; i++)
            {
                var item = commandType.parameters[i];
                ResetAnalysis(promptBuilder, defaultCommad);
                //获取一种语法
                var parsingData = ParsingCommandOnLine(inputList, item);
                if (parsingData == null)
                {
                    parsingData = new ParsingData();
                    parsingData.indexExecute = -1;
                    parsingData.parsingResult = "No command found with corresponding syntax";
                    continue;
                }
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
                    if (!parsingData.canPrompt)
                        continue;
                    //参数未全
                    PromptInterpolatedColor(promptBuilder, parsingData.parsIndex, item, currentParameterColor);
                }
                var currentPara = parsingData.currentPara;
                if (currentPara != null)
                {
                    //Get the completion
                    //UnityEngine.Debug.Log($"尝试获取{currentPara.strType}补全");
                    if (currentPara.completionList != null)
                        resultData.AddCompletion(currentPara.completionList.ToArray());
                }
                resultData.SetParsingData(parsingData);
                resultData.AddPrompt(promptBuilder.ToString());
            }
            if (resultData.parsingData != null)
                resultData.parsingData.target = target;
            return resultData;
        }

        public static ParsingData DefaultExecute(CommandStruct commandType, ParsingData data)
        {
            if (data == null)
                return null;
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
        public static ParsingData ParsingCommandOnLine(string[] inputList, string para)
        {
            var paraStrs = para.Split(' ');
            var paraList = GetParameterStructs(paraStrs);
            if (paraList.Length < inputList.Length)
                return null;
            int i;
            string debug = "No parameter parsing";
            ParameterStruct currentPara = null;
            ParsingData resultData = new ParsingData();
            //UnityEngine.Debug.Log($"CurrentParament is [{paraList[0].parameterName}]");
            for (i = 0; i < inputList.Length && i < paraList.Length; i++)
            {
                //UnityEngine.Debug.Log($"Input is [{inputList[i]}]");
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
            resultData.canPrompt = !(inputList.Length - 1 > i);//inputList.Length == i;//
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
            List<string> result;
            //input = input.Replace("~", " ~").Replace("^", " ^");
            input = GetStringValueOnInput(input);
            var strs = input.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            result = new List<string>(strs);
            if (!string.IsNullOrEmpty(input) && input[input.Length - 1] == ' ')
                result.Add(" ");
            return result.ToArray();
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
        public static string GetStringValueOnInput(string str)
        {
            int count = 0;
            int startIndex = 0;
            int i = 0;
            int j = 0;
            while (true)
            {
                count++;
                if (count > 100)
                    break;
                i = str.IndexOf("\"", startIndex);
                if (i == -1)
                    break;
                var a = str.Substring(0, i);
                j = str.IndexOf("\"", i + 1);
                if (j == -1)
                    break;
                var b = str.Substring(i, j).Replace(" ", "&nbsp");
                var c = str.Substring(j + 1);
                str = a + b + c;
            }
            return str;
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
