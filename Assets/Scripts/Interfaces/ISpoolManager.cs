using System.Collections.Generic;
using UnityEngine;

public interface ISpoolManager : IGameService
{
   List<GameObject> spoolList {  get; }
    GameObject GetSpool(int index);
    void ChangeSpriteOfSpools(Sprite sp);
    void SpoolList(int count);
}
