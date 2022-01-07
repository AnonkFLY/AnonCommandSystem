using System.Text;

namespace CommandSystem
{
    public class TeleportCommand : CommandStruct
    {
        public float x, y, z;
        public float qx, qy, qz;
        public string entiteName, qentiteName;
        //public MyClass cust;
        public TeleportCommand()
        {
            command = "teleport";
            parameters = new string[]{
                "<string:entiteName>",
                "<string:entiteName> facing <string:qentiteName>",
                "<float:x> <float:y> <float:z> facing <float:qx> <float:qy>",
                "<myclass:cust>"
            };
        }

        public override string Execute(ExecuteData data)
        {
            switch (data.indexExecute)
            {
                case 0:
                    UnityEngine.Debug.Log($"Teleport On {entiteName}");
                    break;
                case 1:
                    UnityEngine.Debug.Log($"Teleport On {entiteName} facing {qentiteName}");
                    break;
                case 2:
                    UnityEngine.Debug.Log($"Teleport On {x} {y} {z} facing {qx} {qy}");
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