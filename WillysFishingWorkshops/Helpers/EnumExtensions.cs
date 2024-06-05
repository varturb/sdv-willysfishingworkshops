using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

namespace WillysFishingWorkshops.Helpers
{
  public static class EnumExtensions
  {
    private static readonly ConcurrentDictionary<string, string> DisplayNameCache = new();

    public static string GetString(this Enum value)
    {
      var key = $"{value.GetType().FullName}.{value}";

      var displayName = DisplayNameCache.GetOrAdd(key, x =>
      {
        var name = (DescriptionAttribute[])value
                  .GetType()
                  .GetTypeInfo()
                  .GetField(value.ToString())
                  .GetCustomAttributes(typeof(DescriptionAttribute), false);

        return name.Length > 0 ? name[0].Description : value.ToString();
      });

      return displayName;
    }
  }
}