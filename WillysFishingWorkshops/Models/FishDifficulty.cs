using System.ComponentModel;

namespace WillysFishingWorkshops.Models
{
  public enum FishDifficulty
  {
    [Description("fish_difficulty_easy")]
    Easy,

    [Description("fish_difficulty_medium")]
    Medium,
    
    [Description("fish_difficulty_hard")]
    Hard,

    [Description("fish_difficulty_extremely_hard")]
    ExtremelyHard
  }
}