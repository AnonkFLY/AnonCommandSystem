using System.Diagnostics;
using System.Xml;
using System;
using System.Collections;
using System.Text;
using UnityEngine.UI;
using AnonCommandSystem;
using UnityEngine;
using UnityEditor;
using AnonCommandSyste.ExampleCommandm;
using AnonCommandSystem.ExampleCommand;

public class TestScript : MonoBehaviour
{
    public string str;
    public CommandParser parser;
    public Text candidateList;
    public InputField input;
    public string candidateWord;
    [MenuItem("Debug/Button")]
    public static void DebugButton()
    {
        var str = "\"a  b\"";
        print(str.Length);
        var i = str.IndexOf("\"", 0);
        if (i == -1)
            print("无");
        print(i);
        var a = str.Substring(0, i);
        var j = str.IndexOf("\"", i + 1);
        if (j == -1)
            print("无");
        print(j);
        var b = str.Substring(i, j).Replace(' ', '+');
        var c = str.Substring(j);
        str = a + b + c;
        print(str);
    }
    private void Awake()
    {
        parser = new CommandParser();
        parser.RegisterCommand(new TeleportCommand());
        parser.RegisterCommand(new KillCommand());
        parser.RegisterCommand(new StaticCommand());
        // var method = CommandUtil.GetStaticTryParse(typeof(int));
        // var objs = new object[] { "1", 0 };
        // var b =  method.Invoke(objs[1],objs);
        // print($"{b} and {objs[1]}");

        TestSystem();
    }
    private void TestSystem()
    {
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
        var list = parser.ParseCommand(input);
        StringBuilder candidate = new StringBuilder("");
        if (list == null)
            return;
        foreach (var item in list.completionList)
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
        var back = parser.ExecuteCommand();
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
        var commands = new TestCommand[]
        {
            new TestCommand("teleport 1 2 3 facing 1 23",4),
            new TestCommand("teleport",-1),
            new TestCommand("teleport 1",1),
            new TestCommand("teleport @a",0),
            new TestCommand("teleport @a[c=1]",0),
            new TestCommand("teleport 1 2 3 replace",3),
            new TestCommand("teleport 12  3  4",3),
            new TestCommand("teleport @a facing @s[c=1]",2)
        };

        for (int i = 0; i < commands.Length; i++)
        {
            var result = parser.ExecuteCommand(commands[i].command);
            if (result != commands[i].result.ToString())
            {
                return i;
            }
        }
        return -1;
    }
}
public struct TestCommand
{
    public string command;
    public int result;
    public TestCommand(string command, int result)
    {
        this.command = command;
        this.result = result;
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
