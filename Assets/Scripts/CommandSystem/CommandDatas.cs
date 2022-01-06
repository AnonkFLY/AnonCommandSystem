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
        public HashSet<string> completion;
        /// <summary>
        /// command prompt list
        /// </summary>
        public CommandPrompt prompt;
        public ReturnCommandData()
        {
            completion = new HashSet<string>();
            prompt = new CommandPrompt();
        }
        public bool AddCompletion(string com)
        {
            return completion.Add(com);
        }
        public bool AddPrompt(string com)
        {
            return prompt.promptList.Add(com);
        }
    }
    public class ParameterStruct
    {
        public Type t;
        public string parameterName;
    }
    public class CommandPrompt
    {
        public HashSet<string> promptList = new HashSet<string>();
        public int colorIndex;
        public int currentIndex;
    }
    public class ExecuteData
    {
        public int indexExecute;
        public string resultStr;
    }
}