using WillysFishingWorkshops.Models;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Text;
using System.Collections.Generic;

namespace WillysFishingWorkshops.Helpers
{
  public static class GameHelper
  {
    public static void ShowMessage(string message, MessageType type = MessageType.Global)
    {
      if (type == MessageType.Global)
      {
        Game1.showGlobalMessage(message);
      }
      else if (type == MessageType.Dialogue)
      {
        Game1.drawObjectDialogue(message);
      }
      else
      {
        var hudMessage = new HUDMessage(message, (int)type);
        Game1.addHUDMessage(hudMessage);
      }
    }

    public static void DrawFadedBackground(SpriteBatch b, float alpha = 0.75f)
    {
      if (!Game1.options.showClearBackgrounds)
      {
        var bounds = Game1.graphics.GraphicsDevice.Viewport.Bounds;
        b.Draw(Game1.fadeToBlackRect, new Vector2(bounds.X, bounds.Y), new Rectangle?(bounds), Color.Black * alpha, 0.0f, default, 1f, SpriteEffects.None, 0.9f);
      }
    }

    public static int CalculateDifferenceBetweenTimes(int fromTime, int toTime)
    {
      var f = fromTime.ToString().PadLeft(4, '0');
      var fh = int.Parse(f.Substring(0, 2));
      var fm = int.Parse(f.Substring(2, 2));

      var t = toTime.ToString().PadLeft(4, '0');
      var th = int.Parse(t.Substring(0, 2));
      var tm = int.Parse(t.Substring(2, 2));

      var dif = th * 60 + tm - (fh * 60 + fm);
      var gth = dif / 60;
      var gtm = dif % 60;
      var result = gth * 100 + gtm;

      return result;
    }

    public static string FormatGameTime(int time)
    {
      var t = Math.Abs(time).ToString().PadLeft(4, '0');
      var h = int.Parse(t.Substring(0, 2));
      var m = t.Substring(2, 2);

      return $"{(time < 0 ? $"-{h}" : h)}:{m}";
    }

    public static string ParseString(string text, int width, SpriteFont font)
    {
      var sb = new StringBuilder();
      var spaceLength = font.MeasureString(" ").X;
      var lines = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
      var lineNumber = 0;
      foreach (var row in lines)
      {
        var lineWords = row.Trim().Split(' ');
        var wordCount = lineWords.Length;
        var lineWidth = 0f;
        foreach (var word in lineWords)
        {
          var wordWidth = font.MeasureString(word).X;
          if (wordWidth == 0)
          {
            continue;
          }
          if (lineWidth + wordWidth + spaceLength > width)
          {
            sb.Append(Environment.NewLine);
            lineWidth = wordWidth + spaceLength;
            sb.Append(word);
            sb.Append(' ');
          }
          else
          {
            lineWidth += wordWidth + spaceLength;
            sb.Append(word);
            sb.Append(' ');
          }
        }
        lineNumber++;
        if (lineNumber < lines.Length)
        {
          sb.Append(Environment.NewLine);
        }
      }

      return sb.ToString();
    }

    public static bool TryGetItemDataByEnum<T>(Item fish, out T enumVal) where T : Enum
    {
      var tags = fish.GetContextTags();
      enumVal = default;
      foreach (var e in Enum.GetValues(typeof(T)))
      {
        if (tags.Contains(((T)e).GetString()))
        {
          enumVal = (T)e;
          return true;
        }
      }
      return false;
    }

    public static bool TryGetItemDataListByEnum<T>(Item fish, out List<T> enumVals) where T : Enum
    {
      var tags = fish.GetContextTags();
      enumVals = new();
      foreach (var e in Enum.GetValues(typeof(T)))
      {
        if (tags.Contains(((T)e).GetString()))
        {
          enumVals.Add((T)e);
        }
      }

      return enumVals.Count > 0;
    }

    public static void DrawBox(SpriteBatch b, Texture2D texture, Rectangle bounds, DrawBoxMode drawMode = DrawBoxMode.Default)
    {
      var partSize = 4;
      var sourceX = drawMode switch
      {
        DrawBoxMode.Default => 0,
        DrawBoxMode.Hovered => 12,
        DrawBoxMode.Selected => 24,
        _ => 0
      };
      var sourceY = 114;
      var scale = 4f;
      var color = Color.White;
      var scaledPart = (int)(partSize * scale);

      //start
      b.Draw(
        texture,
        new Rectangle(bounds.X, bounds.Y, scaledPart, scaledPart),
        new Rectangle(sourceX, sourceY, partSize, partSize),
        color
      );
      b.Draw(
        texture,
        new Rectangle(bounds.X, bounds.Y + scaledPart, scaledPart, bounds.Height - scaledPart * 2),
        new Rectangle(sourceX, sourceY + partSize, partSize, partSize),
        color
      );
      b.Draw(
        texture,
        new Rectangle(bounds.X, bounds.Y + bounds.Height - scaledPart, scaledPart, scaledPart),
        new Rectangle(sourceX, sourceY + partSize * 2, partSize, partSize),
        color
      );
      //mid
      b.Draw(
        texture,
        new Rectangle(bounds.X + scaledPart, bounds.Y, bounds.Width - scaledPart * 2, scaledPart),
        new Rectangle(sourceX + partSize, sourceY, partSize, partSize),
        color
      );
      b.Draw(
        texture,
        new Rectangle(bounds.X + scaledPart, bounds.Y + scaledPart, bounds.Width - scaledPart * 2, bounds.Height - scaledPart * 2),
        new Rectangle(sourceX + partSize, sourceY + partSize, partSize, partSize),
        color
      );
      b.Draw(
        texture,
        new Rectangle(bounds.X + scaledPart, bounds.Y + bounds.Height - scaledPart, bounds.Width - scaledPart * 2, scaledPart),
        new Rectangle(sourceX + partSize, sourceY + partSize * 2, partSize, partSize),
        color
      );
      //end
      b.Draw(
        texture,
        new Rectangle(bounds.X + bounds.Width - scaledPart, bounds.Y, scaledPart, scaledPart),
        new Rectangle(sourceX + partSize * 2, sourceY, partSize, partSize),
        color
      );
      b.Draw(
        texture,
        new Rectangle(bounds.X + bounds.Width - scaledPart, bounds.Y + scaledPart, scaledPart, bounds.Height - scaledPart * 2),
        new Rectangle(sourceX + partSize * 2, sourceY + partSize, partSize, partSize),
        color
      );
      b.Draw(
        texture,
        new Rectangle(bounds.X + bounds.Width - scaledPart, bounds.Y + bounds.Height - scaledPart, scaledPart, scaledPart),
        new Rectangle(sourceX + partSize * 2, sourceY + partSize * 2, partSize, partSize),
        color
      );
    }
  }
}