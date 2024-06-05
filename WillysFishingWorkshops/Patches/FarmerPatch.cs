using WillysFishingWorkshops.Handlers;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace WillysFishingWorkshops.Patches
{
  public class FarmerPatch
  {
    internal static bool CaughtFish_Prefix(
      Farmer __instance,
      ref bool __result,
      string itemId, int size, bool from_fish_pond = false, int numberCaught = 1
    )
    {
      try
      {
        if (__instance.IsLocalPlayer && GameStateHandler.IsTicketValid)
        {
          ModUtility.Monitor.Log($"CaughtFish_Prefix: Setting __result to false", LogLevel.Trace);
          __result = false;
          return false;
        }

        return true;
      }
      catch (Exception ex)
      {
        ModUtility.Monitor.Log($"Failed in {nameof(CaughtFish_Prefix)}:\n{ex}", LogLevel.Error);
        return true;
      }
    }

    internal static bool AddItemToInventory_Prefix(Farmer __instance, ref Item __result, Item item, List<Item> affected_items_list)
    {
      try
      {
        if (__instance.IsLocalPlayer && GameStateHandler.IsTicketValid)
        {
          ModUtility.Monitor.Log($"AddItemToInventoryBool_Prefix: Setting __result to null", LogLevel.Trace);
          __result = null;
          return false;
        }

        return true;
      }
      catch (Exception ex)
      {
        ModUtility.Monitor.Log($"Failed in {nameof(AddItemToInventory_Prefix)}:\n{ex}", LogLevel.Error);
        return true;
      }
    }

    internal static bool AddItemToInventoryBool_Prefix(Farmer __instance, ref bool __result, Item item, bool makeActiveObject = false)
    {
      try
      {
        if (__instance.IsLocalPlayer && GameStateHandler.IsTicketValid)
        {
          ModUtility.Monitor.Log($"AddItemToInventoryBool_Prefix: Setting __result to true", LogLevel.Trace);
          __result = true;
          return false;
        }

        return true;
      }
      catch (Exception ex)
      {
        ModUtility.Monitor.Log($"Failed in {nameof(AddItemToInventoryBool_Prefix)}:\n{ex}", LogLevel.Error);
        return true;
      }
    }

    internal static bool GainExperience_Prefix(Farmer __instance, int which, int howMuch)
    {
      try
      {
        if (__instance.IsLocalPlayer && which == 1 && GameStateHandler.IsTicketValid)
        {
          ModUtility.Monitor.Log($"GainExperience_Prefix: Supressing method for which={which}", LogLevel.Trace);
          return false;
        }

        return true;
      }
      catch (Exception ex)
      {
        ModUtility.Monitor.Log($"Failed in {nameof(AddItemToInventoryBool_Prefix)}:\n{ex}", LogLevel.Error);
        return true;
      }
    }
  }
}
