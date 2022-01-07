using System.Security.Principal;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace CommandSystem
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
        public CommandParser()
        {
            //init
            commandList = new HashSet<CommandStruct>();
            parameterDict = new Dictionary<string, string>();
            parameterParsed = new Func<ParameterStruct, string, bool>(StringParsing);
            //parameter dictionary
            parameterDict.Add("float", "System.Single");
            parameterDict.Add("bool", "System.Boolean");
            parameterDict.Add("int", " System.Int32");
            parameterDict.Add("string", "System.String");
            //command add
            AddCommand(new TeleportCommand());
            AddCommand(new KillCommand());
        }
        public bool AddCommand(CommandStruct command)
        {
            return commandList.Add(command); ;
        }
        public ReturnCommandData GetCompletion(string preInput)
        {
            currentCommand = null;
            var completionList = new ReturnCommandData();
            completionList.preInput = preInput;
            //get return data
            foreach (var item in commandList)
            {
                var comm = GetCommandCompare(preInput, item.command);
                if (comm == item.command)
                {
                    //Analysis of command parameters
                    currentCommand = item;
                    return item.CompareToInput(completionList);
                }
                else if (comm != null)
                {
                    completionList.completion.Add(item.command);
                }
            }
            return completionList;
        }
        public string ExecuteCommand(string preInput)
        {
            if (currentCommand != null)
            {
                //CommandUtil.DefaultExecute<typeof(currentCommand)>()
                return currentCommand.ExecuteParsing(preInput);
            }
            else
                return "未找到命令" + preInput;
        }
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
        /// <summary>
        /// Compare a line command
        /// </summary>
        /// <param name="preInput"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private string GetCommandCompare(string preInput, string command)
        {
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
        /// String parsing
        /// </summary>
        /// <param name="para"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool StringParsing(ParameterStruct para, string input)
        {
            para.getValue = input;
            if (para.t == typeof(string))
                return !string.IsNullOrEmpty(input);
            return false;
        }
    }


}