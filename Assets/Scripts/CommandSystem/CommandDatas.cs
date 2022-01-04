using System;
using System.Collections.Generic;

namespace CommandSystem
{
    public class ParameterStruct
    {
        public Type t;
        public string parameterName;
    }
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
        public CommandPrompt prompt;
        public ReturnCommandData()
        {
            completion = new List<string>();
            prompt = new CommandPrompt();
        }
        public bool AddCompletion(string com)
        {
            if (completion.Contains(com))
                return false;
            completion.Add(com);
            return true;
        }
        public bool AddPrompt(string com)
        {
            if (prompt.promptList.Contains(com))
                return false;
            prompt.promptList.Add(com);
            return true;
        }
    }
    public class CommandPrompt
    {
        public List<string> promptList = new List<string>();
        public int colorIndex;
        public int currentIndex;
    }
}