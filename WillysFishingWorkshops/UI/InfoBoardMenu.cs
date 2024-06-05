using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using WillysFishingWorkshops.Helpers;

namespace WillysFishingWorkshops.UI
{
  public class InfoBoardMenu : IClickableMenu
  {
    private readonly Texture2D billboardTexture;

    public InfoBoardMenu()
    {
      width = 1024;
      height = 792;

      billboardTexture = ModUtility.Helper.ModContent.Load<Texture2D>("assets/spritesheet.png");

      ResetLayout();
    }

    private void ResetLayout()
    {
      var topLeft = Utility.getTopLeftPositionForCenteringOnScreen(width, height);
      xPositionOnScreen = (int)topLeft.X;
      yPositionOnScreen = (int)topLeft.Y;

      CreateComponents();
    }

    private void CreateComponents()
    {
      initializeUpperRightCloseButton();
      upperRightCloseButton.myID = upperRightCloseButton_ID;

      populateClickableComponentList();
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      ResetLayout();
    }

    public override void populateClickableComponentList()
    {
      allClickableComponents ??= new();
      allClickableComponents.Clear();
      allClickableComponents.Add(upperRightCloseButton);
    }

    public override void snapToDefaultClickableComponent()
    {
      currentlySnappedComponent = upperRightCloseButton;
      base.snapCursorToCurrentSnappedComponent();
    }

    public override void draw(SpriteBatch b)
    {
      GameHelper.DrawFadedBackground(b, 0.5f);
      b.Draw(
        billboardTexture,
        new Vector2(xPositionOnScreen, yPositionOnScreen),
        new Rectangle(64, 0, 256, 198),
        Color.White,
        0f,
        Vector2.Zero,
        4f,
        SpriteEffects.None,
        1f
      );

      var font = Game1.dialogueFont;
      var color = Game1.textColor;
      var shadow = 0.5f;
      var y = yPositionOnScreen + 128;
      var x = xPositionOnScreen + 96;
      var maxWidth = 832;
      var maxHeight = 500;

      Utility.drawTextWithShadow(
        b,
        I18n.WorkshopInfoBoard_Title(),
        font,
        new Vector2((float)(xPositionOnScreen + width / 2f) - font.MeasureString(I18n.WorkshopInfoBoard_Title()).X / 2f, y),
        color,
        1f,
        -1f,
        -1,
        -1,
        shadow
      );

      var description = I18n.WorkshopInfoBoard_Description();
      var parsedDescription = GameHelper.ParseString(I18n.WorkshopInfoBoard_Description(), maxWidth, font);
      var height = font.MeasureString(parsedDescription).Y;
      var scale = 1f;
      while (height * scale > maxHeight && scale > 0.25f)
      {
        scale -= 0.05f;
        parsedDescription = GameHelper.ParseString(description, (int)(maxWidth / scale), font);
        height = font.MeasureString(parsedDescription).Y;
      }
      Utility.drawTextWithShadow(b, parsedDescription, font, new Vector2(x, y + 64), color, scale, -1f, -1, -1, shadow);

      base.draw(b);
      drawMouse(b);
    }
  }
}