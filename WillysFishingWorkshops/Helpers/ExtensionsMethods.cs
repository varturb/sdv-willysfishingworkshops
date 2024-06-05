using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewModdingAPI;
using WillysFishingWorkshops.Handlers;
using WillysFishingWorkshops.Models;

namespace WillysFishingWorkshops.Helpers
{
  public static class ExtensionMethods
  {
    public static T DeepCopy<T>(this T self)
    {
      var serialized = JsonConvert.SerializeObject(self);
      return JsonConvert.DeserializeObject<T>(serialized);
    }

    public static bool IsEmpty(this string text)
    {
      return text == null || text.Length == 0;
    }

    public static bool IsNotEmpty(this string text)
    {
      return text != null && text.Length > 0;
    }

    public static bool IsOneOf(this string text, List<string> strings)
    {
      return strings.Where(x => x == text).Any();
    }

    public static Color ToColor(this string hex)
    {
      var color = System.Drawing.ColorTranslator.FromHtml(hex);
      return new Color(color.R, color.G, color.B, color.A);
    }

    public static IOrderedEnumerable<TSource> SortBy<TSource, TKey>(
      this IEnumerable<TSource> source,
      Func<TSource, TKey> keySelector,
      SortMode mode
    )
    {
      return mode switch
      {
        SortMode.Asc => source.OrderBy(keySelector),
        SortMode.Desc => source.OrderByDescending(keySelector),
        SortMode.Off => source.OrderBy(x => 0),
        _ => source.OrderBy(x => 0)
      };
    }

    public static IOrderedEnumerable<TSource> ThenSortBy<TSource, TKey>(
      this IOrderedEnumerable<TSource> elements,
      Func<TSource, TKey> keySelector,
      SortMode mode
    )
    {
      return mode switch
      {
        SortMode.Asc => elements.ThenBy(keySelector),
        SortMode.Desc => elements.ThenByDescending(keySelector),
        SortMode.Off => elements,
        _ => elements
      };
    }
  }
}