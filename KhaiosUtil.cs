using KhaiosCore.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.Localization;
using Terraria.ModLoader;
using log4net;

namespace KhaiosCore
{
    public static class KhaiosUtil
    {
        #region METHODS

        private static MethodInfo _startRainMethod;
        private static MethodInfo _startSandstormMethod;
        private static MethodInfo _stopRainMethod;
        private static MethodInfo _stopSandstormMethod;

        private static MethodInfo _startSlimeRainMethod;
        private static MethodInfo _stopSlimeRainMethod;

        private static MethodInfo _startPumpkinMoonMethod;
        private static MethodInfo _startFrostMoonMethod;

        #endregion

        #region INITIALIZE

        public static void Init()
        {
            InitReflection();
        }

        public static void InitReflection()
        {
            try
            {
                _startRainMethod = typeof(Main).GetMethod("StartRain", BindingFlags.NonPublic | BindingFlags.Static);
                _startSlimeRainMethod = typeof(Main).GetMethod("StartSlimeRain", BindingFlags.Public | BindingFlags.Static);
                _stopRainMethod = typeof(Main).GetMethod("StopRain", BindingFlags.NonPublic | BindingFlags.Static);
                _stopSlimeRainMethod = typeof(Main).GetMethod("StopSlimeRain", BindingFlags.Public | BindingFlags.Static);
                _startSandstormMethod = typeof(Sandstorm).GetMethod("StartSandstorm", BindingFlags.NonPublic | BindingFlags.Static);
                _stopSandstormMethod = typeof(Sandstorm).GetMethod("StopSandstorm", BindingFlags.NonPublic | BindingFlags.Static);
                _startPumpkinMoonMethod = typeof(Main).GetMethod("StartPumpkinMoon", BindingFlags.Public | BindingFlags.Static);
                _startFrostMoonMethod = typeof(Main).GetMethod("StartSnowMoon", BindingFlags.Public | BindingFlags.Static);

            }
            catch (Exception e)
            {
                Log(e.Message + " " + e.StackTrace);
            }
        }

        #endregion

        #region INVOKE METHODS

        public static void StartRain()
        {
            _startRainMethod.Invoke(null, null);
        }

        public static void StartSandstorm()
        {
            _startSandstormMethod.Invoke(null, null);
        }

        public static void StopRain()
        {
            _stopRainMethod.Invoke(null, null);
        }

        public static void StopSandstorm()
        {
            _stopSandstormMethod.Invoke(null, null);
        }

        public static void StartSlimeRain()
        {
            _startSlimeRainMethod.Invoke(null, new object[] { true });
        }

        public static void StopSlimeRain()
        {
            _stopSlimeRainMethod.Invoke(null, new object[] { true });
        }

        public static void StartPumpkinMoon()
        {
            _startPumpkinMoonMethod.Invoke(null, null);
        }

        public static void StartFrostMoon()
        {
            _startFrostMoonMethod.Invoke(null, null);
        }

        #endregion

        #region COMMANDS
        
        public static bool TestCommand(string input, ref string error)
        {
            error = "";
            List<string> argsList = SplitString(input);
            if(argsList.Count == 0)
            {
                error = "Command cannot be blank!";
                return false;
            }
            for (int i = 0; i < argsList.Count; i++) argsList[i] = argsList[i].ToLower();

            string command = argsList[0];
            argsList.RemoveAt(0);

            double num; bool isNumeric;
            switch (command)
            {
                #region TIME TEST COMMAND
                case "time":
                case "t":
                    if(argsList.Count == 0)
                    {
                        error = "Command Syntax error! \"t|time set (value)|day|night|midday|midnight\" or \"t|time add (value)\"";
                        return false;
                    }
                    switch(argsList[0])
                    {
                        case "set":
                            if(argsList.Count != 2)
                            {
                                error = "Command Syntax error! \"t|time set (value)|day|night|midday|midnight\" or \"t|time add (value)\"";
                                return false;
                            }
                            switch(argsList[1])
                            {
                                case "day":
                                case "night":
                                case "midday":
                                case "midnight":
                                    return true;
                                default:
                                    isNumeric = double.TryParse(argsList[1], out _);
                                    if (isNumeric) return true;
                                    error = "Command Syntax error! \"t|time set (value)|day|night|midday|midnight\" or \"t|time add (value)\"";
                                    return false;
                            }
                        case "add":
                            if (argsList.Count != 2)
                            {
                                error = "Command Syntax error! \"t|time add (value)\"";
                                return false;
                            }
                            isNumeric = double.TryParse(argsList[1], out _);
                            if (isNumeric) return true;
                            error = "Command Syntax error! \"t|time add (value)\"";
                            return false;
                        default:
                            error = "Command Syntax error! \"t|time set (value)|day|night|midday|midnight\" or \"t|time add (value)\"";
                            return false;
                    }
                    #endregion
                #region WEATHER TEST COMMAND
                case "weather":
                case "w":
                    if (argsList.Count == 0)
                    {
                        error = "Command Syntax error! \"w|weather set|toggle clear|rain|storm|slime\" or \"w|weather clear\"";
                        return false;
                    }
                    switch(argsList[0])
                    {
                        case "set":
                        case "toggle":
                            if(argsList.Count != 2)
                            {
                                error = "Command Syntax error! \"w|weather set|toggle clear|rain|storm|slime\"";
                                return false;
                            }
                            switch(argsList[1])
                            {
                                case "clear":
                                    if (argsList[0] == "set") return true;
                                    error = "Command Syntax error! \"w|weather toggle rain|storm|slime\"";
                                    return false;
                                case "rain":
                                case "slime":
                                case "storm":
                                    return true;
                                default:
                                    error = "Command Syntax error! \"w|weather set|toggle clear|rain|storm|slime\"";
                                    return false;
                            }
                        case "clear":
                            return true;
                        default:
                            error = "Command Syntax error! \"w|weather set|toggle clear|rain|storm|slime\" or \"w|weather clear\"";
                            return false;
                    }
                #endregion
                #region ITEM TEST COMMAND
                case "item":
                case "i":
                    if(argsList.Count <= 1)
                    {
                        error = "Command Syntax error! \"i|item give|take \"name\"|(id) [quantity]\"";
                        return false;
                    }
                    switch(argsList[0])
                    {
                        case "give":
                        case "take":
                            isNumeric = double.TryParse(argsList[1], out num);
                            if(isNumeric)
                            {
                                if((int)num >= ItemLoader.ItemCount)
                                {
                                    error = "Command Syntax error! \"i|item give|take \"name\"|(id) [quantity]\" Id greater than value!";
                                    return false;
                                }
                            } else
                            {
                                Item item = new Item();
                                string name = argsList[1].Replace("\"", "");
                                name = name.Replace("_", " ");
                                for (var i = 0; i < ItemLoader.ItemCount; i++)
                                {
                                    item.SetDefaults(i, true);
                                    if (name != Lang.GetItemNameValue(i).ToLower()) continue;
                                    num = i;
                                    break;
                                }
                                if ((int)num >= ItemLoader.ItemCount)
                                {
                                    error = "Command Syntax error! \"i|item give|take \"name\"|(id) [quantity]\" Id greater than value!";
                                    return false;
                                }
                            }
                            if(argsList.Count >= 3)
                            {
                                isNumeric = double.TryParse(argsList[2], out _);
                                if (!isNumeric)
                                {
                                    error = "Command Syntax error! \"i|item give|take \"name\"|(id) [quantity]\" Quantity not a number!";
                                    return false;
                                }
                                return true;
                            } else
                            {
                                return true;
                            }
                        default:
                            error = "Command Syntax error! \"i|item give|take \"name\"|(id) [quantity]\"";
                            return false;
                    }
                #endregion
                #region SAY TEST COMMAND
                case "say":
                case "s":
                    if(argsList.Count != 1)
                    {
                        error = "Command Syntax Error! \"s|say \"message\"\"";
                        return false;
                    }
                    return true;
                #endregion
                #region KILL TEST COMMAND
                case "kill":
                case "k":
                    if(argsList.Count == 0)
                    {
                        error = "Command Syntax Error! \"k|kill player|npc all|(radius)\"";
                        return false;
                    }
                    switch(argsList[0])
                    {
                        case "player":
                        case "npc":
                            if (argsList[1] == "all") return true;
                            else
                            {
                                isNumeric = double.TryParse(argsList[1], out _);
                                if(!isNumeric)
                                {
                                    error = "Command Syntax Error! \"k|kill player|npc all|(radius)\"";
                                    return false;
                                }
                                return true;
                            }
                        default:
                            error = "Command Syntax Error! \"k|kill player|npc all|(radius)\"";
                            return false;
                    }
                #endregion
                #region EVENT TEST COMMAND
                case "event":
                case "e":
                    if (argsList.Count == 0)
                    {
                        error = "Command Syntax error! \"e|event start bloodmoon|goblin|legion|solar|pirate|pumpkin|frost|martian";
                        return false;
                    }
                    if(argsList[0] == "start")
                    {
                        switch(argsList[1])
                        {
                            case "bloodmoon":
                            case "goblin":
                            case "legion":
                            case "solar":
                            case "pirate":
                            case "pumpkin":
                            case "frost":
                            case "martian":
                                return true;
                            default:
                                error = "Command Syntax error! \"e|event start bloodmoon|goblin|legion|solar|pirate|pumpkin|frost|martian";
                                return false;
                        }
                    } else
                    {
                        error = "Command Syntax error! \"e|event start bloodmoon|goblin|legion|solar|pirate|pumpkin|frost|martian";
                        return false;
                    }
                #endregion
                #region SUMMON TEST COMMAND
                case "summon":
                case "su":
                    if(argsList.Count == 0)
                    {
                        error = "Command Syntax Error! \"su|summon (id)|\"name\" ~x ~y\"";
                        return false;
                    }
                    isNumeric = double.TryParse(argsList[0], out num);
                    if(isNumeric)
                    {
                        if(num < -65 || num >= NPCLoader.NPCCount)
                        {
                            error = "Command Syntax Error! \"su|summon (id)|\"name\" ~x ~y\"";
                            return false;
                        }
                    } else
                    {
                        string name = argsList[0];
                        bool found = false;
                        name = name.Replace("\"", "");
                        name = name.Replace("_", " ");
                        name = name.ToLower();
                        NPC npc = new NPC();
                        for (var i = -65; i < NPCLoader.NPCCount; i++)
                        {
                            npc.SetDefaults(i);
                            if (name != Lang.GetNPCNameValue(i).ToLower()) continue;
                            found = true;
                        }
                        if (!found)
                        {
                            error = "Command Syntax Error! \"su|summon (id)|\"name\" ~x ~y\"";
                            return false;
                        }
                    }
                    isNumeric = double.TryParse(argsList[1], out _);
                    if(!isNumeric)
                    {
                        error = "Command Syntax Error! \"su|summon (id)|\"name\" ~x ~y\"";
                        return false;
                    }
                    isNumeric = double.TryParse(argsList[2], out _);
                    if(!isNumeric)
                    {
                        error = "Command Syntax Error! \"su|summon (id)|\"name\" ~x ~y\"";
                        return false;
                    }
                    return true;
                #endregion
                case "help":
                case "h":
                case "commands":
                    error = "Commands: time, weather, item, say, kill, event, summon";
                    return false;
                default:
                    error = "Not a valid command. Use \"Help\" for a list of commands";
                    return false;
            }
        }

        public static void ExecuteCommand(string input, ref CommandTileEntity entity)
        {
            if(!TestCommand(input, ref entity.commandTileError))
            {
                return;
            }
            entity.commandTileError = "";
            List<string> argsList = SplitString(input);
            for (int i = 0; i < argsList.Count; i++) argsList[i] = argsList[i].ToLower();

            string command = argsList[0];
            argsList.RemoveAt(0);
            switch(command)
            {
                case "w":
                case "weather":
                    CallWeatherCommand(argsList);
                    break;
                case "t":
                case "time":
                    CallTimeCommand(argsList);
                    break;
                case "i":
                case "item":
                    CallItemCommand(argsList);
                    break;
                case "s":
                case "say":
                    argsList = SplitString(input);
                    argsList.RemoveAt(0);
                    CallSayCommand(argsList);
                    break;
                case "k":
                case "kill":
                    CallKillCommand(argsList, entity);
                    break;
                case "e":
                case "event":
                    CallEventCommand(argsList);
                    break;
                case "su":
                case "summon":
                    CallSummonCommand(argsList, entity);
                    break;
            }
        }

        private static void CallWeatherCommand(List<string> args)
        {
            switch(args[0])
            {
                case "set":
                    switch(args[1])
                    {
                        case "clear":
                            StopRain();
                            StopSandstorm();
                            StopSlimeRain();
                            break;
                        case "rain":
                            StartRain();
                            break;
                        case "storm":
                            StartSandstorm();
                            break;
                        case "slime":
                            StartSlimeRain();
                            break;
                    }
                    break;
                case "toggle":
                    switch(args[1])
                    {
                        case "rain":
                            if(Main.raining)
                            {
                                StopRain();
                            } else
                            {
                                StartRain();
                            }
                            break;
                        case "storm":
                            if(Main.raining)
                            {
                                StopSandstorm();
                            } else
                            {
                                StartSandstorm();
                            }
                            break;
                        case "slime":
                            if(Main.slimeRain)
                            {
                                StopSlimeRain();
                            } else
                            {
                                StartSlimeRain();
                            }
                            break;
                    }
                    break;
                case "clear":
                    StopRain();
                    StopSandstorm();
                    break;
            }
        }

        private static void CallTimeCommand(List<string> args)
        {
            double num;
            bool isNumeric;
            switch(args[0])
            {
                case "set":
                    switch(args[1])
                    {
                        case "day":
                            Main.time = Main.dayTime ? 0 : Main.nightLength;
                            break;
                        case "night":
                            Main.time = Main.dayTime ? Main.dayLength : 0;
                            break;
                        case "midday":
                            Main.time = Main.dayTime ? 0 : Main.nightLength;
                            Main.time += 26980;
                            break;
                        case "midnight":
                            Main.time = Main.dayTime ? Main.dayLength : 0;
                            Main.time += 16120;
                            break;
                        default:
                            isNumeric = double.TryParse(args[1], out num);
                            if(isNumeric)
                            {
                                Main.time = num % Main.dayLength;
                                break;
                            }
                            break;
                    }
                    break;
                case "add":
                    isNumeric = double.TryParse(args[1], out num);
                    if(isNumeric)
                    {
                        Main.time += num % Main.dayLength;
                        break;
                    }
                    break;
            }
        }

        private static void CallItemCommand(List<string> args)
        {
            int num;
            bool isNumeric;

            int type; int quantity;
            switch (args[0])
            {
                case "give":
                    isNumeric = int.TryParse(args[1], out num);
                    if(!isNumeric)
                    {
                        Item item = new Item();
                        string name = args[1].Replace("\"", "");
                        name = name.Replace("_", " ");
                        for(var i = 0; i < ItemLoader.ItemCount; i++)
                        {
                            item.SetDefaults(i, true);
                            if (name != Lang.GetItemNameValue(i).ToLower()) continue;
                            num = i;
                            break;
                        }
                    }
                    type = num;
                    if(args.Count >= 3)
                    {
                        isNumeric = int.TryParse(args[2], out num);
                        if(isNumeric) {
                            quantity = num;
                            for(var i = 0; i < 255; i++)
                            {
                                Player player = Main.player[i];
                                if(player.active)
                                {
                                    player.QuickSpawnItem(type, quantity);
                                }
                            }
                            break;
                        }
                    } else
                    {
                        for(var i = 0; i < 255; i++)
                        {
                            Player player = Main.player[i];
                            if(player.active)
                            {
                                player.QuickSpawnItem(type);
                            }
                        }
                        break;
                    }
                    break;
                case "take":
                    isNumeric = int.TryParse(args[1], out num);
                    if(!isNumeric)
                    {
                        Item item = new Item();
                        string name = args[1].Replace("\"", "");
                        name = name.Replace("_", " ");
                        for (var i = 0; i < ItemLoader.ItemCount; i++)
                        {
                            item.SetDefaults(i, true);
                            if (name != Lang.GetItemNameValue(i).ToLower()) continue;
                            num = i;
                            break;
                        }
                    }
                    type = num;
                    if (args.Count >= 3)
                    {
                        isNumeric = int.TryParse(args[2], out num);
                        if (isNumeric)
                        {
                            quantity = num;
                            for (var i = 0; i < 255; i++)
                            {
                                Player player = Main.player[i];
                                if (player.active)
                                {
                                    RemoveItem(player, type, quantity);
                                }
                            }
                        }
                    }
                    else
                    {
                        for(var i = 0; i < 255; i++)
                        {
                            Player player = Main.player[i];
                            if(player.active)
                            {
                                RemoveItem(player, type);
                            }
                        }
                    }
                    break;
            }
        }

        private static void CallSayCommand(List<string> args)
        {
            args[0] = args[0].Replace("\"", "");
            Main.NewText(args[0]);
        }

        private static void CallKillCommand(List<string> args, CommandTileEntity entity)
        {
            switch (args[0])
            {
                case "player":
                    if (args[1] == "all") { 
                        for (var i = 0; i < 255; i++)
                        {
                            Player player = Main.player[i];
                            if (player.active)
                                player.KillMe(PlayerDeathReason.ByCustomReason("killed by forces unknown!"), 1000, 0, false);
                        }
                    } else
                    {
                        bool isNumeric = double.TryParse(args[1], out double num);
                        if (isNumeric)
                        {
                            num = Math.Abs(num);
                            num++;
                            Vector2 pos = entity.Position.ToVector2();
                            for(var i = 0; i < 255; i++)
                            {
                                Player player = Main.player[i];
                                if(player.active)
                                {
                                    float dist = Vector2.Distance(player.position / 16, pos);
                                    dist = Math.Abs(dist);
                                    if(dist <= num)
                                    {
                                        player.KillMe(PlayerDeathReason.ByCustomReason("killed by forces unknown!"), 1000, 0, false);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "npc":
                    if(args[1] == "all")
                    {
                        for (var i = 0; i < Main.npc.Length; i++)
                        {
                            NPC npc = Main.npc[i];
                            if (npc.active)
                            {
                                npc.life = 0;
                                npc.lifeRegen--;
                            }
                        }
                    }
                    else
                    {
                        bool isNumeric = double.TryParse(args[1], out double num);
                        if (isNumeric)
                        {
                            num = Math.Abs(num);
                            num++;
                            Vector2 pos = entity.Position.ToVector2();
                            for(var i = 0; i < Main.npc.Length; i++)
                            {
                                NPC npc = Main.npc[i];
                                if(npc.active)
                                {
                                    float dist = Vector2.Distance(npc.position / 16, pos);
                                    dist = Math.Abs(dist);
                                    if(dist <= num)
                                    {
                                        npc.life = 0;
                                        npc.lifeRegen--;
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private static void CallEventCommand(List<string> args)
        {
            if(args[0] == "start")
            {
                switch(args[1])
                {
                    case "bloodmoon":
                        if(!Main.bloodMoon)
                        {
                            StartBloodMoon();
                        }
                        break;
                    case "goblin":
                        if (Main.invasionType == 0)
                        {
                            Main.StartInvasion(1);
                        }
                        break;
                    case "legion":
                        if(Main.invasionType == 0)
                        {
                            Main.StartInvasion(2);
                        }
                        break;
                    case "solar":
                        if(!Main.eclipse)
                        {
                            StartEclipse();
                        }
                        break;
                    case "pirate":
                        if (Main.invasionType == 0)
                        {
                            Main.StartInvasion(3);
                        }
                        break;
                    case "pumpkin":
                        if(!Main.pumpkinMoon)
                        {
                            StartPumpkinMoon();
                        }
                        break;
                    case "frost":
                        if(!Main.snowMoon)
                        {
                            StartFrostMoon();
                        }
                        break;
                    case "martian":
                        if(Main.invasionType == 0)
                        {
                            Main.StartInvasion(4);
                        }
                        break;
                }
            }
        }

        private static void CallSummonCommand(List<string> args, CommandTileEntity entity)
        {
            bool isNumeric;

            int type = -1;
            int x;
            int y;
            isNumeric = int.TryParse(args[0], out int num);
            if(isNumeric)
            {
                type = num;
            }
            else
            {
                string name = args[0];
                name = name.Replace("\"", "");
                name = name.Replace(" ", "_");
                name = name.ToLower();
                NPC npc = new NPC();
                for (var i = -65; i < NPCLoader.NPCCount; i++)
                {
                    npc.SetDefaults(i);
                    if (name == Lang.GetNPCNameValue(i).ToLower())
                    {
                        type = i;
                        break;
                    }
                }
            }
            x = int.Parse(args[1]);
            y = int.Parse(args[2]);

            int rx = (entity.Position.X + x) * 16;
            int ry = (entity.Position.Y + y) * 16;

            NPC.NewNPC(rx, ry, type);
        }


        #endregion

        #region EVENTS

        public static void StartBloodMoon()
        {
            Main.moonPhase = 4;
            Main.time = Main.dayTime ? Main.dayLength : 0;
            Main.snowMoon = false;
            Main.pumpkinMoon = false;
            Main.bloodMoon = true;
        }

        public static void StartEclipse()
        {
            Main.snowMoon = false;
            Main.pumpkinMoon = false;
            Main.bloodMoon = false;
            Main.eclipse = true;
        }

        #endregion

        #region EXTENSIONS

        private static List<string> SplitString(string input)
        {
            return Regex.Matches(input, @"[\""].+?[\""]|[^ ]+").Cast<Match>().Select(m => m.Value).ToList();
        }

        private static void RemoveItem(Player player, int type, int quantity = 1)
        {
            int totalToRemove = quantity;
            for(int i = 0; i < player.inventory.Length; i++)
            {
                if (totalToRemove == 0) break;

                Item item = player.inventory[i];
                if(item.type == type)
                {
                    if(item.stack > totalToRemove)
                    {
                        item.stack -= totalToRemove;
                        totalToRemove -= totalToRemove;
                    } else
                    {
                        totalToRemove -= item.stack;
                        item.stack = 0;
                    }
                }
            }
        }

        #endregion

        #region LOGGER
        internal static ILog Logger = LogManager.GetLogger("Khaios");

        public static void Log(object Message, params object[] formatData)
        {
            Logger.Debug("[KhaiosCore] " + string.Format(Message.ToString(), formatData));
        }

        #endregion

    }
}
