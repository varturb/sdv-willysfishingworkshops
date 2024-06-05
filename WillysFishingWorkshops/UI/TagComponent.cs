using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using WillysFishingWorkshops.Helpers;

namespace WillysFishingWorkshops.UI
{
  public class TagComponent : ClickableTextureComponent
  {
    public string Text => text;

    private readonly static Texture2D spritesheetTexture = ModUtility.Helper.ModContent.Load<Texture2D>("assets/spritesheet.png");
    private readonly string text;
    private readonly int textLength;
    private readonly Color color;
    
    public TagComponent(string text, int x, int y)
       : base(new(x, y, 0, 0), spritesheetTexture, new(18, 72, 12, 9), 4f, false)
    {
      this.text = text;
      hoverText = TagColorHelper.GetHoverTextForTag(text);
      color = TagColorHelper.GetColorForTag(text);

      var textDimensions = Game1.smallFont.MeasureString(text);
      textLength = (int)textDimensions.X;
      bounds.Width = (int)textDimensions.X + 24;
      bounds.Height = (int)textDimensions.Y;
    }

    public override void draw(SpriteBatch b)
    {
      var partSize = sourceRect.Width / 3;
      //start
      b.Draw(
        texture,
        new Rectangle(bounds.X, bounds.Y, (int)(partSize * scale), bounds.Height),
        new(sourceRect.X, sourceRect.Y, partSize, sourceRect.Height),
        color
      );
      //mid
      b.Draw(
        texture,
        new Rectangle(bounds.X + (int)(partSize * scale), bounds.Y, bounds.Width - (int)(partSize * scale * 2f), bounds.Height),
        new(sourceRect.X + partSize, sourceRect.Y, partSize, sourceRect.Height),
        color
      );
      //end
      b.Draw(
        texture,
        new Rectangle(bounds.X + bounds.Width - (int)(partSize * scale), bounds.Y, (int)(partSize * scale), bounds.Height),
        new(sourceRect.X + partSize * 2, sourceRect.Y, partSize, sourceRect.Height),
        color
      );

      var textColor = TagColorHelper.GetTextColorForTag(color);
      Utility.drawTextWithShadow(b, text, Game1.smallFont, new(bounds.X + bounds.Width / 2f - textLength / 2f, bounds.Y), textColor, 1f);
    }
  }
}
