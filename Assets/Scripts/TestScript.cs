using System.Diagnostics;
using System.Xml;
using System;
using System.Collections;
using System.Text;
using UnityEngine.UI;
using CommandSystem;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public string str;
    public CommandParser parser;
    public Text candidateList;
    public InputField input;
    public SelectorParameter selectorTest;
    public string candidateWord;
    private void Awake()
    {
        parser = new CommandParser();
        var i = TestCommandSystem();
        if (i == -1)
        {
            print("Command System 正常");
        }
        else
        {
            print($"Command System 出错:错误代码:{i}");
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // var tes = str.Split(new char[] { '|', '<', '>' });
            // tes = tes.Where(s => !string.IsNullOrEmpty(s)).ToArray();
            // foreach (var item in tes)
            // {
            //     print(item);
            // }
        }
    }
    public void GetCommand(string input)
    {
        var list = parser.GetCompletion(input);
        StringBuilder candidate = new StringBuilder("");
        foreach (var item in list.completion)
        {
            // if (item. != -1)
            //     candidate.AppendLine($"<color>{item.completion.Substring(0, item.colorIndex)}</color>{item.completion.Remove(0, item.colorIndex)}");
            // else
            candidate.AppendLine(item);
        }
        candidate.AppendLine("Prompt:");
        foreach (var item in list.promptList)
        {
            candidate.AppendLine(item);
        }
        // if (list != null && list.Length > 0)
        //     candidateWord = list[list.Length - 1].completion.Remove(0, input.Length);

        candidateList.text = candidate.ToString();
    }
    public void EnterTheInput(string input)
    {
        var back = parser.ExecuteCommand(input);
        print(back);
        this.input.text = "";
    }
    public IEnumerator MoveToEnd()
    {
        yield return new WaitForEndOfFrame();
        input.MoveTextEnd(false);
    }
    // private bool CustomParsing(ParameterStruct para, string input)
    // {
    //     if (para.tString == "custom")
    //         if (int.TryParse(input, out var getValue))
    //         {
    //             para.getValue = getValue;
    //             return true;
    //         }
    //     return false;
    // }
    public int TestCommandSystem()
    {
        var commands = new string[]
        {
            "teleport 1 2 3 facing 1 23",
            "teleport",
            "teleport 1",
            "teleport @a",
            "teleport @e[c=1]",
            "teleport @e[=1] facing @a"
        };
        var bools = new string[]
        {
            "3",
            "-1",
            "1",
            "0",
            "0",
            "-1"
        };
        for (int i = 0; i < commands.Length; i++)
        {
            parser.GetCompletion(commands[i]);
            var result = parser.ExecuteCommand(commands[i]);
            if (result != bools[i])
            {
                return i;
            }
        }
        return -1;
    }
}
public class MyClass : ICommandParameter<MyClass>
{
    public int id;
    public string name;
    public string[] GetParameteCompletion(string preInput)
    {
        UnityEngine.Debug.Log(preInput);
        return new string[] { "A", "B" };
    }
    public bool TryParse(string input, out MyClass getValue)
    {
        getValue = new MyClass();
        getValue.id = input.Length;
        getValue.name = input;
        return true;
    }

}
