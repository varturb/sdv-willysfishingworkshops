using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using WillysFishingWorkshops.Handlers;
using WillysFishingWorkshops.Helpers;
using WillysFishingWorkshops.Models;

namespace WillysFishingWorkshops.UI
{
  public class InfoSection : IClickableMenu
  {
    public const int Region = 2000;

    private static readonly Texture2D spritesheetTexture = ModUtility.Helper.ModContent.Load<Texture2D>("assets/spritesheet.png");
    private TagContainerComponent tagContainer;
    private readonly InfoSectionActions actions;

    public InfoSection(int x, int y, int width, int height, InfoSectionActions actions)
      : base(x, y, width, height)
    {
      this.actions = actions;

      SetupLayout();
    }

    public void ResetLayout()
    {
      SetupLayout();
    }

    private void SetupLayout()
    {
      tagContainer = new TagContainerComponent(
        xPositionOnScreen + 24,
        yPositionOnScreen + height,
        width - 48,
        height / 2,
        actions.OnTagClick,
        actions.OnTagHover
      );

      populateClickableComponentList();
    }

    public override void populateClickableComponentList()
    {
      allClickableComponents ??= new();
      allClickableComponents.Clear();
      if (tagContainer.allClickableComponents == null || GameStateHandler.SelectedFish == null)
      {
        return;
      }
      allClickableComponents
        .AddRange(tagContainer.allClickableComponents
          .Select(x =>
          {
            x.region = Region;
            return x;
          })
      );
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      tagContainer.receiveLeftClick(x, y);
    }

    public override void performHoverAction(int x, int y)
    {
      tagContainer.performHoverAction(x, y);
    }

    public override void update(GameTime time)
    {
      tagContainer.update(time);
      populateClickableComponentList();
    }

    public override void draw(SpriteBatch b)
    {
      if (GameStateHandler.SelectedFish == null)
      {
        return;
      }

      drawTextureBox(
        b,
        spritesheetTexture,
        new Rectangle(0, 64, 18, 18),
        xPositionOnScreen,
        yPositionOnScreen,
        width,
        height,
        Color.White,
        4f
      );

      var font = Game1.smallFont;
      var x = xPositionOnScreen;
      var y = yPositionOnScreen + 48;
      var fish = GameStateHandler.SelectedFish;
      var scale = 1.2f;
      var textWidth = font.MeasureString(fish.Name).X * scale;

      // fish sprite
      fish.Item.drawInMenu(b, new(x + width / 2f - 32, y), 1.2f, 1f, 0.9f);
      // fish name
      Utility.drawTextWithShadow(b, fish.Name, font, new(x + width / 2f - textWidth / 2f, y + 72 + 12), Game1.textColor, scale, -1f, -1, -1, 0.5f);
      scale = 1f;

      var maxWidth = width - 32;
      x += 24;
      y += 144;
      var parsedDescription = GameHelper.ParseString(fish.Description, maxWidth, font);
      // fish description
      Utility.drawTextWithShadow(b, parsedDescription, font, new Vector2(x, y), Game1.textColor, scale, -1f, -1, -1, 0.5f);
      var descriptionHeight = font.MeasureString(parsedDescription).Y;
      y += (int)descriptionHeight + 24;

      // fish difficulty
      var difficultyLevelText = I18n.FishMenu_InfoPane_Difficulty(fish.DifficultyLevel);
      Utility.drawTextWithShadow(b, difficultyLevelText, font, new(x, y), Game1.textColor, scale, -1f, -1, -1, 0.5f);

      var xOffset = xPositionOnScreen + width - 24;
      var yOffset = y + 4;
      var gap = 4;
      var starWidth = 24;
      var i = 4;
      b.Draw(spritesheetTexture, new Rectangle(xOffset - (i * starWidth) - (i-- * gap), yOffset, 24, 24), new(18, 64, 8, 8), Color.White);
      b.Draw(spritesheetTexture, new Rectangle(xOffset - (i * starWidth) - (i-- * gap), yOffset, 24, 24), new(18, 64, 8, 8), Color.White);
      b.Draw(spritesheetTexture, new Rectangle(xOffset - (i * starWidth) - (i-- * gap), yOffset, 24, 24), new(18, 64, 8, 8), Color.White);
      b.Draw(spritesheetTexture, new Rectangle(xOffset - (i * starWidth) - (i-- * gap), yOffset, 24, 24), new(18, 64, 8, 8), Color.White);

      i = 4;
      b.Draw(spritesheetTexture, new Rectangle(xOffset - (i * starWidth) - (i-- * gap), yOffset, 24, 24), fish.SourceRect, Color.White);

      if (fish.Difficulty >= FishDifficulty.Medium)
      {
        b.Draw(spritesheetTexture, new Rectangle(xOffset - (i * starWidth) - (i-- * gap), yOffset, 24, 24), fish.SourceRect, Color.White);
      }
      if (fish.Difficulty >= FishDifficulty.Hard)
      {
        b.Draw(spritesheetTexture, new Rectangle(xOffset - (i * starWidth) - (i-- * gap), yOffset, 24, 24), fish.SourceRect, Color.White);
      }
      if (fish.Difficulty >= FishDifficulty.ExtremelyHard)
      {
        b.Draw(spritesheetTexture, new Rectangle(xOffset - (i * starWidth) - (i-- * gap), yOffset, 24, 24), fish.SourceRect, Color.White);
      }

      // fish tags
      tagContainer.draw(b);
    }
  }

  public struct InfoSectionActions
  {
    public Action<string> OnTagClick { get; set; }
    public Action<string> OnTagHover { get; set; }
  }
}