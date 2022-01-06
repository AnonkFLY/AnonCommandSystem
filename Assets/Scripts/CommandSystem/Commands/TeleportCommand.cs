using System.Text;

namespace CommandSystem
{
    public class TeleportCommand : CommandStruct
    {
        public float x, y, z;
        public float qx, qy, qz;
        public string entiteName, qentiteName;
        public TeleportCommand()
        {
            command = "teleport";
            parameter = new string[]{
                "<string:entiteName>",
                "<string:entiteName> facing <string:qentiteName>",
                "<float:x> <float:y> <float:z> facing <float:qx> <float:qy>"
            };
        }
        public override ReturnCommandData CompareToInput(ReturnCommandData resultData)
        {
            var result = CommandUtil.DefaultAnalysis<TeleportCommand>(this, resultData.preInput);
            return result;
        }

        public override string Execute(string inputCommand)
        {
            var back = CommandUtil.DefaultExecute<TeleportCommand>(this, inputCommand);
            CommandUtil.DebugLog($"解析结果:{back.resultStr}\n解析索引:{back.indexExecute}");
            return null;
        }

    }
}