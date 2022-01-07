# AnonCommandSystem
仿照Minecraft(我的世界)基岩版本的命令系统解析

[中文](https://github.com/Anon-K/AnonCommandSystem/README.zh.md)

[English](https://github.com/Anon-K/AnonCommandSystem/README.md)

## 已实现功能:

基本命令解析

```c#
//like this
teleport
    <string:entitie>
    <string:entite> facing <entite>
    <float:x> <float:y> <float:z>
```

增加自定义类型参数解析

```C#
//like this
teleport
    <myClass:parameterName>
```

命令补全列表

```c#
//like this
-teleport
>te
```

命令提示符(未包含[]可选参数)

```c#
//like this
-teleport <string:entitie>
-teleport <string:entite> facing <entite>
-teleport <float:x> <float:y> <float:z>
>teleport 
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
| ExecuteData       | 解析后用于判断执行的数据           |

## 快速开始

**第一步:完成命令类**

创建自己的命令

初始化command变量和paramters如下格式(示例使用了构造函数初始化)

*内部解析是使用反射来进行赋值属性的,所以设定语法只需要与类的属性一致即可*

完成Execute函数

```C#	
namespace CommandSystem
{
    public class KillCommand : CommandStruct
    {
        public string killObj;
        public int killCount;
        public KillCommand()
        {
            command = "kill";
            parameters = new string[]{
                "<int:killCount>",
                "<string:killObj>"
            };
        }

        public override string Execute(ExecuteData data)
        {
            CommandUtil.DebugLog($"execute index is {data.indexExecute},Results of the {data.resultStr}");
            switch (data.indexExecute)
            {
                case 0:
                    Kill(killCount);
                    break;
                case 1:
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
    }
}
```

**第二步:创建命令解析器并注册命令**

使用ParsingCommand解析命令，返回一个字符串

*返回的字符串为重写的Execute方法的返回值*

```C#
var parser = new CommandParser();
//Register Kill Command
parser.AddCommand(new KillCommand());
var resultCount = parser.GetCompletion("kill 1");
print(resultCount);
//Although it can be parsed as a string, the parsing of int is in the front, so the priority is higher than the latter
//Output result:kill of 1
//Print result:0
var resultName = parser.GetCompletion("kill a");
print(resultName);
//Output result:kill name of a
//Print result:1
```

### 使用自定义类

**1.实现接口**

*TryParses*为字符串作为参数转为自定义类的方法

*GetParamterCompletion*为获取该自定义类补全列表的方法

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
parser.AddCustomParameterParsing(CustomParsing);
```



## 将来

+ 解析~(相对坐标)与^(局部坐标)
+ 解析选择器(@a/p/e/s)与选择器参数([name/lm/l...])

+ 解析[]的枚举或可选参数(destroy/replace)
