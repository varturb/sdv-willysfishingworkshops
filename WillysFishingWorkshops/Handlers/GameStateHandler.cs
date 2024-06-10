using System;
using System.Collections.Generic;
using System.Linq;
using WillysFishingWorkshops.Debug;
using WillysFishingWorkshops.Helpers;
using WillysFishingWorkshops.Models;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using WillysFishingWorkshops.UI;
using StardewValley.Menus;
using WillysFishingWorkshops.Events;

namespace WillysFishingWorkshops.Handlers
{
  public class GameStateHandler
  {
    public static bool IsTicketValid => CheckIfTicketIsValid();
    public static List<Fish> FishList { get; private set; }
    public static Fish SelectedFish { get; private set; }
    public static Fish LastCreatedFish { get; private set; }
    public static string FilterValue { get; private set; }
    public static event SimpleEvent FishListUpdated;
    public static event SimpleEvent SelectedFishUpdated;
    public static event SimpleEvent FilterValueUpdated;
    public static SortMode NameSortMode { get; private set; } = SortMode.Asc;
    public static SortMode DifficultySortMode { get; private set; } = SortMode.Asc;

    private static WorkshopTicket ticket;
    private readonly static List<Fish> baseFishList = new();

    public static void Init()
    {
      ModUtility.Helper.Events.GameLoop.TimeChanged += OnTimeChanged;
      ModUtility.Helper.Events.GameLoop.DayStarted += OnDayStarted;
      ModUtility.Helper.Events.Display.Rendered += OnRendered;

      InitFishItems();
    }

    public static void BuyTicket(int option)
    {
      switch (option)
      {
        case 1:
          ModUtility.Monitor.Log($"GameStateHandler.BuyTicket: {WorkshopTicketOption.TwoHours}", LogLevel.Trace);
          BuyTicket(WorkshopTicketOption.TwoHours);
          break;
        case 2:
          ModUtility.Monitor.Log($"GameStateHandler.BuyTicket: {WorkshopTicketOption.SixHours}", LogLevel.Trace);
          BuyTicket(WorkshopTicketOption.SixHours);
          break;
      };
    }

    public static void BuyTicket(WorkshopTicketOption option)
    {
      ModUtility.Monitor.Log($"GameStateHandler.BuyTicket: option={option}", LogLevel.Trace);

      if (WorkshopTicket.TryGetTicket(option, out var newTicket))
      {
        ticket = newTicket;
        Game1.player.Money -= ticket.Cost;
        Game1.playSound("coin");

        ShowFishMenu();
      }
      else
      {
        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
      }
    }

    public static void ExpireTicket()
    {
      ModUtility.Monitor.Log($"GameStateHandler.ExpireTicket", LogLevel.Trace);
      ExpireTicket(silent: false);
    }

    public static Item CreateFishItem()
    {
      LastCreatedFish = SelectedFish ?? GetRandomFish(allFish: true);
      return ItemRegistry.Create(LastCreatedFish.ID);
    }

    public static void SelectFish(string id)
    {
      var fish = FishList.FirstOrDefault(x => x.ID == id);
      if (fish != null)
      {
        SelectedFish = fish;
        SelectedFishUpdated?.Invoke();
        ModUtility.Monitor.Log($"GameStateHandler.SelectFish: id={SelectedFish.ID} name={SelectedFish.Name}", LogLevel.Trace);
        ModUtility.Monitor.Log($"GameStateHandler.SelectFish: SelectedFishUpdated.Invoke", LogLevel.Trace);
      }
    }

    public static void UnselectFish()
    {
      SelectedFish = null;
      SelectedFishUpdated?.Invoke();
      ModUtility.Monitor.Log($"GameStateHandler.UnselectFish", LogLevel.Trace);
      ModUtility.Monitor.Log($"GameStateHandler.UnselectFish: SelectedFishUpdated.Invoke", LogLevel.Trace);
    }

    public static void SelectRandomFish()
    {
      if (FishList.Count > 0)
      {
        SelectedFish = GetRandomFish();
        SelectedFishUpdated?.Invoke();
        ModUtility.Monitor.Log($"GameStateHandler.SelectRandomFish: id={SelectedFish.ID} name={SelectedFish.Name}", LogLevel.Trace);
        ModUtility.Monitor.Log($"GameStateHandler.SelectFish: SelectedFishUpdated.Invoke", LogLevel.Trace);
      }
    }

    public static void InitFishItems()
    {
      var data = DataLoader.Fish(Game1.content);
      baseFishList.Clear();
      
      foreach (var dataItem in data)
      {
        var item = ItemRegistry.Create(dataItem.Key);
        var isInvalid = item.GetContextTags().Any(i => i.StartsWith("fish_trap") || i == "fish_nonfish");
        if (!isInvalid)
        {
          var difficultyLevel = int.Parse(dataItem.Value.Split('/').ElementAt(1));
          baseFishList.Add(new Fish(item, difficultyLevel));
        }
      }
      
      FishList = baseFishList;
      PerformSort();
      FilterValue = string.Empty;
    }

    public static void PerformSearchForTag(string tag)
    {
      FilterValue = tag.Trim();
      FishList = PerformFilter(baseFishList);
      PerformSort();
      FilterValueUpdated?.Invoke();
      ModUtility.Monitor.Log($"GameStateHandler.PerformSearchForTag: FilterValueUpdated.Invoke", LogLevel.Trace);
    }

    public static void PerformSearch(string text)
    {
      FilterValue = text.Trim();
      FishList = PerformFilter(baseFishList);
      PerformSort();
      ModUtility.Monitor.Log($"GameStateHandler.PerformSearch: FishListUpdated.Invoke", LogLevel.Trace);
    }

    public static void ResetState()
    {
      FilterValue = string.Empty;
      FishList = PerformFilter(baseFishList);
      PerformSort();
      ModUtility.Monitor.Log($"GameStateHandler.ResetState", LogLevel.Trace);
    }

    public static void ClearSearch()
    {
      FilterValue = string.Empty;
      FishList = PerformFilter(baseFishList);
      PerformSort();
      FilterValueUpdated?.Invoke();
      ModUtility.Monitor.Log($"GameStateHandler.ClearSearch: FilterValueUpdated.Invoke", LogLevel.Trace);
    }

    public static void SortByDifficulty()
    {
      DifficultySortMode = DifficultySortMode switch
      {
        SortMode.Asc => SortMode.Desc,
        SortMode.Desc => SortMode.Off,
        SortMode.Off => SortMode.Asc,
        _ => SortMode.Off
      };
      ModUtility.Monitor.Log($"GameStateHandler.SortByDifficulty: {DifficultySortMode}", LogLevel.Trace);
      PerformSort();
    }

    public static void SortByName()
    {
      NameSortMode = NameSortMode switch
      {
        SortMode.Asc => SortMode.Desc,
        SortMode.Desc => SortMode.Asc,
        _ => SortMode.Asc
      };
      ModUtility.Monitor.Log($"GameStateHandler.SortByName: {NameSortMode}", LogLevel.Trace);
      PerformSort();
    }

    private static Fish GetRandomFish(bool allFish = false)
    {
      var rnd = new Random();

      if (allFish)
      {
        var index = rnd.Next(0, baseFishList.Count);
        return baseFishList[index];
      }

      if (FishList.Count > 0)
      {
        var index = rnd.Next(0, FishList.Count);
        return FishList[index];
      }

      return baseFishList.First();
    }

    private static void PerformSort()
    {
      FishList = FishList
        .SortBy(x => x.DifficultyLevel, DifficultySortMode)
        .ThenSortBy(x => x.Name, NameSortMode)
        .ToList();
      FishListUpdated?.Invoke();
      ModUtility.Monitor.Log($"GameStateHandler.PerformSort: {DifficultySortMode}, {NameSortMode}", LogLevel.Trace);
      ModUtility.Monitor.Log($"GameStateHandler.PerformSort: FishListUpdated.Invoke", LogLevel.Trace);
    }

    private static void ExpireTicket(bool silent = false)
    {
      ticket = null;
      SelectedFish = null;
      ResetState();
      if (!silent)
      {
        GameHelper.ShowMessage(I18n.GlobalInfo_TicketExpired(), MessageType.Dialogue);
      }
      ModUtility.Monitor.Log($"GameStateHandler.ExpireTicket", LogLevel.Trace);
    }

    private static bool CheckIfTicketIsValid()
    {
      return Game1.player.currentLocation is Beach && ticket != null && !ticket.Expired;
    }

    public static void ShowBuyTicketDialog()
    {
      var responses = new List<Response>();
      if (ticket != null && !ticket.Expired)
      {
        responses.Add(new("0", I18n.TicketMachine_Dialog_OptionSelectFish()));
      }
      responses.Add(new("2", I18n.TicketMachine_Dialog_Option2h()));
      responses.Add(new("6", I18n.TicketMachine_Dialog_Option6h()));
      responses.Add(new("Cancel", I18n.TicketMachine_Dialog_OptionCancel()));

      Game1.player.currentLocation.createQuestionDialogue(
        I18n.TicketMachine_Dialog_Title(),
        responses.ToArray(),
        HandleAnswer
      );
    }

    public static void ShowWorkshopInformation()
    {
      Game1.activeClickableMenu = new InfoBoardMenu();
    }

    private static void HandleAnswer(Farmer farmer, string answer)
    {
      switch (answer)
      {
        case "2":
          BuyTicket(WorkshopTicketOption.TwoHours);
          break;
        case "6":
          BuyTicket(WorkshopTicketOption.SixHours);
          break;
        case "0":
          ShowFishMenu();
          break;
      }
    }

    private static void ShowFishMenu()
    {
      if (Game1.activeClickableMenu is DialogueBox dialogueBox)
      {
        dialogueBox.closeDialogue();
        Game1.activeClickableMenu = new FishMenu();
        Game1.playSound("smallSelect");
      }
    }

    private static List<Fish> PerformFilter(List<Fish> fishList)
    {
      var filterValue = FilterValue.ToLower();
      if (filterValue.IsEmpty())
      {
        return fishList;
      }

      var result = fishList
        .Where(
          x => x.Name.ToLower().Contains(filterValue) ||
          x.Tags.Any(x => x.ToLower().Contains(filterValue))
        )
        .ToList();

      return result;
    }

    private static void OnTimeChanged(object sender, TimeChangedEventArgs e)
    {
      if (ticket != null)
      {
        if (ticket.Expired)
        {
          ExpireTicket();
        }
        else if (ticket.TimeLeft == 20)
        {
          GameHelper.ShowMessage(I18n.GlobalInfo_TicketAboutToExpire());
        }
        else
        {
          ModUtility.Monitor.Log($"Workshop ticket expires in {ticket.TimeLeft}", LogLevel.Trace);
        }
      }
    }

    private static void OnDayStarted(object sender, DayStartedEventArgs e)
    {
      ExpireTicket(silent: true);
    }

    private static void OnRendered(object sender, RenderedEventArgs e)
    {
      if (!Context.IsWorldReady)
        return;

      var index = 0;
      foreach (var menu in Game1.onScreenMenus)
      {
        if (menu is InfoCornerBox)
        {
          Game1.onScreenMenus.RemoveAt(index);
          break;
        }
        index++;
      }

      if (Game1.eventUp || ticket == null || !IsTicketValid)
        return;

      var isRandomFish = SelectedFish == null;
      var isBobberBar = Game1.activeClickableMenu is BobberBar;

      var add = true;
      foreach (var menu in Game1.onScreenMenus)
      {
        if (menu is InfoCornerBox)
        {
          add = false;
          break;
        }
      }

      if (add)
      {
        Game1.onScreenMenus.Add(new InfoCornerBox(
          isRandomFish && !isBobberBar ? I18n.GlobalInfo_TitleRandomFish() : LastCreatedFish?.Name ?? SelectedFish.Name,
          I18n.GlobalInfo_TicketExpiryTime(GameHelper.FormatGameTime(ticket.TimeLeft)),
          5,
          5,
          isRandomFish && !isBobberBar
        ));
      }
    }
  }
}