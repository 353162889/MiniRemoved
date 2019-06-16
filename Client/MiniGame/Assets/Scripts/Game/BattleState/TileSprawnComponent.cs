using System.Collections.Generic;
using UnityEngine;

public class TileSprawnComponent : DataComponent
{
    public float downDuration = 0.5f;
    public bool isTileDowning = false;
    public HashSet<Vector2Int> lstDownPos = new HashSet<Vector2Int>();
}
