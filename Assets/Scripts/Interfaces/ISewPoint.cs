using System.Collections.Generic;
using UnityEngine;

public interface ISewPoint : IGameService
{
    //List<Transform> stitchEffect_ThreadPoints {  get;}
    bool startFlag { get; }
    bool selected {  get; }
    void Selected(bool val);
    bool IsSelected();
    void ChangeText(string s);
}
