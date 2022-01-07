# AnonCommandSystem

Analysis of the command system modeled after the bedrock version of Minecraft(Waiting for translation)

[中文](README.zh.md)

[English](README.md)

## Completed:

Basic command analysis

```c#
//like this
teleport
    <string:entitie>
    <string:entite> facing <entite>
    <float:x> <float:y> <float:z>
```

Customize your type parameter resolution

```C#
//like this
teleport
    <myClass:parameterName>
```

Command completion list

```c#
//like this
-teleport
>te
```

Command prompt (without [] optional parameters)

```c#
//like this
-teleport <string:entitie>
-teleport <string:entite> facing <entite>
-teleport <float:x> <float:y> <float:z>
>teleport 
```

## Bewrite

| class             | intention                          |
| ----------------- | ---------------------------------- |
| CommandParser     | 命令解析器,并储存命令列表数据      |
| CommandStruct     | 所有命令的抽象类                   |
| CommandParameter  | 自定义参数类型的接口               |
| CommandUtil       | 命令的工具类(包含了大多数核心功能) |
| ReturnCommandData | 命令解析数据,用于使用本功能        |
| CommandPrompt     | 命令提示符数据                     |
| ExecuteData       | 解析后用于判断执行的数据           |

## Quick start

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

**第二步:创建命令解析器并使用ParsingCommand解析命令，返回一个字符串**

*返回的字符串为重写的Execute方法的返回值*

```C#
var parser = new CommandParser();
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

## 将来

+ 解析~(相对坐标)与^(局部坐标)
+ 解析选择器(@a/p/e/s)与选择器参数([name/lm/l...])

+ 解析[]的枚举或可选参数(destroy/replace)
