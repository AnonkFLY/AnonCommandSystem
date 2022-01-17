using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic;

namespace CommandSystem
{
    /// <summary>
    /// Core data class in this system
    /// </summary>
    public class ReturnCommandData
    {
        public ParsingData parsingData;
        /// <summary>
        /// Completion list
        /// </summary>
        public List<string> completion;
        /// <summary>
        /// command prompt list
        /// </summary>
        public HashSet<string> promptList;
        public string[] GetPromptStringArray()
        {
            string[] stringList = new string[promptList.Count];
            promptList.CopyTo(stringList);
            return stringList;
        }
        public ReturnCommandData()
        {
            completion = new List<string>();
            promptList = new HashSet<string>();
        }
        public bool AddCompletion(string com)
        {
            UnityEngine.Debug.Log($"添加{com}");
            if (completion.Contains(com))
                return false;
            completion.Add(com);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="completionList"></param>
        public void AddCompletion(string[] completionList)
        {
            var merge = new string[completion.Count + completionList.Length];
            completion.CopyTo(merge, 0);
            completionList.CopyTo(merge, completion.Count);
            completion = new List<string>(merge);
        }
        public void SetCompletion(string[] completionList)
        {
            completion = new List<string>(completionList);
        }
        public void ClearCompletion()
        {
            completion.Clear();
        }
        public bool AddPrompt(string com)
        {
            return promptList.Add(com);
        }
        public void SetParsingData(ParsingData data)
        {
            if (parsingData == null || !parsingData.IsParameterResult())
                parsingData = data;
        }
    }
    public class ParameterStruct
    {
        public string parameterName;
        public string strType;
        public string strValue;
        public Type t;
        public object getValue;
        public ParameterType paraType;
    }
    public class ExecutionTarget
    {
        public float[] position = new float[3];
        public string name;
    }
    public enum ParameterType
    {
        Required,
        Optional,
        SyntaxOptions
    }
    public class ParsingData
    {
        public int indexExecute;
        public ExecutionTarget target;
        public string parsingResult;
        public int parsIndex;
        public ParameterStruct[] paraList;
        public ParameterStruct currentPara;
        public bool IsParameterResult()
        {
            return paraList.Length == parsIndex;
        }
    }
}