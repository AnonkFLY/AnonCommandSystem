using System.Xml;
using System;
using System.Collections;
using System.Text;
using UnityEngine.UI;
using CommandSystem;
using UnityEngine;

public class StringToIntTest : MonoBehaviour
{
    public string str;
    public CommandParser paeser;
    public Text candidateList;
    public InputField input;
    public string candidateWord;
    private void Awake()
    {
        paeser = new CommandParser();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            input.text += candidateWord;
            StartCoroutine(MoveToEnd());
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
        var list = paeser.GetCompletion(input);
        StringBuilder candidate = new StringBuilder("");
        foreach (var item in list.prompt.promptList)
        {
            candidate.AppendLine(item);
        }
        // if (list != null && list.Length > 0)
        //     candidateWord = list[list.Length - 1].completion.Remove(0, input.Length);
        foreach (var item in list.completion)
        {
            // if (item. != -1)
            //     candidate.AppendLine($"<color>{item.completion.Substring(0, item.colorIndex)}</color>{item.completion.Remove(0, item.colorIndex)}");
            // else
            candidate.AppendLine(item);
        }
        candidateList.text = candidate.ToString();
    }
    public IEnumerator MoveToEnd()
    {
        yield return new WaitForEndOfFrame();
        input.MoveTextEnd(false);
    }
}
