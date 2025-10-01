using UnityEngine;

public interface ISewPoint : IGameService
{
    bool selected {  get; }
    void Selected();
    bool IsSelected();
}
