using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace KhaiosCore
{
    public class Khaios : Mod
    {
        public static Khaios instance;
        public UserInterface KhaiosInterface;

        public override void Load()
        {
            if (ModLoader.version < new Version(0, 11, 6, 1))
            {
                KhaiosUtil.Log("Out of data tModLoader. Version {0} recommended!", (new Version(0, 11, 6, 1).ToString()));
            }
            try
            {
                instance = this;
                Init();
            } catch(Exception e)
            {
                KhaiosUtil.Log("Load:\n " + e.Message + "\n" + e.StackTrace + "\n");
            }
        }

        public override void Unload()
        {
            instance = null;
            KhaiosInterface = null;
        }

        public void Init()
        {
            KhaiosUtil.Init();
            if(!Main.dedServ)
            {
                KhaiosInterface = new UserInterface();
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if(inventoryIndex != -1)
            {
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer(
                        "Khaios: Commands",
                        delegate
                        {
                            KhaiosInterface.Draw(Main.spriteBatch, new GameTime());
                            return true;
                        },
                        InterfaceScaleType.UI
                    )
                );
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            KhaiosInterface.Update(gameTime);
        }
    }
}
