using System;
using WillysFishingWorkshops.Handlers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace WillysFishingWorkshops.Patches
{
  public class BobberBarPatch
  {
    internal static void Update_Prefix(BobberBar __instance, GameTime time, out bool __state)
    {
      try
      {
        __state = false;
        if (Game1.player.IsLocalPlayer && GameStateHandler.IsTicketValid)
        {
          __state = Game1.player.fishCaught == null || Game1.player.fishCaught.Length == 0;
          if (__state)
          {
            Game1.player.fishCaught.Add(string.Empty, new int[1]);
          }
        }
      }
      catch (Exception ex)
      {
        __state = false;
        ModUtility.Monitor.Log($"Failed in {nameof(Update_Prefix)}:\n{ex}", LogLevel.Error);
      }
    }

    internal static void Update_Postfix(BobberBar __instance, GameTime time, bool __state)
    {
      try
      {
        if (Game1.player.IsLocalPlayer && GameStateHandler.IsTicketValid)
        {
          if (__state && Game1.player.fishCaught.Length == 1)
          {
            Game1.player.fishCaught.Clear();
          }
        }
      }
      catch (Exception ex)
      {
        ModUtility.Monitor.Log($"Failed in {nameof(Update_Postfix)}:\n{ex}", LogLevel.Error);
      }
    }

    internal static void Draw_Prefix(BobberBar __instance, SpriteBatch b, out bool __state)
    {
      try
      {
        __state = false;
        if (Game1.player.IsLocalPlayer && GameStateHandler.IsTicketValid)
        {
          __state = Game1.player.fishCaught == null || Game1.player.fishCaught.Length == 0;
          if (__state)
          {
            Game1.player.fishCaught.Add(string.Empty, new int[1]);
          }
        }
      }
      catch (Exception ex)
      {
        __state = false;
        ModUtility.Monitor.Log($"Failed in {nameof(Draw_Prefix)}:\n{ex}", LogLevel.Error);
      }
    }

    internal static void Draw_Postfix(BobberBar __instance, SpriteBatch b, bool __state)
    {
      try
      {
        if (Game1.player.IsLocalPlayer && GameStateHandler.IsTicketValid)
        {
          if (__state && Game1.player.fishCaught.Length == 1)
          {
            Game1.player.fishCaught.Clear();
          }
        }
      }
      catch (Exception ex)
      {
        ModUtility.Monitor.Log($"Failed in {nameof(Draw_Postfix)}:\n{ex}", LogLevel.Error);
      }
    }
  }
}