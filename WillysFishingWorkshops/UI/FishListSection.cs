using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using WillysFishingWorkshops.Models;

namespace WillysFishingWorkshops.UI
{
  public class FishListSection : IClickableMenu
  {
    public const int Region = 1000;

    private static readonly Texture2D spritesheetTexture = ModUtility.Helper.ModContent.Load<Texture2D>("assets/spritesheet.png");
    private readonly FishListSectionActions actions;
    private ClickableTextureComponent scrollUpButton;
    private ClickableTextureComponent scrollDownButton;
    private ClickableTextureComponent scrollBar;
    private Rectangle scrollBarRunner;
    private List<Fish> fishList;
    private readonly List<FishComponent> fishComponents = new();
    private RandomFishComponent randomFishComponent;
    private const int pageSize = 4;
    private const int rowSize = 6;
    private int topRowIndex = 0;
    private int rowsCount = 0;
    private int selectedIndex = -1;
    private bool scrolling;

    public FishListSection(int x, int y, int width, int height, FishListSectionActions actions)
      : base(x, y, width, height)
    {
      this.actions = actions;

      SetupLayout();
    }

    public void ResetLayout(List<Fish> fishList, Fish selectedFish)
    {
      this.fishList = fishList;
      CreateComponents();
      ResetScrollPosition(selectedFish);
      populateClickableComponentList();
    }

    public void ResetScrollPosition(Fish selectedFish)
    {
      selectedIndex = selectedFish != null
        ? fishList.FindIndex(x => x.ID == selectedFish.ID)
        : 0;
      SetScrollBarToCurrentIndex();
    }

    public void  AdjustScrollPosition(Direction direction, ClickableComponent snappedComponent)
    {
      if (!Game1.options.SnappyMenus || snappedComponent == null || direction == Direction.Left || direction == Direction.Right)
      {
        return;
      }

      var index = allClickableComponents.FindIndex(e => e.myID == snappedComponent.myID);
      if (index < 0)
      {
        return;
      }
      var rowIndex = index / rowSize;
      if (rowIndex < topRowIndex)
      {
        topRowIndex = rowIndex;
      }
      else if (rowIndex >= topRowIndex + pageSize)
      {
        topRowIndex = rowIndex - pageSize + 1;
      }

      selectedIndex = index;  
      SetScrollBarToCurrentIndex();
    }

    private void SetupLayout()
    {
      scrollUpButton = new ClickableTextureComponent(
        new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen, 44, 48),
        Game1.mouseCursors,
        new Rectangle(421, 459, 11, 12),
        4f
      )
      { region = Region };

      scrollDownButton = new ClickableTextureComponent(
        new Rectangle(xPositionOnScreen + width + 16, yPositionOnScreen + height - 64, 44, 48),
        Game1.mouseCursors,
        new Rectangle(421, 472, 11, 12),
        4f
      )
      { region = Region };

      scrollBar = new ClickableTextureComponent(
        new Rectangle(scrollUpButton.bounds.X + 12, scrollUpButton.bounds.Y + scrollUpButton.bounds.Height + 4, 24, 40),
        Game1.mouseCursors,
        new Rectangle(435, 463, 6, 10),
        4f
      )
      { region = Region };

      scrollBarRunner = new Rectangle(
        scrollBar.bounds.X,
        scrollUpButton.bounds.Y + scrollUpButton.bounds.Height + 4,
        scrollBar.bounds.Width,
        height - 64 - scrollUpButton.bounds.Height - 8
      );
    }

    private void CreateComponents()
    {
      var xPos = xPositionOnScreen + 16;
      var yPos = yPositionOnScreen + 16;
      var xOffset = 16 * 8;
      var yOffset = 16 * 8;
      var lastX = xPos;
      var lastY = yPos;

      rowsCount = (int)Math.Ceiling(fishList.Count / (float)rowSize);
      fishComponents.Clear();
      for (int i = 0; i < rowsCount; i++)
      {
        for (int j = 0; j < rowSize && i * rowSize + j <= fishList.Count; j++)
        {
          if (i == 0 && j == 0)
          {
            randomFishComponent = new RandomFishComponent(lastX, lastY) { region = Region };
            continue;
          }

          var x = xPos + j * xOffset - j * 4;
          var y = yPos + i * yOffset - i * 4;
          fishComponents.Add(new(fishList[i * rowSize + j - 1], x, y) { region = Region });
        }
      }
    }

    private void SetScrollBarToCurrentIndex()
    {
      if (rowsCount > 0)
      {
        if (selectedIndex > -1)
        {
          var selectedRowIndex = selectedIndex / rowSize;
          if (topRowIndex > selectedRowIndex || topRowIndex <= selectedRowIndex - pageSize)
          {
            topRowIndex = Math.Max(0, Math.Min(rowsCount - pageSize, selectedRowIndex));
          }
        }
        scrollBar.bounds.Y = scrollBarRunner.Height / Math.Max(1, rowsCount - pageSize + 1) * topRowIndex + scrollUpButton.bounds.Bottom + 4;
        if (topRowIndex == rowsCount - pageSize)
        {
          scrollBar.bounds.Y = scrollDownButton.bounds.Y - scrollBar.bounds.Height - 4;
        }
      }
      selectedIndex = -1;
      topRowIndex = Math.Max(0, Math.Min(rowsCount - pageSize, topRowIndex));

      UpdateRowsPositions();
    }

    private void UpdateRowsPositions()
    {
      var visibleFrom = topRowIndex * rowSize - 1;
      var visibleTo = rowSize * pageSize + visibleFrom;

      for (int i = 0; i < rowsCount; i++)
      {
        for (int j = 0; j < rowSize && i * rowSize + j - 1 < fishList.Count; j++)
        {
          if (i == 0 && j == 0)
          {
            randomFishComponent.bounds.Y = yPositionOnScreen + 16 + (j - topRowIndex) * ((height - 32) / pageSize);
            randomFishComponent.visible = visibleFrom <= 0 && 0 < visibleTo;
          }
          else
          {
            var index = i * rowSize + j - 1;
            var fishComponent = fishComponents[index];
            fishComponent.bounds.Y = yPositionOnScreen + 16 + (i - topRowIndex) * ((height - 32) / pageSize);
            fishComponent.visible = visibleFrom <= index && index < visibleTo;
          }
        }
      }
    }

    private void UpArrowPressed()
    {
      topRowIndex--;
      scrollUpButton.scale = 3.5f;
      SetScrollBarToCurrentIndex();
    }

    private void DownArrowPressed()
    {
      topRowIndex++;
      scrollDownButton.scale = 3.5f;
      SetScrollBarToCurrentIndex();
    }

    public override void populateClickableComponentList()
    {
      allClickableComponents ??= new();
      allClickableComponents.Clear();
      allClickableComponents.Add(randomFishComponent);
      allClickableComponents.AddRange(fishComponents);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      if (scrollUpButton.containsPoint(x, y) && topRowIndex > 0)
      {
        UpArrowPressed();
        Game1.playSound("shwip");
        return;
      }

      if (scrollDownButton.containsPoint(x, y) && topRowIndex < rowsCount - pageSize)
      {
        DownArrowPressed();
        Game1.playSound("shwip");
        return;
      }

      if (scrollBar.containsPoint(x, y))
      {
        scrolling = true;
        return;
      }

      for (int i = topRowIndex; i < topRowIndex + pageSize && i < rowsCount; i++)
      {
        for (int j = 0; j < rowSize && i * rowSize + j <= fishList.Count; j++)
        {
          if (i == 0 && j == 0)
          {
            continue;
          }

          var index = i * rowSize + j - 1;
          if (fishComponents[index].containsPoint(x, y))
          {
            actions.OnFishClick(fishList[index].ID, fishComponents[index].myID);
          }
        }
      }

      if (randomFishComponent.containsPoint(x, y))
      {
        actions.OnUnselectFishClick();
      }

      topRowIndex = Math.Max(0, Math.Min(rowsCount - pageSize, topRowIndex));
    }

    public override void leftClickHeld(int x, int y)
    {
      if (scrolling)
      {
        int y2 = scrollBar.bounds.Y;
        scrollBar.bounds.Y = Math.Min(yPositionOnScreen + height - 64 - 12 - scrollBar.bounds.Height, Math.Max(y, yPositionOnScreen + scrollUpButton.bounds.Height + 20));
        float num = (float)(y - scrollBarRunner.Y) / (float)scrollBarRunner.Height;
        topRowIndex = Math.Min(Math.Max(0, rowsCount - pageSize), Math.Max(0, (int)((float)rowsCount * num)));
        SetScrollBarToCurrentIndex();
        if (y2 != scrollBar.bounds.Y)
        {
          Game1.playSound("shiny4");
        }
      }
    }

    public override void releaseLeftClick(int x, int y)
    {
      scrolling = false;
    }

    public override void receiveScrollWheelAction(int direction)
    {
      base.receiveScrollWheelAction(direction);
      if (direction > 0 && topRowIndex > 0)
      {
        UpArrowPressed();
        Game1.playSound("shiny4");
      }
      else if (direction < 0 && topRowIndex < Math.Max(0, rowsCount - pageSize))
      {
        DownArrowPressed();
        Game1.playSound("shiny4");
      }
    }

    public override void performHoverAction(int x, int y)
    {
      base.performHoverAction(x, y);

      if (rowsCount > 0)
      {
        scrollUpButton.tryHover(x, y);
        scrollDownButton.tryHover(x, y);
      }

      for (int i = topRowIndex; i < topRowIndex + pageSize && i < rowsCount; i++)
      {
        for (int j = 0; j < rowSize && i * rowSize + j <= fishList.Count; j++)
        {
          if (i == 0 && j == 0)
          {
            continue;
          }

          var index = i * rowSize + j - 1;
          var hovered = fishComponents[index].containsPoint(x, y);
          fishComponents[index].Hover(hovered);
          if (hovered)
          {
            actions.OnFishHover(fishList[index].Name);
          }
        }
      }

      var randomFishHovered = randomFishComponent.containsPoint(x, y);
      randomFishComponent.Hover(randomFishHovered);
      if (randomFishHovered)
      {
        actions.OnUnselectFishHover(I18n.FishMenu_FishListSection_Random_Hover());
      }
    }

    public override void draw(SpriteBatch b)
    {
      // draw box
      drawTextureBox(
        b,
        spritesheetTexture,
        new(0, 64, 18, 18),
        xPositionOnScreen,
        yPositionOnScreen,
        width,
        height,
        Color.White,
        4f
      );

      // draw scrollbar
      if (rowsCount > pageSize)
      {
        scrollUpButton.draw(b);
        scrollDownButton.draw(b);
        drawTextureBox(b, Game1.mouseCursors, new(403, 383, 6, 6), scrollBarRunner.X, scrollBarRunner.Y, scrollBarRunner.Width, scrollBarRunner.Height, Color.White, 4f);
        scrollBar.draw(b);
      }

      // draw fish components
      foreach (var fishComponent in fishComponents)
      {
        fishComponent.draw(b);
      }

      randomFishComponent.draw(b);
    }
  }

  public struct FishListSectionActions
  {
    public Action<string, int> OnFishClick { get; set; }
    public Action OnUnselectFishClick { get; set; }
    public Action<string> OnFishHover { get; set; }
    public Action<string> OnUnselectFishHover { get; set; }
  }
}