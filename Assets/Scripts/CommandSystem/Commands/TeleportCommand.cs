using System.Diagnostics;
using System.Text;

namespace CommandSystem
{
    public class TeleportCommand : CommandStruct
    {
        public float x, y, z;
        public float qx, qy;
        public SelectorParameter entiteName, qentiteName;
        public string cust;
        public TeleportCommand()
        {
            command = "teleport";
            parameters = new string[]{
                "<Selector:entiteName>",
                "<string:cust>",
                "<Selector:entiteName> facing <Selector:qentiteName>",
                "<float:x> <float:y> <float:z> facing <float:qx> <float:qy>"
            };
        }

        public override string Execute(ExecuteData data)
        {
            UnityEngine.Debug.Log($"{data.resultStr}");
            // if (data.indexExecute == -1)
            // {
            //     return "Parsing failed";
            // }
            // switch (data.indexExecute)
            // {
            //     case 0:
            //         UnityEngine.Debug.Log($"Teleport On {data.indexExecute}");
            //         break;
            //     case 1:
            //         UnityEngine.Debug.Log($"Teleport On {data.indexExecute}");
            //         break;
            //     case 2:
            //         UnityEngine.Debug.Log($"Teleport On {data.indexExecute}");
            //         break;
            //     case 3:
            //         UnityEngine.Debug.Log($"Teleport On {data.indexExecute}");
            //         break;
            // }
            //CommandUtil.DebugLog($"{data.indexExecute} of {data.resultStr}");
            //UnityEngine.Debug.Log($"cust value is {cust.id} and {cust.name}");
            return data.indexExecute.ToString();
        }
    }
}