using System.Collections.Generic;
using System;

namespace AnonCommandSystem
{

    public class CommandParser
    {
        /// <summary>
        /// Resolve custom types of delegates
        /// </summary>
        public static Func<ParameterStruct, string, bool> parameterParsed;
        /// <summary>
        /// Used for reflection type conversion
        /// </summary>
        public static Dictionary<string, string> parameterDict;

        private CommandStruct currentCommand;
        private HashSet<CommandStruct> commandList;
        private string preInput = " ";
        public CommandParser()
        {
            //init
            commandList = new HashSet<CommandStruct>();
            parameterDict = new Dictionary<string, string>();
            parameterParsed = new Func<ParameterStruct, string, bool>(StringParsing);
            //parameter dictionary
            InitDefaultParameter();
            //command add
            // AddCommand(new TeleportCommand());
            // AddCommand(new KillCommand());
            //Parameter type add
            AddCustomParameterParsing<SelectorParameter>("Selector");
        }
        public bool RegisterCommand(CommandStruct command)
        {
            return commandList.Add(command);
        }
        public ReturnCommandData ParseCommand(string preInput, ExecutionTarget target = null)
        {
            this.preInput = preInput;
            currentCommand = null;
            var completionList = new ReturnCommandData();
            //get return data
            foreach (var item in commandList)
            {
                var comm = ParsingCommand(preInput, item.command);
                if (comm == item.command)
                {
                    //Analysis of command parameters
                    currentCommand = item;
                    return item.CompareToInput(preInput, target);
                }
                else if (comm != null)
                {
                    completionList.completionList.Add(item.command);
                }
            }
            return completionList;
        }
        public string ExecuteCommand(string input = null, ExecutionTarget target = null)
        {
            if (!string.IsNullOrEmpty(input))
                ParseCommand(input, target);
            if (currentCommand != null)
            {
                return currentCommand.ExecuteParsing();
            }
            else
                return "No Found " + preInput.Split(' ')[0] + " Command";
        }
        /// <summary>
        /// Add custom type names,no completion
        /// </summary>
        /// <param name="action">Parsing Func</param>
        public void AddCustomParameterParsing(Func<ParameterStruct, string, bool> action)
        {
            parameterParsed += action;
        }
        public bool AddCustomParameterParsing<T>(string keyParameter) where T : ICommandParameter<T>, new()
        {
            T t = new T();
            parameterDict.Add(keyParameter, t.ToString());
            return true;
        }
        private void InitDefaultParameter()
        {
            parameterDict.Add("string", typeof(string).ToString());
            parameterDict.Add("int", typeof(int).ToString());
            parameterDict.Add("float", typeof(float).ToString());
            parameterDict.Add("byte", typeof(byte).ToString());
            parameterDict.Add("bool", typeof(bool).ToString());
        }
        /// <summary>
        /// Compare a line command
        /// </summary>
        /// <param name="preInput"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private string ParsingCommand(string preInput, string command)
        {
            preInput = preInput.Split(' ')[0];
            if (preInput.Length > command.Length)
                return null;
            if (preInput == command)
                return command;
            //输入为空。输入一半命令，输入全命令，，， 输入命令，在其他命令是半命令
            if (string.IsNullOrEmpty(preInput))
                return null;
            int length = Math.Min(preInput.Length, command.Length);
            for (int i = 0; i < length; i++)
            {
                if (preInput[i] != command[i])
                    return null;
            }
            return preInput.Substring(0, length);
        }
        /// <summary>
        /// String parsing on Custom Parsing
        /// </summary>
        /// <param name="para"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool StringParsing(ParameterStruct para, string input)
        {
            para.getValue = input;
            if (para.t == typeof(string) && input != "")
                return true;
            return false;
        }
    }


}