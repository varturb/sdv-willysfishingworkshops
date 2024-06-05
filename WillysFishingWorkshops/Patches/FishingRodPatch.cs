using WillysFishingWorkshops.Handlers;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using System;

namespace WillysFishingWorkshops.Patches
{
  public class FishingRodPatch
  {
    internal static void DoDoneFishing_Prefix(FishingRod __instance, ref bool consumeBaitAndTackle)
    {
      try
      {
        if (__instance.lastUser.IsLocalPlayer && GameStateHandler.IsTicketValid)
        {
          consumeBaitAndTackle = false;

          ModUtility.Monitor.Log($"DoDoneFishing_Prefix: consumeBaitAndTackle={consumeBaitAndTackle}", LogLevel.Trace);
        }
      }
      catch (Exception ex)
      {
        ModUtility.Monitor.Log($"Failed in {nameof(DoDoneFishing_Prefix)}:\n{ex}", LogLevel.Error);
      }
    }

    internal static bool CalculateTimeUntilFishingBite_Prefix(FishingRod __instance, ref float __result, Vector2 bobberTile, bool isFirstCast, Farmer who)
    {
      try
      {
        if (__instance.lastUser.IsLocalPlayer && GameStateHandler.IsTicketValid)
        {
          var min = 800;
          var max = 2000;
          __result = Game1.random.Next(min, Math.Max(min, max));
          
          ModUtility.Monitor.Log($"CalculateTimeUntilFishingBite_Prefix: __result={__result}", LogLevel.Trace);
          return false;
        }
        return true;
      }
      catch (Exception ex)
      {
        ModUtility.Monitor.Log($"Failed in {nameof(DoDoneFishing_Prefix)}:\n{ex}", LogLevel.Error);
        return true;
      }
    }

    internal static void StartMinigameEndFunction_Prefix(FishingRod __instance, ref double ___baseChanceForTreasure, out double __state)
    {
      try
      {
        __state = ___baseChanceForTreasure;
        if (__instance.lastUser.IsLocalPlayer && GameStateHandler.IsTicketValid)
        {
          ___baseChanceForTreasure = 0;
          ModUtility.Monitor.Log($"StartMinigameEndFunction_Prefix: ___baseChanceForTreasure={___baseChanceForTreasure}", LogLevel.Trace);
          return;
        }
      }
      catch (Exception ex)
      {
        __state = ___baseChanceForTreasure;
        ModUtility.Monitor.Log($"Failed in {nameof(DoDoneFishing_Prefix)}:\n{ex}", LogLevel.Error);
      }
    }

    internal static void StartMinigameEndFunction_Postfix(FishingRod __instance, ref double ___baseChanceForTreasure, double __state)
    {
      try
      {
        ___baseChanceForTreasure = __state;
        ModUtility.Monitor.Log($"StartMinigameEndFunction_Postfix: ___baseChanceForTreasure={___baseChanceForTreasure}", LogLevel.Trace);
      }
      catch (Exception ex)
      {
        ModUtility.Monitor.Log($"Failed in {nameof(DoDoneFishing_Prefix)}:\n{ex}", LogLevel.Error);
      }
    }
  }
}
