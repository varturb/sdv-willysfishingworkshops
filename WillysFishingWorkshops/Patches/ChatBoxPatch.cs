using WillysFishingWorkshops.Handlers;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using WillysFishingWorkshops.UI;

namespace WillysFishingWorkshops.Patches
{
  public class ChatBoxPatch
  {
    internal static bool ConsoleCommand_Prefix(ChatBox __instance, string command)
    {
      try
      {
        var args = ArgUtility.SplitBySpace(command);
        if (args[0] == "bt")
        {
          GameStateHandler.BuyTicket(int.Parse(args[1]));
          return false;
        }

        if (args[0] == "et")
        {
          GameStateHandler.ExpireTicket();
          return false;
        }

        if(args[0] == "sf")
        {
          GameStateHandler.SelectFish(args[1]);
          return false;
        }

        if(args[0] == "q")
        {
          Game1.activeClickableMenu = new FishMenu();
          return false;
        }

        if(args[0] == "w")
        {
          Game1.warpFarmer("Beach", 39, 32, true);
          return false;
        }

        return true;
      }
      catch (Exception ex)
      {
        ModUtility.Monitor.Log($"Failed in {nameof(ConsoleCommand_Prefix)}:\n{ex}", LogLevel.Error);
        return true;
      }
    }
  }
}