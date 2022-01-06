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
        public abstract string Execute(string inputCommand);

        /// <summary>
        /// pre input compared
        /// </summary>
        /// <param name="preInput">input string</param>
        /// <returns>string after color processing</returns>
        public abstract ReturnCommandData CompareToInput(ReturnCommandData resultData);

    }

}