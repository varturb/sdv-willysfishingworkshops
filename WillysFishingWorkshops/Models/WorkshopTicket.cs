using StardewValley;
using WillysFishingWorkshops.Helpers;

namespace WillysFishingWorkshops.Models
{
  public enum WorkshopTicketOption
  {
    TwoHours,
    SixHours
  }

  public class WorkshopTicket
  {
    public int TimeStarted { get; private set; }
    public int Duration { get; private set; }
    public WorkshopTicketOption Option { get; private set; }
    public int TimeLeft => GameHelper.CalculateDifferenceBetweenTimes(Game1.timeOfDay, TimeStarted + Duration);

    public int Cost => GetPrice(Option);
    public bool Expired => TimeLeft <= 0;

    private WorkshopTicket(WorkshopTicketOption option)
    {
      TimeStarted = Game1.timeOfDay;
      Duration = GetDuration(option);
      Option = option;
    }

    public static bool TryGetTicket(WorkshopTicketOption option, out WorkshopTicket ticket)
    {
      ticket = new WorkshopTicket(option);
      if (Game1.player.Money >= ticket.Cost)
      {
        return true;
      }

      ticket = null;
      return false;
    }

    private static int GetPrice(WorkshopTicketOption option)
    {
      return option switch
      {
        WorkshopTicketOption.TwoHours => 200,
        WorkshopTicketOption.SixHours => 500,
        _ => 0
      };
    }

    private static int GetDuration(WorkshopTicketOption option)
    {
      return option switch
      {
        WorkshopTicketOption.TwoHours => 200,
        WorkshopTicketOption.SixHours => 600,
        _ => 0
      };
    }
  }
}