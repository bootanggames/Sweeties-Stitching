using UnityEngine;

public interface ISewPoint : IGameService
{
    bool selected {  get; }
    void Selected(bool val);
    bool IsSelected();
}
