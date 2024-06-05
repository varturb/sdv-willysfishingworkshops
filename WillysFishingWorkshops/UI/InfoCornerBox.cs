using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using WillysFishingWorkshops.Handlers;
using WillysFishingWorkshops.Helpers;
using WillysFishingWorkshops.Models;

namespace WillysFishingWorkshops.UI
{
  public class InfoCornerBox : IClickableMenu
  {
    private static readonly Texture2D texture = ModUtility.Helper.ModContent.Load<Texture2D>("assets/spritesheet.png");

    private readonly string textLine1;
    private readonly string textLine2;
    private readonly bool isRandomFish;

    public InfoCornerBox(string textLine1, string textLine2, int x, int y, bool isRandomFish)
    {
      xPositionOnScreen = x;
      yPositionOnScreen = y;
      var boundsLine1 = Game1.dialogueFont.MeasureString(textLine1);
      var boundsLine2 = Game1.smallFont.MeasureString(textLine2);
      width = (int)Math.Max(boundsLine1.X, boundsLine2.X) + 48;
      height = (int)(boundsLine1.Y + boundsLine2.Y + 28);

      this.textLine1 = textLine1;
      this.textLine2 = textLine2;
      this.isRandomFish = isRandomFish;
    }

    public override void draw(SpriteBatch b)
    {
      var x = xPositionOnScreen;
      var y = yPositionOnScreen;
      var boundsLine1 = Game1.dialogueFont.MeasureString(textLine1);
      var bounds = new Rectangle(x, y, width, height);
      var fishBounds = new Rectangle(x, y, 136, 136);
      bounds.X += fishBounds.Width - 4;
      GameHelper.DrawBox(b, texture, fishBounds);
      GameHelper.DrawBox(b, texture, fishBounds);

      if (isRandomFish)
      {
        RandomFishComponent.DrawItem(b, x, y, fishBounds.Width, fishBounds.Height);
      }
      else
      {
        var fish = GameStateHandler.LastCreatedFish ?? GameStateHandler.SelectedFish;
        if (fish != null)
        {
          FishComponent.DrawItem(b, fish, x, y, fishBounds.Width, fishBounds.Height);

          var xOffset = fishBounds.X + fishBounds.Width / 2 - 54;
          var yOffset = fishBounds.Y + fishBounds.Height - 42;
          var gap = 4;
          var starWidth = 24;
          var i = 0;
          b.Draw(texture, new Rectangle(xOffset + (i * starWidth) + (i++ * gap), yOffset, 24, 24), new(18, 64, 8, 8), Color.White);
          b.Draw(texture, new Rectangle(xOffset + (i * starWidth) + (i++ * gap), yOffset, 24, 24), new(18, 64, 8, 8), Color.White);
          b.Draw(texture, new Rectangle(xOffset + (i * starWidth) + (i++ * gap), yOffset, 24, 24), new(18, 64, 8, 8), Color.White);
          b.Draw(texture, new Rectangle(xOffset + (i * starWidth) + (i++ * gap), yOffset, 24, 24), new(18, 64, 8, 8), Color.White);

          i = 0;
          b.Draw(texture, new Rectangle(xOffset + (i * starWidth) + (i++ * gap), yOffset, 24, 24), fish.SourceRect, Color.White);

          if (fish.Difficulty >= FishDifficulty.Medium)
          {
            b.Draw(texture, new Rectangle(xOffset + (i * starWidth) + (i++ * gap), yOffset, 24, 24), fish.SourceRect, Color.White);
          }
          if (fish.Difficulty >= FishDifficulty.Hard)
          {
            b.Draw(texture, new Rectangle(xOffset + (i * starWidth) + (i++ * gap), yOffset, 24, 24), fish.SourceRect, Color.White);
          }
          if (fish.Difficulty >= FishDifficulty.ExtremelyHard)
          {
            b.Draw(texture, new Rectangle(xOffset + (i * starWidth) + (i++ * gap), yOffset, 24, 24), fish.SourceRect, Color.White);
          }
        }
      }

      GameHelper.DrawBox(b, texture, bounds);
      var textOffsetX = bounds.X + 16;
      var textOffsetY = bounds.Y + 16;
      var text1Pos = new Vector2(textOffsetX, textOffsetY);
      Utility.drawTextWithShadow(b, textLine1, Game1.dialogueFont, text1Pos, Game1.textColor);
      Utility.drawTextWithShadow(b, textLine2, Game1.smallFont, new(text1Pos.X, text1Pos.Y + boundsLine1.Y), Game1.textColor);
    }
  }
}