using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewValley;

namespace WillysFishingWorkshops.Helpers
{
  public class TagColorHelper
  {
    public static List<string> FilterTags(List<string> tags)
    {
      var filteredTags = new List<string>();
      foreach (var tag in tags)
      {
        var mappedTag = MapTag(tag);
        if (mappedTag.IsNotEmpty())
        {
          filteredTags.AddRange(mappedTag.Split(',').ToList());
        }
      }

      return SortByCriteria(filteredTags);
    }

    public static Color GetColorForTag(string tag)
    {
      return GetColorsForTags().FirstOrDefault(x => x.Value.Contains(tag)).Key;
    }

    private static string MapTag(string tag)
    {
      return tag switch
      {
        "fish_motion_smooth" => "smooth",
        "fish_motion_sinker" => "sinker",
        "fish_motion_floater" => "floater",
        "fish_motion_dart" => "dart",
        "fish_motion_mixed" => "mixed",
        "fish_difficulty_easy" => "easy",
        "fish_difficulty_medium" => "medium",
        "fish_difficulty_hard" => "hard",
        "fish_difficulty_extremely_hard" => "extreme",
        "season_spring" => "spring",
        "season_summer" => "summer",
        "season_fall" => "fall",
        "season_winter" => "winter",
        "season_all" => "spring,summer,fall,winter",
        "fish_favor_weather_sunny" => "sunny",
        "fish_favor_weather_rainy" => "rainy",
        "fish_favor_weather_windy" => "windy",
        "fish_favor_weather_both" => "sunny,rainy",
        "fish_legendary" => "legendary",
        "fish_legendary_family" => "legendary II",
        _ => string.Empty
      };
    }

    private static Dictionary<Color, List<string>> GetColorsForTags()
    {
      var colorPalette = new Dictionary<Color, List<string>>
      {
        { "#ef6352".ToColor(), new() { "legendary", "legendary II" } },
        { "#ce8f39".ToColor(), new() { "easy", } },
        { "#dadfe5".ToColor(), new() { "medium" } },
        { "#ffff18".ToColor(), new() { "hard" } },
        { "#801fe2".ToColor(), new() { "extreme" } },
        { "#87bca2".ToColor(), new() { "smooth", "sinker", "dart", "floater", "mixed" } },
        { "#8bc926".ToColor(), new() { "spring" } },
        { "#feca39".ToColor(), new() { "summer"} },
        { "#a44300".ToColor(), new() { "fall" } },
        { "#c5d1eb".ToColor(), new() { "winter" } },
        { "#fffb76".ToColor(), new() { "sunny" } },
        { "#5a7684".ToColor(), new() { "rainy" } },
        { "#a88fac".ToColor(), new() { "windy" } }
      };

      return colorPalette;
    }

    public static string GetHoverTextForTag(string tag)
    {
      return tag switch
      {
        var t when new[] { "easy", "medium", "hard", "extreme" }.Contains(t) => I18n.FishMenu_InfoPane_Difficulty_Hover(),
        var t when new[] { "smooth", "sinker", "dart", "floater", "mixed" }.Contains(t) => I18n.FishMenu_InfoPane_Behavior_Hover(),
        var t when new[] { "spring", "summer", "fall", "winter" }.Contains(t) => I18n.FishMenu_InfoPane_Season_Hover(),
        var t when new[] { "sunny", "rainy", "windy" }.Contains(t) => I18n.FishMenu_InfoPane_Weather_Hover(),
        _ => string.Empty
      };
    }

    public static Color GetTextColorForTag(Color color)
    {
      var brightnes = (int)Math.Sqrt(
        color.R * color.R * .299 +
        color.G * color.G * .587 +
        color.B * color.B * .114);
      return brightnes > 130 ? Game1.textColor : Color.White;
    }

    private static List<string> SortByCriteria(List<string> tags)
    {
      var result = new List<string>();
      var criteria = new List<string>(){
        "easy", "medium", "hard", "extreme",
        "spring", "summer", "fall", "winter",
        "dart", "sinker", "floater", "mixed", "smooth",
        "sunny", "rainy", "windy",
        "legendary", "legendary II"
      };

      foreach (var tag in criteria)
      {
        if (tags.Contains(tag))
        {
          result.Add(tag);
        }
      }

      return result;
    }
  }
}