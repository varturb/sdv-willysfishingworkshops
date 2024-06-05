using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using StardewValley;
using WillysFishingWorkshops.Helpers;

namespace WillysFishingWorkshops.Models
{
  public class Fish
  {
    public Item Item { get; set; }
    public string ID { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int DifficultyLevel { get; private set; }
    public FishDifficulty Difficulty { get; private set; }
    public List<string> Tags { get; private set; } = new();
    public Rectangle SourceRect { get; private set; }

    public Fish(Item item, int difficultyLevel)
    {
      Item = item;
      ID = item.ItemId;
      Name = item.DisplayName;
      Description = Regex.Replace(item.getDescription(), @"\r\n?|\n", "").Trim();
      Tags = TagColorHelper.FilterTags(item.GetContextTags().ToList());
      DifficultyLevel = difficultyLevel;
      
      if (GameHelper.TryGetItemDataByEnum<FishDifficulty>(item, out var difficulty))
      {
        Difficulty = difficulty;
        SourceRect = GetSourceRect();
      }
    }

    private Rectangle GetSourceRect()
    {
      return Difficulty switch
      {
        FishDifficulty.Easy => new(26, 64, 8, 8),
        FishDifficulty.Medium => new(34, 64, 8, 8),
        FishDifficulty.Hard => new(42, 64, 8, 8),
        FishDifficulty.ExtremelyHard => new(50, 64, 8, 8),
        _ => new(18, 64, 8, 8)
      };
    }
  }
}