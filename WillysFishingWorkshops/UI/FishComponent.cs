using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using WillysFishingWorkshops.Handlers;
using WillysFishingWorkshops.Helpers;
using WillysFishingWorkshops.Models;

namespace WillysFishingWorkshops.UI
{
  public class FishComponent : ClickableTextureComponent
  {
    private readonly static Texture2D spritesheetTexture = ModUtility.Helper.ModContent.Load<Texture2D>("assets/spritesheet.png");
    private bool hovered = false;
    private bool Selected => GameStateHandler.SelectedFish?.ID == item.ItemId;
    private readonly Fish fish;

    public FishComponent(Fish fish, int x, int y, int width = 128, int height = 128)
        : base(new(x, y, width, height), spritesheetTexture, default, 4f, false)
    {
      this.fish = fish;
      item = fish?.Item;
    }


    public void Hover(bool hovered)
    {
      if (visible)
      {
        this.hovered = hovered;
      }
    }

    public static Rectangle DrawItem(SpriteBatch b, Fish fish, int x, int y, int width = 128, int height = 128)
    {
      var fc = new FishComponent(fish, x, y, width, height);
      fc.drawItem(b, fc.bounds.Width / 2 - width / 4, (int)(height / 16f * 3));
      return fc.bounds;
    }

    public override void draw(SpriteBatch b)
    {
      if (!visible)
      {
        return;
      }

      var drawMode = Selected ? DrawBoxMode.Selected : hovered ? DrawBoxMode.Hovered : DrawBoxMode.Default;
      GameHelper.DrawBox(b, texture, bounds, drawMode);
      base.drawItem(b, bounds.Width / 2 - 32, 24);
      b.Draw(
        texture,
        new(bounds.X + bounds.Width - 40, bounds.Y + bounds.Height - 40),
        fish.SourceRect,
        Color.White,
        0f,
        default,
        3f,
        SpriteEffects.None,
        0.8f
      );
    }
  }
}

