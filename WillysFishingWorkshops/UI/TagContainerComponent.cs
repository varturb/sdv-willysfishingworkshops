using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using WillysFishingWorkshops.Handlers;

namespace WillysFishingWorkshops.UI
{
  public class TagContainerComponent : IClickableMenu
  {
    public string HoverText => hoverText;
    private List<string> tags;
    private readonly List<TagComponent> tagComponents = new();
    private string lastSelectedID = null;
    private bool Visible => delay == 0;
    private readonly int initialY;
    private const int delayVal = 1;
    private int delay = delayVal;
    private string hoverText = string.Empty;
    private readonly Action<string> onTagClick;
    private readonly Action<string> onTagHover;

    public TagContainerComponent(int x, int y, int width, int height, Action<string> onTagClick, Action<string> onTagHover)
      : base(x, y, width, height)
    {
      initialY = y;
      this.onTagClick = onTagClick;
      this.onTagHover = onTagHover;

      UpdateLayout();
    }

    private void UpdateLayout()
    { 
      if (GameStateHandler.SelectedFish == null)
      {
        tagComponents.Clear();
        return;
      }

      if (lastSelectedID == GameStateHandler.SelectedFish.ID)
      {
        return;
      }

      lastSelectedID = GameStateHandler.SelectedFish.ID;
      delay = delayVal;
      tags = GameStateHandler.SelectedFish?.Tags ?? new();

      tagComponents.Clear();
      foreach (var tagText in tags)
      {
        var tag = new TagComponent(tagText, xPositionOnScreen, yPositionOnScreen);
        tagComponents.Add(tag);
      }

      populateClickableComponentList();
    }

    public override void populateClickableComponentList()
    {
      allClickableComponents ??= new();
      allClickableComponents.Clear();
      allClickableComponents.AddRange(tagComponents);
    }

    public override void performHoverAction(int x, int y)
    {
      hoverText = string.Empty;

      foreach (var tag in tagComponents)
      {
        tag.tryHover(x, y);
        if (tag.containsPoint(x, y))
        {
          hoverText = tag.hoverText;
          break;
        }
      }

      onTagHover(hoverText);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      foreach (var tag in tagComponents)
      {
        if (tag.containsPoint(x, y))
        {
          onTagClick(tag.Text);
          return;
        }
      }
    }

    public override void update(GameTime time)
    {
      UpdateLayout();

      var xOffset = (int)Game1.smallFont.MeasureString(I18n.FishMenu_InfoPane_Tags()).X + 16;
      var yOffset = 0;
      var tagHeight = 0;

      foreach (var tag in tagComponents)
      {
        tag.bounds.X = xPositionOnScreen + xOffset;
        tag.bounds.Y = yPositionOnScreen + yOffset;
        if (tag.bounds.Width + xOffset > width)
        {
          xOffset = 0;
          yOffset += tag.bounds.Height + 12;
          tag.bounds.X = xPositionOnScreen + xOffset;
          tag.bounds.Y = yPositionOnScreen + yOffset;
          xOffset += tag.bounds.Width + 12;
          tagHeight = tag.bounds.Height;
        }
        else
        {
          xOffset += tag.bounds.Width + 12;
        }
      }

      height = yOffset + tagHeight;
      yPositionOnScreen = initialY - height - 24;
    }

    public override void draw(SpriteBatch b)
    {
      if (!Visible)
      {
        delay--;
        return;
      }

      Utility.drawTextWithShadow(b, I18n.FishMenu_InfoPane_Tags(), Game1.smallFont, new Vector2(xPositionOnScreen, yPositionOnScreen), Game1.textColor, 1f, -1f, -1, -1, 0.5f);
      foreach (var tag in tagComponents)
      {
        tag.draw(b);
      }
    }
  }
}