using AnonCommandSystem;

namespace AnonCommandSyste.ExampleCommandm
{
    public class TeleportCommand : CommandStruct
    {
        public float x, y, z;
        public float qx, qy;
        public SelectorParameter entiteName, qentiteName;
        public string cust;
        public string mode;

        public override string Execute(ParsingData data)
        {
            //UnityEngine.Debug.Log($"{data.parsingResult} and {data.indexExecute}");
            // if (data.indexExecute == -1)
            // {
            //     return "Parsing failed";
            // }
            switch (data.indexExecute)
            {
                case 1:
                    UnityEngine.Debug.Log($"Teleport String On {cust}");
                    break;
                case 0:
                    UnityEngine.Debug.Log($"Teleport On @{entiteName.target} and {entiteName.c}");
                    break;
                case 2:
                    UnityEngine.Debug.Log($"Teleport On {x} {y} {z} facing [{qx}] [{qy}]");
                    break;
                case 3:
                    break;
            }
            //CommandUtil.DebugLog($"{data.indexExecute} of {data.resultStr}");
            //UnityEngine.Debug.Log($"cust value is {cust.id} and {cust.name}");
            return data.indexExecute.ToString();
        }

        public override void InitCommand(CommandParser parser)
        {
            command = "teleport";
            parameters = new string[]{
                "<Selector:entiteName>",
                 "<string:cust>",
                "<Selector:entiteName> facing <Selector:qentiteName>",
                 "<float:x> <float:y> <float:z> [replace|moved|keep|abab:mode]",
                "<float:x> <float:y> <float:z> facing <float:qx> <float:qy>"
            };
        }
    }
}