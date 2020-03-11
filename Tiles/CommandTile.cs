using KhaiosCore.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace KhaiosCore.Tiles
{
    public class CommandTile : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<CommandTileEntity>().Hook_AfterPlacement, -1, 0, true);
            TileObjectData.newTile.AnchorBottom = new AnchorData(Terraria.Enums.AnchorType.None, 0, 0);
            TileObjectData.addTile(Type);

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Terminal");
            AddMapEntry(Color.LightGray, name);

            disableSmartCursor = true;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            ModContent.GetInstance<CommandTileEntity>().Kill(i, j);
            base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.showItemIcon = true;
            player.showItemIcon2 = mod.ItemType(GetType().Name);
        }
        public override bool NewRightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            KhaiosPlayer modPlayer = player.GetModPlayer<KhaiosPlayer>();
            int entity = ModContent.GetInstance<CommandTileEntity>().Find(i, j);
            if (entity == -1) return false;
            CommandTileEntity tileEntity = (CommandTileEntity)TileEntity.ByID[entity];
            modPlayer.OpenCommand();
            if (PlayerInput.GrappleAndInteractAreShared)
                PlayerInput.Triggers.JustPressed.Grapple = false;
            Main.PlaySound(12, -1, -1, 1);
            Khaios.instance.KhaiosInterface.SetState(new CommandTileInterface(this, tileEntity, modPlayer));
            return true;
        }

        public override void HitWire(int i, int j)
        {
            int entity = ModContent.GetInstance<CommandTileEntity>().Find(i, j);
            if (entity == -1) return;
            CommandTileEntity tileEntity = (CommandTileEntity)TileEntity.ByID[entity];
            if(Wiring.CheckMech(i, j, 20))
                KhaiosUtil.ExecuteCommand(tileEntity.commandTileCommand, ref tileEntity);
        }
    }

    public class CommandTileEntity : ModTileEntity
    {
        internal string commandTileCommand;
        internal string commandTileError;

        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.type == mod.TileType("CommandTile");
        }

        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            commandTileCommand = reader.ReadString();
            commandTileError = reader.ReadString();
        }

        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            writer.Write(commandTileCommand);
            writer.Write(commandTileError);
        }

        public override TagCompound Save()
        {
            return new TagCompound
            {
                { "commandTileCommand", commandTileCommand },
                { "commandTileError", commandTileError }
            };
        }

        public override void Load(TagCompound tag)
        {
            commandTileCommand = tag.GetString("commandTileCommand");
            commandTileError = tag.GetString("commandTileError");
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            if(Main.netMode == 1)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 3);
                NetMessage.SendData(87, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
        }
    }
}
