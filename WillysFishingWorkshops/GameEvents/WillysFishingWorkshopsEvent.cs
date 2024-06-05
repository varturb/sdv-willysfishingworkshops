using System.Collections.Generic;
using System.Linq;
using StardewValley;

namespace WillysFishingWorkshops.GameEvents
{
  public class WillysFishingWorkshopsEvent
  {
    private const string eventId = "_e1";
    private const string time = "/t 600 1800";
    private const string requiredEventSeenId = "739330";
    private const string requiredEventSeen = $"/e {requiredEventSeenId}";
    private const string requiredMail = "/*n spring_2_1";
    private readonly static string modId = ModUtility.Manifest.UpdateKeys.ElementAt(0).Split(':').ElementAt(1);
    private readonly static string eventSeenId = modId + eventId;
    private readonly static string eventKey = eventSeenId + time + requiredEventSeen + requiredMail;
    
    public static bool HasSeenRequiredEvent()
    {
      return Game1.MasterPlayer.eventsSeen.Contains(requiredEventSeenId);
    }

    public static bool HasSeenEvent()
    {
      return Game1.MasterPlayer.eventsSeen.Contains(eventSeenId);
    }

    public static KeyValuePair<string, string> GetEventKeyValuePair()
    {
      return new(eventKey, BuildEventString());
    }

    public static string BuildEventString()
    {
      return string.Concat(
        "ocean/-1000 -1000",
        "/farmer 26 30 2 Willy 38 31 1",
        "/skippable",
        "/animate Willy false true 200 40 41 42 41 40",
        "/move farmer 0 5 2 true",
        "/viewport 32 33 false",
        "/pause 200",                 
        "/playSound crafting",
        "/pause 1000",                
        "/playSound crafting",
        "/pause 1000",                
        "/playSound crafting",
        "/pause 300",                 
        "/move farmer 12 0 1 true",
        "/pause 200",                 
        "/playsound seagulls",
        "/stopAnimation Willy",
        "/pause 1000",                
        "/move Willy 0 1 2 true",
        "/pause 500",                 
        "/move Willy 1 0 1 true",
        "/pause 500",                 
        "/facedirection Willy 0 true",
        "/viewport move 1 0 6000",
        "/pause 1700",                    
        "/facedirection Willy 1 true",
        "/pause 1000",
        "/animate Willy false true 500 28 29 30 31",
        "/pause 1600",                
        "/facedirection farmer 0 true",
        "/pause 200",                 
        "/playsound seagulls",
        "/pause 1500",                
        "/playsound seagulls",
        "/pause 1500",                
        "/emote farmer 40",
        "/pause 1000",                
        "/stopanimation Willy",
        "/pause 400",
        "/facedirection Willy 2",
        "/pause 200",
        "/emote Willy 16",
        "/pause 500",
        $"/speak Willy \"{I18n.IntroEvent_DialogSequence_1()}\"$0",
        $"/speak Willy \"{I18n.IntroEvent_DialogSequence_2()}\"$0",
        "/pause 300",
        "/move farmer 0 -3 0",
        "/facedirection Willy 3",
        "/pause 300",
        "/facedirection Willy 0",
        "/pause 500",
        "/playsound seagulls",
        "/emote farmer 8",
        "/pause 1000",
        $"/speak Willy \"{I18n.IntroEvent_DialogSequence_3()}\"$h",
        "/pause 500",
        "/facedirection Willy 3",
        "/pause 500",
        "/facedirection farmer 1",
        $"/speak Willy \"{I18n.IntroEvent_DialogSequence_4()}\"$0",
        $"/speak Willy \"{I18n.IntroEvent_DialogSequence_5()}\"$h",
        "/pause 500",
        "/facedirection Willy 3",
        "/pause 300",
        "/playsound seagulls",
        "/facedirection Willy 2",
        "/pause 200",
        "/facedirection Willy 1",
        "/pause 1500",
        "/animate Willy false true 1000 28 29 30 31",
        "/pause 3000",
        $"/speak Willy \"{I18n.IntroEvent_DialogSequence_6()}\"$u",
        "/pause 500",
        "/move farmer 0 1 2",
        "/move farmer 1 0 1 true",
        "/pause 2000",
        "/emote farmer 28",
        "/pause 800",
        "/stopAnimation Willy",
        "/playsound seagulls",
        "/emote Willy 32",
        "/pause 1000",
        "/facedirection Willy 2",
        "/pause 300",
        "/facedirection farmer 0",
        "/pause 200",
        $"/speak Willy \"{I18n.IntroEvent_DialogSequence_7()}\"$0",
        $"/speak Willy \"{I18n.IntroEvent_DialogSequence_8()}\"$h",
        "/pause 1000",
        "/facedirection Willy 3",
        "/pause 1000",
        "/facedirection Willy 2",
        "/pause 500",
        $"/speak Willy \"{I18n.IntroEvent_DialogSequence_9()}\"$0",
        "/pause 600",
        "/playsound seagulls",
        "/move Willy -1 0 3",
        "/move Willy 0 1 2",
        "/facedirection farmer 3 true",
        "/move Willy 0 2 2",
        "/move Willy -8 0 3",
        "/move Willy 0 -2 3",
        "/warp Willy -1000 -1000",
        "/playsound doorClose",
        "/playsound seagulls",
        "/pause 500",
        "/facedirection farmer 0",
        "/pause 1500",
        "/move farmer 0 -1 0 true",
        "/viewport move 0 -2 5000",
        "/playsound seagulls",
        "/pause 2000",
        "/end position 39 32"
      );
    }
  }
}