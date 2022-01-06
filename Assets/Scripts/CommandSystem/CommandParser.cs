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
        public CommandStruct currentCommand;
        public HashSet<CommandStruct> commandList;
        public static Dictionary<string, string> parameterDict;
        public CommandParser()
        {
            //init
            commandList = new HashSet<CommandStruct>();
            parameterDict = new Dictionary<string, string>();
            //parameter dictionary
            parameterDict.Add("float", "System.Single");
            parameterDict.Add("bool", "System.Boolean");
            parameterDict.Add("int", " System.Int32");
            parameterDict.Add("string", "System.String");

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
                    currentCommand = item;
                    //Analysis of command parameters
                    return item.CompareToInput(completionList);
                }
                else if (comm != null)
                {
                    completionList.completion.Add(item.command);
                }
            }
            return completionList;
        }
        public void ExecuteCommand(string preInput)
        {
            if (currentCommand != null)
            {
                //CommandUtil.DefaultExecute<typeof(currentCommand)>()
                currentCommand.Execute(preInput);
            }
        }
        /// <summary>
        /// Compare a command
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
    }


}