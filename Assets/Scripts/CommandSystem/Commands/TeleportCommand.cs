using System.Text;

namespace CommandSystem
{
    public class TeleportCommand : CommandStruct
    {
        public float x, y, z;
        public float qx, qy, qz;
        public string entiteName, qentiteName;
        public MyClass cust;
        public TeleportCommand()
        {
            command = "teleport";
            parameter = new string[]{
                //"<string:entiteName>",
                //"<string:entiteName> facing <string:qentiteName>",
                //"<float:x> <float:y> <float:z> facing <float:qx> <float:qy>",
                "<myclass:cust>"
            };
        }

        public override string Execute(ExecuteData data)
        {
            //CommandUtil.DebugLog($"{data.indexExecute} of {data.resultStr}");
            UnityEngine.Debug.Log($"cust value is {cust.id} and {cust.name}");
            return "命令未实现";
        }
    }
}