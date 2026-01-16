using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TutorialScreenWithType
{
    public string screenName;
    public GameObject screenParent;
    //public List<GameObject> screen;
    public List<TutorialScreens> screens;
}
