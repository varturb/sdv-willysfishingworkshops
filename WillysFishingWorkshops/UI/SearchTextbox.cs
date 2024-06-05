using StardewValley;
using StardewValley.Menus;

namespace WillysFishingWorkshops.UI
{
  public class SearchTextbox : TextBox
  {
    public event TextBoxEvent OnTextChanged;
    
    public SearchTextbox()
     : base(null, null, Game1.dialogueFont, Game1.textColor)
    {
    }

    public override void RecieveTextInput(char inputChar)
    {
      base.RecieveTextInput(inputChar);
      OnTextChanged?.Invoke(this);
    }

    public override void RecieveTextInput(string text)
    {
      base.RecieveTextInput(text);
      OnTextChanged?.Invoke(this);
    }

    public override void RecieveCommandInput(char command)
    {
      base.RecieveCommandInput(command);
      OnTextChanged?.Invoke(this);
    }
  }
}