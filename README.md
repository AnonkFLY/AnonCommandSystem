# AnonCommandSystem
仿照Minecraft(我的世界)基岩版本的命令系统解析

[中文](https://github.com/Anon-K/AnonCommandSystem/blob/master/README.md)

[English](https://github.com/Anon-K/AnonCommandSystem/blob/master/README.en.md)

## 已实现功能:

基本命令解析

```c#
//like this
teleport
    <string:entitie>
    <string:entite> facing <entite>
    <float:x> <float:y> <float:z>
//teleport a
//teleport a facing b
//teleport 1 2 3
```

增加自定义类型参数解析

```C#
//like this
teleport
    <myClass:parameterName>
```

命令补全列表(包含:命令补全,可选参数补全,目标选择器补全)

```c#
//like this
-teleport
>te
```

命令提示符(包含:***必填参数***,***语法参数***,***选填参数***)

```c#
//like this
-teleport <string:entitie>
-teleport <string:entite> facing <entite>
-teleport <float:x> <float:y> <float:z>
	>teleport 
//or this
-teleport <float:x> <float:y> <float:z> [replace|moved|keep|abab:mode]
>teleport 1 2 3 repalce
	>teleport 1 2 3
//teleport 1 2 3 on [replace]
//teleport 1 2 3 on []
    //or
-teleport <float:x> <float:y> <float:z> [int:mode]
>teleport 1 2 3 4
```

目标选择器和参数扩展

```C#
//like this
-teleport <Selector:entiteName>
>teleport @a[c=5]
//Teleport On @a and 5
```

## 简述

| 类名              | 功能                               |
| ----------------- | ---------------------------------- |
| CommandParser     | 命令解析器,并储存命令列表数据      |
| CommandStruct     | 所有命令的抽象类                   |
| CommandParameter  | 自定义参数类型的接口               |
| CommandUtil       | 命令的工具类(包含了大多数核心功能) |
| ReturnCommandData | 命令解析数据,用于使用本功能        |
| CommandPrompt     | 命令提示符数据                     |
| ParsingData       | 解析后用于判断执行的数据           |

## 快速开始

**第一步:完成命令类**

创建自己的命令

1.初始化command变量和paramters如下格式(示例使用了构造函数初始化)

*内部解析是使用反射来进行赋值属性的,所以设定语法只需要与类的属性一致即可*

2.完成Execute函数

```C#	
namespace AnonCommandSystem
{
    public class KillCommand : CommandStruct
    {
        public string killObj;
        public int killCount;
        public Selector entitie;
        public KillCommand()
        {
            command = "kill";
            parameters = new string[]{
                "<int:killCount>",
                "<Selector:entitie>",
                "<string:killObj>"
            };
        }

        public override string Execute(ParsingData data)
        {
            switch (data.indexExecute)
            {
                case 0:
                    Kill(killCount);
                    break;
                case 1:
                    Kill(entitie);
                    break;
                case 2:
                    Kill(killObj);
                    break;
            }
            return data.indexExecute.ToString();
        }
        private void Kill(int count)
        {
            CommandUtil.DebugLog("kill of " + count);
        }
        private void Kill(string killName)
        {
            CommandUtil.DebugLog("kill name of " + killName);
        }
        private void Kill(SelectorParameter killTarget)
        {
            CommandUtil.DebugLog($"kill Selector @{killTarget.target} count is {killTarget}");
        }
    }
}
```

**第二步:创建命令解析器并注册命令**

使用ExecuteCommand(string input)解析并执行命令，返回一个字符串

*返回的字符串为重写的Execute方法的返回值*

```C#
var parser = new CommandParser();
//Register Kill Command
parser.RegisterCommand(new KillCommand());
var resultCount = parser.ExecuteCommand("kill 1");
print(resultCount);
//Although it can be parsed as a string, the parsing of <int> is in the front, so the priority is higher than the latter
//Output result:kill of 1
//Print result:0
var resultName = parser.ExecuteCommand("kill @a[c=3]");
print(resultName);
//Output result:kill Selector @a count is 3
//Print result:1
var resultName = parser.Exec("kill a");
print(resultName);
//Output result:kill name of a
//Print result:2
```

### 获取补全与语法提示

```C#
var parser = new CommandParser();
ReturnCommandData data = parser.ParseCommand("teleport ");
//ReturnCommandData all property
public class ReturnCommandData
{
    /// <summary>
    /// parsed command result
    /// </summary>
    public ParsingData parsingData;
    /// <summary>
    /// Completion list
    /// </summary>
    public List<string> completionList;
    /// <summary>
    /// command prompt list
    /// </summary>
    public HashSet<string> promptList;
}
```



### 使用自定义类

**1.实现接口**

*TryParses*为**字符串作为参数转为自定义类的方法**

*GetParamterCompletion*为**获取该自定义类补全列表的方法**

```C#
namespcae CommandSystem;
public class MyClass : ICommandParameter<MyClass>
{
    public int id;
    public string name;
    public string[] GetParameteCompletion(string preInput)
    {
        throw new NotImplementedException();
    }
    public bool TryParse(string input, out MyClass getValue)
    {
        getValue = new MyClass();
        getValue.id = input.Length;
        getValue.name = input;
        return true;
    }
}
```

**2.注册类型**

```C#	
var parser = new CommandParser();
parser.AddCustomParameterParsing<MyClass>("myclass");
---------
parameters = new string[]
{
    "<myclass:propertyName>"
}
---------
```

或者...你仅仅是想自定义类型名字

可以使用

```C#
private bool CustomParsing(ParameterStruct para, string input)
{
    if (para.tString == "custom")
        if (int.TryParse(input, out var getValue))
        {
            para.getValue = getValue;
            return true;
        }
    return false;
}
//then register the type
parser.AddCustomParameterParsing(CustomParsing);
```



## 将来

+ 解析~(相对坐标)与^(局部坐标)
+ ~~解析选择器(@a/p/e/s)与选择器参数([name/lm/l...])~~

+ ~~解析[]的枚举或可选参数(destroy/replace)~~
