using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using WillysFishingWorkshops.Handlers;
using WillysFishingWorkshops.Helpers;
using WillysFishingWorkshops.Models;

namespace WillysFishingWorkshops.UI
{
  public class FishMenu : IClickableMenu
  {
    private static List<Fish> FishList => GameStateHandler.FishList;
    private static Fish SelectedFish => GameStateHandler.SelectedFish;
    private FishListSection fishListSection;
    private SearchSection searchSection;
    private InfoSection infoSection;
    private const int searchTextBoxYOffset = 18;
    private static readonly Dimension sectionGap = new(16, 24);
    private static readonly Dimension fishListSectionDim = new(780, 532);
    private static readonly Dimension searchSectionDim = new(fishListSectionDim.Width, 64);
    private static readonly Dimension infoSectionDim = new(
      384,
      fishListSectionDim.Height + searchSectionDim.Height + sectionGap.Height + searchTextBoxYOffset
    );
    private static readonly Dimension menuDim = new(
      fishListSectionDim.Width + infoSectionDim.Width + sectionGap.Width,
      infoSectionDim.Height
    );
    private string hoverText = string.Empty;

    public FishMenu()
    {
      width = menuDim.Width;
      height = menuDim.Height;

      ResetLayout();

      snapToDefaultClickableComponent();

      GameStateHandler.FishListUpdated += ResetFishListLayout;
      GameStateHandler.SelectedFishUpdated += ResetScrollPosition;
      exitFunction = delegate
      {
        GameStateHandler.FishListUpdated -= ResetFishListLayout;
        GameStateHandler.SelectedFishUpdated -= ResetScrollPosition;
        GameStateHandler.ResetState();
      };
    }

    public void ResetLayout()
    {
      SetupLayout();
      fishListSection.ResetLayout(FishList, SelectedFish);
      infoSection.ResetLayout();
      searchSection.ResetLayout();
      populateClickableComponentList();
    }

    private void ResetFishListLayout()
    {
      fishListSection.ResetLayout(FishList, SelectedFish);
      populateClickableComponentList();
    }

    private void ResetScrollPosition()
    {
      fishListSection.ResetScrollPosition(SelectedFish);
      if (Game1.options.SnappyMenus)
      {
        populateClickableComponentList();
      }
    }

    public void SetupLayout()
    {
      var topLeft = Utility.getTopLeftPositionForCenteringOnScreen(fishListSectionDim.Width, fishListSectionDim.Height);
      xPositionOnScreen = (int)topLeft.X - infoSectionDim.Width - sectionGap.Width;
      yPositionOnScreen = (int)topLeft.Y - 64;

      fishListSection = new(
        (int)topLeft.X,
        yPositionOnScreen,
        fishListSectionDim.Width,
        fishListSectionDim.Height,
        actions: new()
        {
          OnFishClick = (id, myID) =>
          {
            GameStateHandler.SelectFish(id);
            currentlySnappedComponent = allClickableComponents.Find(e => e.myID == myID);
            populateClickableComponentList();
          },
          OnUnselectFishClick = GameStateHandler.UnselectFish,
          OnFishHover = SetHoverText,
          OnUnselectFishHover = SetHoverText
        }
      );

      searchSection = new(
        fishListSection.xPositionOnScreen,
        fishListSection.yPositionOnScreen + fishListSection.height + sectionGap.Height,
        searchSectionDim.Width,
        searchSectionDim.Height,
        actions: new()
        {
          OnSearchInput = GameStateHandler.PerformSearch,
          OnClearSearchClick = GameStateHandler.ClearSearch,
          OnSortByDifficultyClick = GameStateHandler.SortByDifficulty,
          OnSortByNameClick = GameStateHandler.SortByName,
          OnRandomClick = GameStateHandler.SelectRandomFish,
          OnConfirmClick = () => exitThisMenu(),
          OnClearSearchHover = SetHoverText,
          OnSortByDifficultyHover = SetHoverText,
          OnSortByNameHover = SetHoverText,
          OnConfirmHover = SetHoverText,
          OnRandomHover = SetHoverText
        }
      );

      infoSection = new(
        xPositionOnScreen,
        yPositionOnScreen,
        infoSectionDim.Width,
        infoSectionDim.Height,
        actions: new()
        {
          OnTagClick = GameStateHandler.PerformSearchForTag,
          OnTagHover = SetHoverText
        }
      );

      initializeUpperRightCloseButton();
      upperRightCloseButton.myID = upperRightCloseButton_ID;
    }

    private void SetHoverText(string text)
    {
      if (text.IsNotEmpty())
      {
        hoverText = text;
      }
    }

    public override void applyMovementKey(int direction)
    {
      if (Game1.options.SnappyMenus)
      {
        if (currentlySnappedComponent == null)
        {
          snapToDefaultClickableComponent();
        }

        var id = currentlySnappedComponent.myID;

        switch ((Direction)direction)
        {
          case Direction.Up:
            id = currentlySnappedComponent.upNeighborID;
            break;
          case Direction.Down:
            id = currentlySnappedComponent.downNeighborID;
            break;
          case Direction.Left:
            id = currentlySnappedComponent.leftNeighborID;
            break;
          case Direction.Right:
            id = currentlySnappedComponent.rightNeighborID;
            break;
        }

        if (id == ClickableComponent.ID_ignore)
        {
          return;
        }
        if (id == ClickableComponent.CUSTOM_SNAP_BEHAVIOR)
        {
          customSnapBehavior(direction, currentlySnappedComponent.region, currentlySnappedComponent.myID);
        }
        else
        {
          currentlySnappedComponent = allClickableComponents.Find(e => e.myID == id) ?? currentlySnappedComponent;
        }

        if (currentlySnappedComponent != null)
        {
          fishListSection.AdjustScrollPosition((Direction)direction, currentlySnappedComponent);
          snapCursorToCurrentSnappedComponent();
        }
      }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      fishListSection.receiveLeftClick(x, y, playSound);
      searchSection.receiveLeftClick(x, y, playSound);
      infoSection.receiveLeftClick(x, y, playSound);

      base.receiveLeftClick(x, y, playSound);
    }

    public override void releaseLeftClick(int x, int y)
    {
      fishListSection.releaseLeftClick(x, y);
    }

    public override void leftClickHeld(int x, int y)
    {
      fishListSection.leftClickHeld(x, y);
    }

    public override void receiveKeyPress(Keys key)
    {
      searchSection.receiveKeyPress(key);

      if (!searchSection.SearchSelected)
      {
        base.receiveKeyPress(key);
      }
    }

    public override void receiveGamePadButton(Buttons b)
    {
      base.receiveGamePadButton(b);
      searchSection.receiveGamePadButton(b);
    }

    public override void performHoverAction(int x, int y)
    {
      hoverText = string.Empty;

      base.performHoverAction(x, y);
      fishListSection?.performHoverAction(x, y);
      searchSection?.performHoverAction(x, y);
      infoSection?.performHoverAction(x, y);
    }

    public override void receiveScrollWheelAction(int direction)
    {
      fishListSection.receiveScrollWheelAction(direction);
    }

    public override void populateClickableComponentList()
    {
      allClickableComponents ??= new();
      allClickableComponents.Clear();
      allClickableComponents.AddRange(fishListSection.allClickableComponents);
      allClickableComponents.AddRange(searchSection.allClickableComponents);
      allClickableComponents.AddRange(infoSection.allClickableComponents);
      allClickableComponents.Add(upperRightCloseButton);

      CustomSnapHandler.SetCustomSnapBehavior(allClickableComponents);
    }

    protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
    {
      currentlySnappedComponent = CustomSnapHandler.HandleCustomSnapBehavior(allClickableComponents, (Direction)direction, oldRegion, oldID);
      if (currentlySnappedComponent == null)
      {
        return;
      }
      snapCursorToCurrentSnappedComponent();
    }

    public override void snapToDefaultClickableComponent()
    {
      if (Game1.options.SnappyMenus)
      {
        var firstVisibleComponent = allClickableComponents.First(e => e.region == FishListSection.Region && e.visible);
        currentlySnappedComponent = firstVisibleComponent;
        snapCursorToCurrentSnappedComponent();
      }
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
      ResetLayout();
    }

    public override void update(GameTime time)
    {
      searchSection.update(time);
      infoSection.update(time);
      populateClickableComponentList();
    }

    public override void draw(SpriteBatch b)
    {
      // draw faded background
      GameHelper.DrawFadedBackground(b, 0.2f);

      // draw sections
      fishListSection.draw(b);
      searchSection.draw(b);
      infoSection.draw(b);

      // draw hover text
      if (hoverText.IsNotEmpty())
      {
        drawHoverText(b, hoverText, Game1.smallFont);
      }

      base.draw(b);
      drawMouse(b);
    }
  }
}