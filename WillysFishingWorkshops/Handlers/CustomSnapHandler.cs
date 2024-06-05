using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Menus;
using WillysFishingWorkshops.Models;
using WillysFishingWorkshops.UI;

namespace WillysFishingWorkshops.Handlers
{
  public class CustomSnapHandler
  {
    private const int customID = ClickableComponent.CUSTOM_SNAP_BEHAVIOR;
    private const int ignoreID = ClickableComponent.ID_ignore;
    private const int closeButtonID = IClickableMenu.upperRightCloseButton_ID;
    private static List<ClickableComponent> componentList;

    public static void SetCustomSnapBehavior(List<ClickableComponent> components)
    {
      componentList = components;
      SetSearchRegion(components);
      SetFishListRegion(components);
      SetInfoRegion(components);
      SetCloseButton(components);
    }

    public static ClickableComponent HandleCustomSnapBehavior(List<ClickableComponent> components, Direction direction, int oldRegion, int oldID)
    {
      componentList = components;
      ModUtility.Monitor.Log($"HandleCustomSnapBehavior: direction={direction}, oldRegion={oldRegion}, oldID={oldID}", LogLevel.Trace);
      return oldRegion switch
      {
        FishListSection.Region when direction == Direction.Down => GetClosestComponentFromRegion(oldID, SearchSection.Region, direction),
        FishListSection.Region when direction == Direction.Left => GetClosestComponentFromRegion(oldID, InfoSection.Region, direction),
        SearchSection.Region when direction == Direction.Left => GetClosestComponentFromRegion(oldID, InfoSection.Region, direction),
        SearchSection.Region when direction == Direction.Up => GetClosestComponentFromRegion(oldID, FishListSection.Region, direction),
        InfoSection.Region when direction == Direction.Right => GetClosestComponentFromRegion(oldID, SearchSection.Region, direction),
        _ => null,
      };
    }

    private static void SetFishListRegion(List<ClickableComponent> components)
    {
      var region = FishListSection.Region;
      var regionComponents = components
        .Where(e => e.region == region)
        .OrderBy(e => e.bounds.Center.Y)
        .ThenBy(e => e.bounds.Center.X)
        .ToList();

      var anyTagComponent = components.Any(e => e.region == InfoSection.Region);
      var lastIndex = regionComponents.Count - 1;
      var rowSize = regionComponents.DistinctBy(e => e.bounds.Center.Y).Count();
      var columnSize = regionComponents.DistinctBy(e => e.bounds.Center.X).Count();

      var lastRow = rowSize - 1;
      var lastColumn = columnSize - 1;

      for (int y = 0; y < rowSize; y++)
      {
        for (int x = 0; x < columnSize && y * columnSize + x <= lastIndex; x++)
        {
          var currentIndex = y * columnSize + x;
          var component = regionComponents[currentIndex];
          var myID = currentIndex + region;

          component.myID = myID;
          component.fullyImmutable = true;
          SetUpID(component, currentIndex, x, y, rowSize, columnSize, lastRow, lastColumn, lastIndex);
          SetDownID(component, currentIndex, x, y, rowSize, columnSize, lastRow, lastColumn, lastIndex);
          SetLeftID(component, currentIndex, x, y, rowSize, columnSize, lastRow, lastColumn, lastIndex, anyTagComponent);
          SetRightID(component, currentIndex, x, y, rowSize, columnSize, lastRow, lastColumn, lastIndex);
        }
      }
    }

    private static void SetSearchRegion(List<ClickableComponent> components)
    {
      var region = SearchSection.Region;
      var regionComponents = components
        .Where(e => e.region == region)
        .OrderBy(e => e.bounds.Center.X)
        .ToList();
      var anyTagComponent = components.Any(e => e.region == InfoSection.Region);

      var lastIndex = regionComponents.Count - 1;

      for (int x = 0; x < regionComponents.Count; x++)
      {
        var component = regionComponents[x];
        component.myID = x + region;
        component.fullyImmutable = true;
        component.upNeighborID = customID;
        component.downNeighborID = ignoreID;
        component.leftNeighborID = x == 0 ? (anyTagComponent ? customID : ignoreID) : x + region - 1;
        component.rightNeighborID = x == lastIndex ? ignoreID : x + region + 1;
      }
    }

    private static void SetInfoRegion(List<ClickableComponent> components)
    {
      var region = InfoSection.Region;
      var regionComponents = components
        .Where(e => e.region == region)
        .OrderBy(e => e.bounds.Center.Y)
        .ThenBy(e => e.bounds.Center.X)
        .ToList();

      var lastIndex = regionComponents.Count - 1;
      var rowSize = regionComponents.DistinctBy(e => e.bounds.Center.Y).Count();

      for (int i = 0; i <= lastIndex; i++)
      {
        var component = regionComponents[i];
        component.myID = i + region;
        component.fullyImmutable = true;
      }

      for (int i = 0; i <= lastIndex; i++)
      {
        var component = regionComponents[i];
        SetTagIDs(component, regionComponents, i);
      }
    }

    private static void SetCloseButton(List<ClickableComponent> components)
    {
      var component = components.First(x => x.myID == closeButtonID);
      component.fullyImmutable = true;
      component.leftNeighborID = GetClosestID(component, components, Direction.Left, ignoreID);
      component.downNeighborID = component.leftNeighborID;
    }

    private static void SetUpID(ClickableComponent component, int currentIndex, int x, int y, int rowSize, int columnSize, int lastRow, int lastColumn, int lastIndex)
    {
      if (y == 0)
      {
        component.upNeighborID = x == lastColumn ? closeButtonID : ignoreID;
        return;
      }

      component.upNeighborID = component.myID - columnSize;
    }

    private static void SetDownID(ClickableComponent component, int currentIndex, int x, int y, int rowSize, int columnSize, int lastRow, int lastColumn, int lastIndex)
    {
      if (y == lastRow)
      {
        component.downNeighborID = customID;
        return;
      }
      if (currentIndex + columnSize > lastIndex)
      {
        component.downNeighborID = customID;
        return;
      }

      component.downNeighborID = component.myID + columnSize;
    }

    private static void SetLeftID(ClickableComponent component, int currentIndex, int x, int y, int rowSize, int columnSize, int lastRow, int lastColumn, int lastIndex, bool anyTagComponent)
    {
      component.leftNeighborID = x == 0 ? (anyTagComponent ? customID : ignoreID) : component.myID - 1;
    }

    private static void SetRightID(ClickableComponent component, int currentIndex, int x, int y, int rowSize, int columnSize, int lastRow, int lastColumn, int lastIndex)
    {
      if (x == lastColumn)
      {
        component.rightNeighborID = y == 0 ? closeButtonID : ignoreID;
        return;
      }

      component.rightNeighborID = currentIndex + 1 <= lastIndex ? component.myID + 1 : ignoreID;
    }

    private static void SetTagIDs(ClickableComponent component, List<ClickableComponent> components, int currentIndex)
    {
      var center = component.bounds.Center;
      var upperComponents = components.Where(e => e.bounds.Center.Y < center.Y).ToList();
      var downComponents = components.Where(e => e.bounds.Center.Y > center.Y).ToList();
      var leftComponents = components.Where(e => e.bounds.Center.X < center.X && e.bounds.Center.Y == center.Y).ToList();
      var rightComponents = components.Where(e => e.bounds.Center.X > center.X && e.bounds.Center.Y == center.Y).ToList();

      component.upNeighborID = GetClosestID(component, upperComponents, Direction.Up, ignoreID);
      component.downNeighborID = GetClosestID(component, downComponents, Direction.Down, ignoreID);
      component.leftNeighborID = GetClosestID(component, leftComponents, Direction.Left, ignoreID);
      component.rightNeighborID = GetClosestID(component, rightComponents, Direction.Right, customID);
    }

    private static int GetClosestID(ClickableComponent component, List<ClickableComponent> components, Direction direction, int defaultID)
    {
      components = direction switch
      {
        Direction.Up => components.Where(e => e.bounds.Center.Y < component.bounds.Center.Y).ToList(),
        Direction.Down => components.Where(e => e.bounds.Center.Y > component.bounds.Center.Y).ToList(),
        Direction.Left => components.Where(e => e.bounds.Center.X < component.bounds.Center.X).ToList(),
        Direction.Right => components.Where(e => e.bounds.Center.X > component.bounds.Center.X).ToList(),
        _ => components
      };

      var closestID = defaultID;
      var lastDiff = 0f;
      foreach (var uc in components)
      {
        var diff = Vector2.Distance(new(component.bounds.Center.X, component.bounds.Center.Y), new(uc.bounds.Center.X, uc.bounds.Center.Y));
        if (lastDiff == 0 || diff < lastDiff)
        {
          lastDiff = diff;
          closestID = uc.myID;
        }
      }

      return closestID;
    }

    private static ClickableComponent GetClosestComponentFromRegion(int id, int region, Direction direction)
    {
      ModUtility.Monitor.Log($"GetClosestComponentFromRegion: id={id}, region={region}", LogLevel.Trace);
      var snappedComponent = componentList.Find(e => e.myID == id);
      var closeComponents = componentList
        .Where(e => e.region == region && e.visible)
        .ToList();

      var closestID = GetClosestID(snappedComponent, closeComponents, direction, ignoreID);
      if (closestID != ignoreID)
      {
        return closeComponents.Find(e => e.myID == closestID);
      }

      return null;
    }
  }
}