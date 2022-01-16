using System;
using System.Collections.Generic;

namespace CommandSystem
{
    /// <summary>
    /// Core data class in this system
    /// </summary>
    public class ReturnCommandData
    {
        /// <summary>
        /// Pre-Input
        /// </summary>
        public string preInput;
        public string current;
        /// <summary>
        /// Completion list
        /// </summary>
        public List<string> completion;
        /// <summary>
        /// command prompt list
        /// </summary>
        public HashSet<string> promptList;
        public string[] GetStringArray()
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
            //            UnityEngine.Debug.Log($"添加{completionList[0]}..");
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
    }
    public class ParameterStruct
    {
        public Type t;
        public string parameterName;
        public object getValue;
        public string tType;
        public string tValue;
        public ParameterType type = ParameterType.Required;
    }
    public class ExecuteData
    {
        /// <summary>
        /// Executed syntax index，execution failed as -1
        /// </summary>
        public int indexExecute;
        /// <summary>
        /// Parsing result
        /// </summary>
        public string resultStr;
        public ExecuteData SetValue(int index, string result)
        {
            this.indexExecute = index;
            this.resultStr = result;
            return this;
        }
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
}