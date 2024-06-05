using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using WillysFishingWorkshops.GameEvents;

namespace WillysFishingWorkshops.Patches
{
  public class BeachPatch
  {
    internal static void Draw_Postfix(Beach __instance, SpriteBatch b)
    {
      try
      {
        if (!Game1.isFestival() && 
          (!Game1.fadeIn && WillysFishingWorkshopsEvent.HasSeenRequiredEvent()
            || WillysFishingWorkshopsEvent.HasSeenEvent()))
        {
          var texture = ModUtility.Helper.ModContent.Load<Texture2D>("assets/spritesheet.png");
          var targetPosition = Game1.GlobalToLocal(Game1.viewport, new Vector2(37 * 64, 28 * 64));
          b.Draw(
            texture,
            targetPosition,
            new Rectangle(0, 0, 64, 64),
            Color.White,
            0f,
            new Vector2(6f, 6f),
            4f,
            SpriteEffects.None,
            0.009f
          );
        }
      }
      catch (Exception ex)
      {
        ModUtility.Monitor.Log($"Failed in {nameof(Draw_Postfix)}:\n{ex}", LogLevel.Error);
      }
    }

    internal static void MakeMapModifications_Postfix(Beach __instance, bool force)
    {
      try
      {
        if (!Game1.isFestival() && WillysFishingWorkshopsEvent.HasSeenEvent())
        {
          __instance.setMapTile(37, 31, 2000, "Buildings", "ShowWorkshopInformation", 1);
          __instance.setMapTile(38, 31, 2000, "Buildings", "ShowWorkshopInformation", 1);
          __instance.setMapTile(39, 31, 2001, "Buildings", "BuyWorkshopTicket", 1);
          ModUtility.Monitor.Log($"MakeMapModifications_Postfix", LogLevel.Trace);
        }
      }
      catch (Exception ex)
      {
        ModUtility.Monitor.Log($"Failed in {nameof(MakeMapModifications_Postfix)}:\n{ex}", LogLevel.Error);
      }
    }
  }
}