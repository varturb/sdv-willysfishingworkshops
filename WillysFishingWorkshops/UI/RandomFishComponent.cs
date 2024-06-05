using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using WillysFishingWorkshops.Handlers;
using WillysFishingWorkshops.Helpers;
using WillysFishingWorkshops.Models;

namespace WillysFishingWorkshops.UI
{
  public class RandomFishComponent : ClickableTextureComponent
  {
    private readonly static Texture2D spritesheetTexture = ModUtility.Helper.ModContent.Load<Texture2D>("assets/spritesheet.png");
    private bool hovered = false;
    private static bool Selected => GameStateHandler.SelectedFish == null;

    public RandomFishComponent(int x, int y, int width = 128, int height = 128)
        : base(new(x, y, width, height), spritesheetTexture, new(36, 114, 18, 18), 4f, false)
    {
    }

    public void Hover(bool hovered)
    {
      if (visible)
      {
        this.hovered = hovered;
      }
    }

    public static Rectangle DrawItem(SpriteBatch b, int x, int y, int width = 128, int height = 128, RandomFishComponent rfc = null)
    {
      rfc ??= new RandomFishComponent(x, y, width);
      b.Draw(
        spritesheetTexture,
        new Rectangle(
          (int)(rfc.bounds.X + rfc.bounds.Width / 2f - width / 4f),
          (int)(rfc.bounds.Y + rfc.bounds.Height / 2f - height / 4f),
          (int)(rfc.sourceRect.Width * rfc.scale),
          (int)(rfc.sourceRect.Height * rfc.scale)
        ),
        rfc.sourceRect,
        Color.White
      );
      return rfc.bounds;
    }

    public override void draw(SpriteBatch b)
    {
      if (!visible)
      {
        return;
      }

      var drawMode = Selected ? DrawBoxMode.Selected : hovered ? DrawBoxMode.Hovered : DrawBoxMode.Default;
      GameHelper.DrawBox(b, texture, bounds, drawMode);
      DrawItem(b, bounds.X, bounds.Y, rfc: this);
    }
  }
}

