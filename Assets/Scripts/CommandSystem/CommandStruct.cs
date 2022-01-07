namespace CommandSystem
{
    //命令结构。将字符串传入后解析结果，并将参数返回
    //每一个对象代表一个命令对象
    public abstract class CommandStruct
    {
        public string command;
        public string expound;
        public string[] parameter;
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