using TMPro;

public interface IPlushieInventory : IGameService
{
   int noOfPlushieEnabled {  get; }
   TextMeshProUGUI totalPlushies {  get; }
    void NoPlushieIncrement(int c);
}
