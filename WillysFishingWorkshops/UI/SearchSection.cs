using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using WillysFishingWorkshops.Handlers;
using WillysFishingWorkshops.Models;

namespace WillysFishingWorkshops.UI
{
  public class SearchSection : IClickableMenu
  {
    public const int Region = 3000;
    public bool SearchSelected => searchComponent.Selected;

    private static readonly Texture2D spritesheetTexture = ModUtility.Helper.ModContent.Load<Texture2D>("assets/spritesheet.png");
    private readonly SearchSectionActions actions;
    private SearchComponent searchComponent;
    private ClickableTextureComponent clearSearchButton;
    private ClickableTextureComponent sortByNameSearchButton;
    private ClickableTextureComponent sortByDifficultySearchButton;
    private ClickableTextureComponent confirmButton;
    private ClickableTextureComponent randomButton;

    public SearchSection(int x, int y, int width, int height, SearchSectionActions actions)
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
      searchComponent = new(
        xPositionOnScreen + 16,
        yPositionOnScreen + 8,
        width / 2
      );

      clearSearchButton = new(
        new Rectangle(searchComponent.xPositionOnScreen + searchComponent.width + 32 + 2, yPositionOnScreen, 64, 64),
        spritesheetTexture,
        new Rectangle(32, 98, 16, 16),
        4f
      )
      { region = Region };

      sortByDifficultySearchButton = new(
        new Rectangle(clearSearchButton.bounds.X + 64 + 2, clearSearchButton.bounds.Y, 64, 64),
        spritesheetTexture,
        new Rectangle(0, 82, 16, 16),
        4f
      )
      { region = Region };

      sortByNameSearchButton = new(
        new Rectangle(sortByDifficultySearchButton.bounds.X + 64 + 2, sortByDifficultySearchButton.bounds.Y, 64, 64),
        spritesheetTexture,
        new Rectangle(0, 98, 16, 16),
        4f
      )
      { region = Region };

      confirmButton = new(
        new Rectangle(xPositionOnScreen + width - 64, yPositionOnScreen, 64, 64),
        spritesheetTexture,
        new Rectangle(48, 98, 16, 16),
        4f
      )
      { region = Region };

      randomButton = new(
        new Rectangle(confirmButton.bounds.X - 64 - 2, confirmButton.bounds.Y, 64, 64),
        spritesheetTexture,
        new Rectangle(0, 126, 32, 32),
        2f
      )
      { region = Region };

      populateClickableComponentList();
    }

    public override void populateClickableComponentList()
    {
      allClickableComponents ??= new();
      allClickableComponents.Clear();
      allClickableComponents
        .AddRange(searchComponent.allClickableComponents
        .Select(x =>
        {
          x.region = Region;
          return x;
        })
      );
      allClickableComponents.Add(clearSearchButton);
      allClickableComponents.Add(sortByDifficultySearchButton);
      allClickableComponents.Add(sortByNameSearchButton);
      allClickableComponents.Add(confirmButton);
      allClickableComponents.Add(randomButton);
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
      searchComponent.receiveLeftClick(x, y, playSound);

      if (clearSearchButton.containsPoint(x, y))
      {
        actions.OnClearSearchClick();
        Game1.playSound("smallSelect");
        return;
      }

      if (sortByDifficultySearchButton.containsPoint(x, y))
      {
        actions.OnSortByDifficultyClick();
        Game1.playSound("smallSelect");
        return;
      }

      if (sortByNameSearchButton.containsPoint(x, y))
      {
        actions.OnSortByNameClick();
        Game1.playSound("smallSelect");
        return;
      }

      if (confirmButton.containsPoint(x, y))
      {
        actions.OnConfirmClick();
        return;
      }

      if (randomButton.containsPoint(x, y))
      {
        actions.OnRandomClick();
        Game1.playSound("smallSelect");
        return;
      }
    }

    public override void receiveKeyPress(Keys key)
    {
      if (searchComponent.Selected)
      {
        searchComponent.receiveKeyPress(key);
      }
      else
      {
        if (Game1.options.doesInputListContain(Game1.options.menuButton, key) && readyToClose())
        {
          exitThisMenu();
        }
      }
    }

    public override void receiveGamePadButton(Buttons b)
    {
      base.receiveGamePadButton(b);
      searchComponent.receiveGamePadButton(b);
    }

    public override void performHoverAction(int x, int y)
    {
      clearSearchButton?.tryHover(x, y);
      if (clearSearchButton.containsPoint(x, y))
      {
        actions.OnClearSearchHover(I18n.FishMenu_ClearButton_Hover());
      }

      sortByDifficultySearchButton?.tryHover(x, y);
      if (sortByDifficultySearchButton.containsPoint(x, y))
      {
        var hoverText = GameStateHandler.DifficultySortMode switch
        {
          SortMode.Asc => I18n.FishMenu_SortByDifficultyButton_Asc_Hover(),
          SortMode.Desc => I18n.FishMenu_SortByDifficultyButton_Desc_Hover(),
          SortMode.Off => I18n.FishMenu_SortByDifficultyButton_Off_Hover(),
          _ => I18n.FishMenu_SortByDifficultyButton_Off_Hover(),
        };
        actions.OnSortByDifficultyHover(hoverText);
      }
      sortByNameSearchButton?.tryHover(x, y);
      if (sortByNameSearchButton.containsPoint(x, y))
      {
        var hoverText = GameStateHandler.NameSortMode switch
        {
          SortMode.Asc => I18n.FishMenu_SortByNameButton_Asc_Hover(),
          SortMode.Desc => I18n.FishMenu_SortByNameButton_Desc_Hover(),
          SortMode.Off => I18n.FishMenu_SortByNameButton_Off_Hover(),
          _ => I18n.FishMenu_SortByNameButton_Off_Hover(),
        };
        actions.OnSortByNameHover(hoverText);
      }

      confirmButton?.tryHover(x, y);
      if (confirmButton.containsPoint(x, y))
      {
        actions.OnConfirmHover(I18n.FishMenu_ConfirmButton_Hover());
      }

      randomButton?.tryHover(x, y);
      if (randomButton.containsPoint(x, y))
      {
        actions.OnRandomHover(I18n.FishMenu_RandomButton_Hover());
      }
    }

    public override void update(GameTime time)
    {
      sortByDifficultySearchButton.sourceRect.X = GameStateHandler.DifficultySortMode switch
      {
        SortMode.Off => 0,
        SortMode.Asc => 16,
        SortMode.Desc => 32,
        _ => 0
      };
      sortByNameSearchButton.sourceRect.X = GameStateHandler.NameSortMode switch
      {
        SortMode.Asc => 0,
        SortMode.Desc => 16,
        _ => 0
      };
    }

    public override void draw(SpriteBatch b)
    {
      // draw search box
      searchComponent.draw(b);

      // draw buttons
      clearSearchButton.draw(b);
      sortByDifficultySearchButton.draw(b);
      sortByNameSearchButton.draw(b);
      confirmButton.draw(b);
      randomButton.draw(b);
    }
  }

  public struct SearchSectionActions
  {
    public Action<string> OnSearchInput { get; set; }
    public Action OnClearSearchClick { get; set; }
    public Action OnSortByNameClick { get; set; }
    public Action OnSortByDifficultyClick { get; set; }
    public Action OnRandomClick { get; set; }
    public Action OnConfirmClick { get; set; }
    public Action<string> OnClearSearchHover { get; set; }
    public Action<string> OnSortByNameHover { get; set; }
    public Action<string> OnSortByDifficultyHover { get; set; }
    public Action<string> OnRandomHover { get; set; }
    public Action<string> OnConfirmHover { get; set; }
  }
}