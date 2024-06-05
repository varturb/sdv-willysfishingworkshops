namespace WillysFishingWorkshops.Models
{
  public readonly struct Dimension
  {
    public readonly int Width;
    public readonly int Height;

    public Dimension(int width, int height)
    {
      Width = width;
      Height = height;
    }
  }
}