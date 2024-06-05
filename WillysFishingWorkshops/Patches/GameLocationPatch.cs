using StardewValley;
using StardewModdingAPI;
using System;
using Microsoft.Xna.Framework;
using WillysFishingWorkshops.Handlers;
using StardewValley.Locations;
using System.Collections.Generic;
using WillysFishingWorkshops.GameEvents;

namespace WillysFishingWorkshops.Patches
{
  public class GameLocationPatch
  {
    internal static bool GetFish_Prefix(
      GameLocation __instance,
      ref Item __result,
      float millisecondsAfterNibble, string bait, int waterDepth, Farmer who, double baitPotency, Vector2 bobberTile, string locationName = null
    )
    {
      try
      {
        if (who.IsLocalPlayer && GameStateHandler.IsTicketValid)
        {
          __result = GameStateHandler.CreateFishItem();
          ModUtility.Monitor.Log($"GetFish_Prefix: __result={__result.Name}", LogLevel.Trace);

          return false;
        }

        return true;
      }
      catch (Exception ex)
      {
        ModUtility.Monitor.Log($"Failed in {nameof(GetFish_Prefix)}:\n{ex}", LogLevel.Error);
        return true;
      }
    }
    internal static bool PerformAction_Prefix(GameLocation __instance, string[] action, Farmer who, xTile.Dimensions.Location tileLocation)
    {
      try
      {
        ModUtility.Monitor.Log($"PerformAction_Prefix", LogLevel.Trace);
        
        if (__instance is Beach)
        {
          switch (ArgUtility.Get(action, 0))
          {
            case "BuyWorkshopTicket":
              GameStateHandler.ShowBuyTicketDialog();
              return false;
            case "ShowWorkshopInformation":
              GameStateHandler.ShowWorkshopInformation();
              return false;
          }
        }

        return true;
      }
      catch (Exception ex)
      {
        ModUtility.Monitor.Log($"Failed in {nameof(PerformAction_Prefix)}:\n{ex}", LogLevel.Error);
        return true;
      }
    }

    internal static void TryGetLocationEvents_Postfix(GameLocation __instance, string assetName, ref Dictionary<string, string> events)
    {
      try
      {
        if (__instance is Beach)
        {
          var @event = WillysFishingWorkshopsEvent.GetEventKeyValuePair();
          events.TryAdd(@event.Key, @event.Value);
          ModUtility.Monitor.Log($"TryGetLocationEvents_Postfix: {events.Count}", LogLevel.Trace);
        }
      }
      catch (Exception ex)
      {
        ModUtility.Monitor.Log($"Failed in {nameof(TryGetLocationEvents_Postfix)}:\n{ex}", LogLevel.Error);
      }
    }
  }
}
