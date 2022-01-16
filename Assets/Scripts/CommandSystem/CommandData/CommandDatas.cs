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
            if (completion.Contains(com))
                return false;
            completion.Add(com);
            return true;
        }
        /// <summary>
        /// If AddCompletion is required, this function should be executed before AddCompletion
        /// </summary>
        /// <param name="completionList"></param>
        public void SetCompletion(string[] completionList)
        {
            completion = new List<string>(completionList);
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