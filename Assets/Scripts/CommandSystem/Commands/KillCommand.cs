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