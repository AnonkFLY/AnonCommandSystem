using System.Collections;
using System.Collections.Generic;
using AnonCommandSystem;
using UnityEngine;

namespace AnonCommandSyste.ExampleCommandm
{
    public class StaticCommand : CommandStruct
    {
        public override string Execute(ParsingData data)
        {
            return "Execute";
        }
        public override void InitCommand()
        {
            command = "static";
        }
    }
}
