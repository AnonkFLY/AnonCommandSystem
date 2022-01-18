using System.Linq;
using System;
using System.Collections.Generic;
namespace AnonCommandSystem
{
    public class SelectorParameter : ICommandParameter<SelectorParameter>
    {
        protected List<char> selectorMask = new List<char>(new char[] { 'a', 'p', 'e', 's' });
        public string[] parameterList;
        public char target;
        public int c;
        public string[] GetParameteCompletion(string preInput)
        {
            return selectorMask.Select<char, string>(c => { return "@" + c.ToString(); }).ToArray();
        }
        public bool TryParse(string input, out SelectorParameter getValue)
        {
            getValue = this;
            try
            {
                if (input[0] != '@')
                    return false;
                var selector = input[1];
                var parameters = input.Split(' ', '[', '@', ']');
                if (parameters[0].Length > 1 || !selectorMask.Contains(selector))
                    return false;
                this.target = selector;
                if (parameters.Length == 2)//if only @x
                    return true;
                var selectorPara = parameters[2].Split(',');
                if (string.IsNullOrEmpty(parameters[2]))
                    return false;
                for (int i = 0; i < selectorPara.Length; i++)
                {
                    parameterList = new string[selectorPara.Length];
                    if (!ParsingSelectorParameter(selectorPara[i], out var paraName))
                        return false;
                    parameterList[i] = paraName;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool ParsingSelectorParameter(string parameter, out string parameterName)
        {
            var keyValue = parameter.Split('=');
            parameterName = keyValue[0];
            try
            {
                var para = CommandUtil.GetParameterStruct($"<{GetSelectorParameterType(parameterName)}:{parameterName}>");
                para.strValue = keyValue[1];
                var canSet = CommandUtil.CompareParameterOnInput(para);
                if (canSet)
                {
                    CommandUtil.ReflectionSetValue(this, parameterName, para.getValue);
                }
                return canSet;
            }
            catch
            {
                return false;
            }
        }
        private Type GetSelectorParameterType(string parameterName)
        {
            return this.GetType().GetField(parameterName).FieldType;
        }
    }

}