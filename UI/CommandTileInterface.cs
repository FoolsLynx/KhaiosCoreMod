using KhaiosCore.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace KhaiosCore.UI
{
    public class CommandTileInterface : UIState
    {
        #region UI ELEMENTS

        private UIPanel mainPanel;

        private UIText commandText;
        private UITextField commandTextBox;
        private UIText commandTextError;
        private UIImageButton[] commandButtons;

        #endregion

        #region POSITION VARIABLES

        private float left;
        private float top;
        private float width;
        private float height;

        private float yOffset = 0;
        private float indent = 0;

        #endregion

        #region COMMAND TILE VARIABLES

        private readonly CommandTile tile;
        private CommandTileEntity tileEntity;
        private readonly KhaiosPlayer caller;

        private string commandEntered;
        private string commandError;

        #endregion

        public CommandTileInterface(CommandTile tile, CommandTileEntity tileEntity, KhaiosPlayer caller)
        {
            this.tile = tile;
            this.tileEntity = tileEntity;
            this.caller = caller;
        }

        public override void OnInitialize()
        {
            // Setup Position Data
            width = 620;
            height = 128;
            left = (Main.screenWidth / 2) - (width / 2);
            top = (Main.screenHeight / 4) - (height / 2);
            indent = 8;

            // Main Panel
            mainPanel = new UIPanel();
            mainPanel.SetPadding(indent);
            mainPanel.Left.Set(left, 0f);
            mainPanel.Top.Set(top, 0f);
            mainPanel.Width.Set(width, 0f);
            mainPanel.Height.Set(height, 0f);
            mainPanel.BackgroundColor = new Color(73, 94, 171);

            // Command Text
            commandText = new UIText("Command: ", 0.85f);
            commandText.Left.Pixels = indent;
            commandText.Top.Pixels = 4;
            mainPanel.Append(commandText);
            yOffset += 24;

            // Command Text Field
            commandTextBox = new UITextField("", 0.9f, false);
            commandTextBox.Left.Pixels = indent;
            commandTextBox.Top.Pixels = yOffset;
            commandTextBox.Width.Pixels = width - 32;
            if(!string.IsNullOrEmpty(tileEntity.commandTileCommand))
                commandTextBox.SetText(tileEntity.commandTileCommand);
            commandTextBox.OnTextChanged += () =>
            {
                CommandTextBox_OnTextChange();
            };
            mainPanel.Append(commandTextBox);
            yOffset += 32;

            // Command Text Error
            commandTextError = new UIText("", 1f, false);
            commandTextError.Left.Pixels = indent;
            commandTextError.Top.Pixels = yOffset;
            if (!string.IsNullOrEmpty(tileEntity.commandTileError))
                commandTextError.SetText(tileEntity.commandTileError);
            mainPanel.Append(commandTextError);
            yOffset += 28;

            // Icons
            Texture2D[] iconTextures = new Texture2D[]
            {
                Khaios.instance.GetTexture("UI/Icons/UIButtonConfirm"),
                Khaios.instance.GetTexture("UI/Icons/UIButtonExecute"),
                Khaios.instance.GetTexture("UI/Icons/UIButtonTest"),
                Khaios.instance.GetTexture("UI/Icons/UIButtonCancel")
            };
            commandButtons = new UIImageButton[4];
            for(int i = 0; i < commandButtons.Length; i++)
            {
                commandButtons[i] = new UIImageButton(iconTextures[i]);
                commandButtons[i].Left.Pixels = indent;
                commandButtons[i].Top.Pixels = yOffset;
                commandButtons[i].Width.Pixels = 96;
                commandButtons[i].Height.Pixels = 32;
                switch(i)
                {
                    case 0: commandButtons[i].OnClick += new MouseEvent(CommandButtons_ConfirmButtonClicked); break;
                    case 1: commandButtons[i].OnClick += new MouseEvent(CommandButtons_ConfirmExecuteButtonClicked); break;
                    case 2: commandButtons[i].OnClick += new MouseEvent(CommandButtons_TestButtonClicked); break;
                    case 3: commandButtons[i].OnClick += new MouseEvent(CommandButtons_CancelButtonClicked); break;
                }
                mainPanel.Append(commandButtons[i]);
                indent += 104;
            }
            Append(mainPanel);
        }

        private void CommandTextBox_OnTextChange()
        {
            commandEntered = commandTextBox.Text;
        }

        private void CommandButtons_ConfirmButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            if(tile == null || tileEntity == null)
            {
                return;
            }
            tileEntity.commandTileCommand = commandEntered;
            caller.CloseCommand();
        }

        private void CommandButtons_ConfirmExecuteButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            if (tile == null || tileEntity == null)
            {
                return;
            }
            tileEntity.commandTileCommand = commandEntered;
            if (!KhaiosUtil.TestCommand(commandEntered, ref commandError))
            {
                commandTextError.SetText(commandError);
                return;
            }
            KhaiosUtil.ExecuteCommand(commandEntered, ref tileEntity);
            caller.CloseCommand();
        }

        private void CommandButtons_TestButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            if(!KhaiosUtil.TestCommand(commandEntered, ref commandError))
            {
                commandTextError.SetText(commandError);
            } else
            {
                commandTextError.SetText("Success!");
            }
        }

        private void CommandButtons_CancelButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            caller.CloseCommand();
        }

    }
}
