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
            return CommandUtil.DefaultAnalysis<TeleportCommand>(this, resultData);
        }

        public override string Execute(string inputCommand)
        {
            return null;
        }

    }
}