using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
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
    internal class UITextField : UITextPanel<string>
    {
        internal bool focused = false;
        private int _cursor;
        private int _frameCount;
        private int _maxLength = 60;
        private readonly string hintText;
        public event Action OnFocus;
        public event Action OnUnfocus;
        public event Action OnTextChanged;
        public event Action OnTabPressed;
        public event Action OnEnterPressed;
        public event Action OnUpPressed;
        internal bool unfocusOnEnter = true;
        internal bool unfocusOnTab = true;

        public UITextField(string text, float textScale = 1, bool large = false) : base("", textScale, large)
        {
            hintText = text;
            SetPadding(4);
        }

        public override void Click(UIMouseEvent evt)
        {
            Focus();
            base.Click(evt);
        }
        public void Focus()
        {
            if (!focused)
            {
                Main.clrInput();
                focused = true;
                OnFocus?.Invoke();
            }
        }

        public void Unfocus()
        {
            if (focused)
            {
                focused = false;
                OnUnfocus?.Invoke();
            }
        }

        public void SetUnfocusKey(bool unfocusOnEnter, bool unfocusOnTab)
        {
            this.unfocusOnEnter = unfocusOnEnter;
            this.unfocusOnTab = unfocusOnTab;
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
            if (!ContainsPoint(MousePosition) && Main.mouseLeft)
            {
                Unfocus();
            }
            base.Update(gameTime);
        }

        public void Write(string text)
        {
            base.SetText(base.Text.Insert(this._cursor, text));
            this._cursor += text.Length;
            _cursor = Math.Min(Text.Length, _cursor);
            Recalculate();

            OnTextChanged?.Invoke();
        }

        public void WriteAll(string text)
        {
            bool changed = text != Text;
            base.SetText(text);
            this._cursor = text.Length;
            Recalculate();

            if (changed)
            {
                OnTextChanged?.Invoke();
            }
        }

        public override void SetText(string text, float textScale, bool large)
        {
            if (text.ToString().Length > this._maxLength)
            {
                text = text.ToString().Substring(0, this._maxLength);
            }
            base.SetText(text, textScale, large);

            this._cursor = Math.Min(base.Text.Length, this._cursor);

            OnTextChanged?.Invoke();
        }

        public void SetTextMaxLength(int maxLength)
        {
            this._maxLength = maxLength;
        }

        public void Backspace()
        {
            if (this._cursor == 0)
            {
                return;
            }
            base.SetText(base.Text.Substring(0, base.Text.Length - 1));
            Recalculate();
        }

        public void CursorLeft()
        {
            if (this._cursor == 0)
            {
                return;
            }
            this._cursor--;
        }

        public void CursorRight()
        {
            if (this._cursor < base.Text.Length)
            {
                this._cursor++;
            }
        }

        static bool JustPressed(Keys key)
        {
            return Main.inputText.IsKeyDown(key) && !Main.oldInputText.IsKeyDown(key);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle hitbox = GetDimensions().ToRectangle();
            Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.White);


            if (focused)
            {
                Terraria.GameInput.PlayerInput.WritingText = true;
                Main.instance.HandleIME();
                WriteAll(Main.GetInputText(Text));

                if (JustPressed(Keys.Tab))
                {
                    if (unfocusOnTab) Unfocus();
                    OnTabPressed?.Invoke();
                }

                if (JustPressed(Keys.Enter))
                {
                    if (unfocusOnEnter) Unfocus();
                    OnEnterPressed?.Invoke();
                }
                if (JustPressed(Keys.Up))
                {
                    OnUpPressed?.Invoke();
                }

            }
            CalculatedStyle innerDimensions2 = base.GetInnerDimensions();
            Vector2 pos2 = innerDimensions2.Position();
            if (IsLarge)
            {
                pos2.Y -= 10f * TextScale * TextScale;
            }
            else
            {
                pos2.Y -= 2f * TextScale;
            }
            if (IsLarge)
            {
                Utils.DrawBorderStringBig(spriteBatch, Text, pos2, TextColor, TextScale, 0f, 0f, -1);
                return;
            }
            Utils.DrawBorderString(spriteBatch, Text, pos2, TextColor, TextScale, 0f, 0f, -1);

            this._frameCount++;

            CalculatedStyle innerDimensions = base.GetInnerDimensions();
            Vector2 pos = innerDimensions.Position();
            DynamicSpriteFont spriteFont = base.IsLarge ? Main.fontDeathText : Main.fontMouseText;
            Vector2 vector = new Vector2(spriteFont.MeasureString(base.Text.Substring(0, this._cursor)).X, base.IsLarge ? 32f : 16f) * base.TextScale;
            if (base.IsLarge)
            {
                pos.Y -= 8f * base.TextScale;
            }
            else
            {
                pos.Y -= 1f * base.TextScale;
            }
            if (Text.Length == 0)
            {
                pos.X += 5;
                if (base.IsLarge)
                {
                    Utils.DrawBorderStringBig(spriteBatch, hintText, pos, Color.Gray, base.TextScale, 0f, 0f, -1);
                    return;
                }
                Utils.DrawBorderString(spriteBatch, hintText, pos, Color.Gray, base.TextScale, 0f, 0f, -1);
                pos.X -= 5;
            }

            if (!focused) return;

            pos.X += /*(innerDimensions.Width - base.TextSize.X) * 0.5f*/ +vector.X - (base.IsLarge ? 8f : 4f) * base.TextScale + 6f;
            if ((this._frameCount %= 40) > 20)
            {
                return;
            }
            if (base.IsLarge)
            {
                Utils.DrawBorderStringBig(spriteBatch, "|", pos, base.TextColor, base.TextScale, 0f, 0f, -1);
                return;
            }
            Utils.DrawBorderString(spriteBatch, "|", pos, base.TextColor, base.TextScale, 0f, 0f, -1);
        }
    }
}
