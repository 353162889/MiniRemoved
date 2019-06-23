using UnityEngine;
using System.Collections;
using System.Numerics;
using DG.Tweening.Plugins.Options;
using Unity.Entities;

public class TileOpeareteComponent : DataComponent
{
    public Vector2Int selectPos;
    public bool isSwapOperating = false;
    public Vector2Int swapPosA;
    public Vector2Int swapPosB;
    public float swapDuration = 0.3f;
}


