namespace AnonCommandSystem.ExampleCommand
{
    public class KillCommand : CommandStruct
    {
        public string killObj;
        public int killCount;
        public SelectorParameter entitie;
        public override string Execute(ParsingData data)
        {
            switch (data.indexExecute)
            {
                case 0:
                    Kill(killCount);
                    break;
                case 1:
                    Kill(entitie);
                    break;
                case 2:
                    Kill(killObj);
                    break;
            }
            return data.indexExecute.ToString();
        }

        public override void InitCommand()
        {
            command = "kill";
            parameters = new string[]{
                "<int:killCount>",
                "<Selector:entitie>",
                "<string:killObj>"
            };
        }

        private void Kill(int count)
        {
            CommandUtil.DebugLog("kill of " + count);
        }
        private void Kill(string killName)
        {
            CommandUtil.DebugLog("kill name of " + killName);
        }
        private void Kill(SelectorParameter killTarget)
        {
            CommandUtil.DebugLog($"kill Selector @{killTarget.target} count is {killTarget}");
        }
    }
}