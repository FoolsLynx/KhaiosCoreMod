using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace KhaiosCore
{
    public class KhaiosPlayer : ModPlayer
    {
        public Point16 commandPosition;

        public bool playerCommand = false;
        public bool playerBoss = false;

        public override void UpdateDead()
        {
            if(player.whoAmI == Main.myPlayer)
            {
                CloseCommand();
            }
        }

        public override void PreUpdate()
        {
            if (playerCommand || playerBoss)
            {
                if(player.position.ToPoint16() != commandPosition)
                {
                    CloseCommand();
                }
            }
        }

        public void OpenCommand()
        {
            commandPosition = player.position.ToPoint16();
            playerCommand = true;
            Main.blockInput = true;
        }

        public void CloseCommand()
        {
            commandPosition = new Point16(-1, -1);
            playerCommand = false;
            Main.blockInput = false;
            Khaios.instance.KhaiosInterface.SetState(null);
        }

        public Point16 ViewingCommand()
        {
            return commandPosition;
        }
    }
}
