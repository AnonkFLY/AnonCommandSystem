namespace CommandSystem
{
    /// <summary>
    /// Abstract base class for all commands
    /// </summary>
    public abstract class CommandStruct
    {
        /// <summary>
        /// command name,Used for prefix parsing
        /// </summary>
        public string command;
        /// <summary>
        /// describe for this command
        /// </summary>
        public string expound;
        /// <summary>
        /// Parameter list
        /// </summary>
        public string[] parameters;
        /// <summary>
        ///  analyze and execute
        /// </summary>
        /// <param name="inputCommand"></param>
        /// <returns>result</returns>
        public virtual string ExecuteParsing(string inputCommand)
        {
            return Execute(CommandUtil.DefaultExecute(this, inputCommand));
        }
        public abstract string Execute(ExecuteData data);

        /// <summary>
        /// pre input compared
        /// </summary>
        /// <param name="preInput">input string</param>
        /// <returns>string after color processing</returns>
        public virtual ReturnCommandData CompareToInput(ReturnCommandData resultData)
        {
            return CommandUtil.DefaultAnalysis(this, resultData.preInput);
        }

    }

}