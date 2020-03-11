using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace KhaiosCore
{
    public class HelpCommand : ModCommand
    {
        public override string Command => "commands";

        public override CommandType Type => CommandType.Chat;

        public override string Usage => "/commands [command]";

        public override string Description => "Lists the commands or syntax for command terminals.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if(args.Length == 0)
            {
                caller.Reply("Commands: time, weather, item, say, kill, event, summon");
            } else
            {
                string lower = args[0].ToString().ToLower();
                switch(lower)
                {
                    case "time":
                        caller.Reply("t|time set (value)|day|night|midday|midnight or t | time add(value)");
                        break;
                    case "weather":
                        caller.Reply("w|weather set|toggle clear|rain|storm|slime or w | weather clear");
                        break;
                    case "item":
                        caller.Reply("i|item give|take \"name\"|(id) [quantity]");
                        break;
                    case "say":
                        caller.Reply("s|say \"message\"");
                        break;
                    case "kill":
                        caller.Reply("k|kill player|npc all|(radius)");
                        break;
                    case "event":
                        caller.Reply("e|event start bloodmoon|goblin|legion|solar|pirate|pumpkin|frost|martian");
                        break;
                    case "summon":
                        caller.Reply("su|summon (id)|\"name\" ~x ~y");
                        break;
                    default:
                        caller.Reply("Commands: time, weather, item, say, kill, event, summon");
                        throw new UsageException("Command provided is not a valid command!");
                }
            }
        }
    }
}
