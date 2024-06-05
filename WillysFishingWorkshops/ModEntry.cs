using System;
using System.Collections.Generic;
using WillysFishingWorkshops.Handlers;
using WillysFishingWorkshops.Patches;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Tools;
using xTile.Dimensions;
using StardewModdingAPI.Events;
using WillysFishingWorkshops.GameEvents;
using Microsoft.Xna.Framework.Graphics;

namespace WillysFishingWorkshops
{
  public class ModEntry : Mod
  {
    public override void Entry(IModHelper helper)
    {
      ModUtility.Initialize(helper, Monitor, ModManifest);

      try
      {
        // GameLocation
        var harmony = new Harmony(ModManifest.UniqueID);
        harmony.Patch(
          original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.getFish)),
          prefix: new HarmonyMethod(typeof(GameLocationPatch), nameof(GameLocationPatch.GetFish_Prefix))
        );
        harmony.Patch(
          original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction), new Type[] { typeof(string[]), typeof(Farmer), typeof(Location) }),
          prefix: new HarmonyMethod(typeof(GameLocationPatch), nameof(GameLocationPatch.PerformAction_Prefix))
        );
        harmony.Patch(
          original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.TryGetLocationEvents), new Type[] { typeof(string).MakeByRefType(), typeof(Dictionary<string, string>).MakeByRefType() }),
          postfix: new HarmonyMethod(typeof(GameLocationPatch), nameof(GameLocationPatch.TryGetLocationEvents_Postfix))
        );
      
        // Beach
        harmony.Patch(
          original: AccessTools.Method(typeof(Beach), nameof(Beach.draw)),
          postfix: new HarmonyMethod(typeof(BeachPatch), nameof(BeachPatch.Draw_Postfix))
        );
        harmony.Patch(
          original: AccessTools.Method(typeof(Beach), nameof(Beach.MakeMapModifications)),
          postfix: new HarmonyMethod(typeof(BeachPatch), nameof(BeachPatch.MakeMapModifications_Postfix))
        );

        // FishingRod
        harmony.Patch(
          original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.startMinigameEndFunction)),
          prefix: new HarmonyMethod(typeof(FishingRodPatch), nameof(FishingRodPatch.StartMinigameEndFunction_Prefix))
        );
        harmony.Patch(
          original: AccessTools.Method(typeof(FishingRod), nameof(FishingRod.startMinigameEndFunction)),
          postfix: new HarmonyMethod(typeof(FishingRodPatch), nameof(FishingRodPatch.StartMinigameEndFunction_Postfix))
        );
        harmony.Patch(
          original: AccessTools.Method(typeof(FishingRod), "doDoneFishing"),
          prefix: new HarmonyMethod(typeof(FishingRodPatch), nameof(FishingRodPatch.DoDoneFishing_Prefix))
        );
        harmony.Patch(
          original: AccessTools.Method(typeof(FishingRod), "calculateTimeUntilFishingBite"),
          prefix: new HarmonyMethod(typeof(FishingRodPatch), nameof(FishingRodPatch.CalculateTimeUntilFishingBite_Prefix))
        );

        // BobberBar
        harmony.Patch(
          original: AccessTools.Method(typeof(BobberBar), nameof(BobberBar.draw), new Type[] { typeof(SpriteBatch)}),
          prefix: new HarmonyMethod(typeof(BobberBarPatch), nameof(BobberBarPatch.Draw_Prefix))
        );
        harmony.Patch(
          original: AccessTools.Method(typeof(BobberBar), nameof(BobberBar.draw), new Type[] { typeof(SpriteBatch)}),
          postfix: new HarmonyMethod(typeof(BobberBarPatch), nameof(BobberBarPatch.Draw_Postfix))
        );
        harmony.Patch(
          original: AccessTools.Method(typeof(BobberBar), nameof(BobberBar.update)),
          prefix: new HarmonyMethod(typeof(BobberBarPatch), nameof(BobberBarPatch.Update_Prefix))
        );
        harmony.Patch(
          original: AccessTools.Method(typeof(BobberBar), nameof(BobberBar.update)),
          postfix: new HarmonyMethod(typeof(BobberBarPatch), nameof(BobberBarPatch.Update_Postfix))
        );

        // Farmer
        harmony.Patch(
          original: AccessTools.Method(typeof(Farmer), nameof(Farmer.caughtFish)),
          prefix: new HarmonyMethod(typeof(FarmerPatch), nameof(FarmerPatch.CaughtFish_Prefix))
        );
        harmony.Patch(
          original: AccessTools.Method(typeof(Farmer), nameof(Farmer.addItemToInventory), new Type[] { typeof(Item), typeof(List<Item>) }),
          prefix: new HarmonyMethod(typeof(FarmerPatch), nameof(FarmerPatch.AddItemToInventory_Prefix))
        );
        harmony.Patch(
          original: AccessTools.Method(typeof(Farmer), nameof(Farmer.addItemToInventoryBool)),
          prefix: new HarmonyMethod(typeof(FarmerPatch), nameof(FarmerPatch.AddItemToInventoryBool_Prefix))
        );
        harmony.Patch(
          original: AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience)),
          prefix: new HarmonyMethod(typeof(FarmerPatch), nameof(FarmerPatch.GainExperience_Prefix))
        );
        
        #if DEBUG
        // ChatBox
        harmony.Patch(
          original: AccessTools.Method(typeof(ChatBox), "runCommand"),
          prefix: new HarmonyMethod(typeof(ChatBoxPatch), nameof(ChatBoxPatch.ConsoleCommand_Prefix))
        );
        #endif
      }
      catch (Exception e)
      {
        Monitor.Log($"Issue with Harmony patch: {e}", LogLevel.Error);
        return;
      }

      Helper.Events.GameLoop.GameLaunched += delegate
      {
        GameStateHandler.Init();
      };

      I18n.Init(Helper.Translation);
    }
  }
}
