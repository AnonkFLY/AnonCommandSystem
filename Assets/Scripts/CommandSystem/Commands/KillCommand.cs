

namespace CommandSystem
{
    public class KillCommand : CommandStruct
    {
        public KillCommand()
        {
            command = "taKill";
        }
        public override ReturnCommandData CompareToInput(ReturnCommandData preInput)
        {
            throw new System.NotImplementedException();
        }

        public override string Execute(string inputCommand)
        {
            throw new System.NotImplementedException();
        }
    }
}