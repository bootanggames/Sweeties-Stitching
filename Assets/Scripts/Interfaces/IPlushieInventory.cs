using TMPro;
using TS.PageSlider;

public interface IPlushieInventory : IGameService
{
    PageContainer[] plushies {  get; }
   int noOfPlushieEnabled {  get; }
   TextMeshProUGUI totalPlushies {  get; }
    void NoPlushieIncrement(int c);
}
