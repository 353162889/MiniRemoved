using UnityEngine;
using System.Collections;
using Unity.Entities;
using System;
using Unity.Collections;

public class TileMapComponent : DataComponent
{
    public Vector3 origin;      //左下角坐标
    public float tileWidth;     //每个tile的宽
    public float tileHeight;    //每个tile的高
    public int rowCount;        //行数
    public int columnCount;     //列数
    public Entity[,] entities;  //实体数组
    public int minRemoveCount;

    void OnDestroy()
    {
        entities = null;
    }
}


