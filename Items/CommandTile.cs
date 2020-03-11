using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace KhaiosCore.Items
{
    public class CommandTile : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terminal");
            Tooltip.SetDefault("Allows for commands to be executed through wires.");
        }

        public override void SetDefaults()
        {
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 5;
            item.autoReuse = true;
            item.width = 24;
            item.height = 28;
            item.rare = 1;
            item.value = 20000;
            item.mech = true;
            item.maxStack = 999;
            item.consumable = true;
            item.createTile = mod.TileType(GetType().Name);
            item.tileBoost = 20;
        }
    }
}
