using System.Diagnostics;
using System.Text;

namespace CommandSystem
{
    public class TeleportCommand : CommandStruct
    {
        public float x, y, z;
        public float qx, qy;
        public string entiteName, qentiteName;
        public MyClass cust;
        public TeleportCommand()
        {
            command = "teleport";
            parameters = new string[]{
                //"<string:entiteName>",
                "<string:entiteName> facing <string:qentiteName>",
                "<float:x> <float:y> <float:z> facing <float:qx> <float:qy>",
                "<Custom:cust>"
            };
        }

        public override string Execute(ExecuteData data)
        {
            if (data.indexExecute == -1)
                UnityEngine.Debug.Log("解析失败");
            UnityEngine.Debug.Log(data.resultStr);
            switch (data.indexExecute)
            {
                case 0:
                    UnityEngine.Debug.Log($"Teleport On {entiteName}");
                    break;
                case 1:
                    UnityEngine.Debug.Log($"Teleport On {entiteName} facing {qentiteName}");
                    break;
                case 2:
                    UnityEngine.Debug.Log($"Teleport On {cust.id} on {cust.name}");
                    break;
                case 3:
                    UnityEngine.Debug.Log($"wait test");
                    break;
            }
            //CommandUtil.DebugLog($"{data.indexExecute} of {data.resultStr}");
            //UnityEngine.Debug.Log($"cust value is {cust.id} and {cust.name}");
            return "Done";
        }
    }
}