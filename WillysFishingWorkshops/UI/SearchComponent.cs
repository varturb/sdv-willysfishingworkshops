using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using WillysFishingWorkshops.Handlers;

namespace WillysFishingWorkshops.UI
{
  public class SearchComponent : IClickableMenu
  {
    public bool Selected => textBox.Selected;

    private SearchTextbox textBox;
    private ClickableComponent textBoxCC;
    private TextBoxEvent e;

    public SearchComponent(int x, int y, int width)
    {
      xPositionOnScreen = x;
      yPositionOnScreen = y;
      this.width = width;
      height = 192;

      CreateComponents();

      GameStateHandler.FilterValueUpdated += delegate
      {
        textBox.Text = GameStateHandler.FilterValue;
      };
    }

    private void CreateComponents()
    {
      textBox = new SearchTextbox();
      textBox.X = xPositionOnScreen;
      textBox.Y = yPositionOnScreen;
      textBox.Width = width;
      textBox.Height = height;
      e = TextBoxEnter;
      textBox.OnTextChanged += e;
      textBox.Text = GameStateHandler.FilterValue;
      Game1.keyboardDispatcher.Subscriber = textBox;
      textBox.Selected = false;

      textBoxCC = new ClickableComponent(
        new Rectangle(textBox.X, textBox.Y, textBox.Width, textBox.Height / 3),
        string.Empty
      );

      populateClickableComponentList();
    }

    public void UpdateText(string text)
    {
      textBox.Text = text;
      GameStateHandler.PerformSearch(textBox.Text);
    }

    private void TextBoxEnter(TextBox sender)
    {
      if (sender.Text.Length >= 0)
      {
        GameStateHandler.PerformSearch(textBox.Text);
      }
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      CreateComponents();
    }

    public override void populateClickableComponentList()
    {
      allClickableComponents ??= new();
      allClickableComponents.Clear();
      allClickableComponents.Add(textBoxCC);
    }

    public override void receiveKeyPress(Keys key)
    {
      if (textBox.Selected && !Game1.options.doesInputListContain(Game1.options.menuButton, key))
      {
        base.receiveKeyPress(key);
      }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      base.receiveLeftClick(x, y, playSound);
      textBox.Update();
    }

    public override void receiveGamePadButton(Buttons b)
    {
      base.receiveGamePadButton(b);
      if (textBox.Selected)
      {
        switch (b)
        {
          case Buttons.DPadUp:
          case Buttons.DPadDown:
          case Buttons.DPadLeft:
          case Buttons.DPadRight:
          case Buttons.LeftThumbstickLeft:
          case Buttons.LeftThumbstickUp:
          case Buttons.LeftThumbstickDown:
          case Buttons.LeftThumbstickRight:
            textBox.Selected = false;
            break;
        }
      }
    }

    public override void draw(SpriteBatch b)
    {
      base.draw(b);

      textBox.Draw(b);

      drawMouse(b);
    }
  }
}