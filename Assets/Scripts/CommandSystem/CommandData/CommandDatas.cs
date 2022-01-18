using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic;

namespace AnonCommandSystem
{
    /// <summary>
    /// Core data class in this system
    /// </summary>
    public class ReturnCommandData
    {
        /// <summary>
        /// parsed command result
        /// </summary>
        public ParsingData parsingData;
        /// <summary>
        /// Completion list
        /// </summary>
        public List<string> completionList;
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
            completionList = new List<string>();
            promptList = new HashSet<string>();
        }
        public void AddCompletion(string[] completion)
        {
            var merge = new string[completionList.Count + completion.Length];
            completionList.CopyTo(merge, 0);
            completion.CopyTo(merge, completionList.Count);
            completionList = new List<string>(merge);
        }
        public bool AddPrompt(string com)
        {
            return promptList.Add(com);
        }
        public void SetParsingData(ParsingData data)
        {
            if (parsingData == null || !parsingData.IsParameterParseSucceeded())
            {
                parsingData = data;
            }
        }
    }
    public class ParameterStruct
    {
        public List<string> completionList = new List<string>();
        public string parameterName;
        public string strType;
        public string strValue;
        public Type t;
        public object getValue;
        public ParameterType paraType;
        public bool AddCompletion(string com)
        {
            if (completionList.Contains(com))
                return false;
            completionList.Add(com);
            return true;
        }
        public void AddCompletion(string[] completion)
        {
            var merge = new string[completionList.Count + completion.Length];
            completionList.CopyTo(merge, 0);
            completion.CopyTo(merge, completionList.Count);
            completionList = new List<string>(merge);
        }
        public void ClearCompletion()
        {
            completionList.Clear();
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
    public class ParsingData
    {
        public int indexExecute;
        public ExecutionTarget target;
        public string parsingResult;
        public int parsIndex;
        public ParameterStruct[] ParaList
        {
            get
            {
                return paraList;
            }
            set
            {
                paraList = value;
                for (atLeastParal = paraList.Length - 1; atLeastParal > 0; atLeastParal--)
                {
                    if (paraList[atLeastParal].paraType != ParameterType.Optional)
                        break;
                }
            }
        }
        public ParameterStruct currentPara;
        private ParameterStruct[] paraList;
        private int atLeastParal;
        public bool IsParameterParseSucceeded()
        {
            return atLeastParal < parsIndex && parsIndex <= paraList.Length;
        }
    }
}