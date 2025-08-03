using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Resources", menuName = "Scriptable Objects/Resources")]
public class ResourceTiles : ScriptableObject
{
    public List<TilesofResources> ResourceList;
}

[Serializable]
public struct TilesofResources
{
    public ResourceEnum ResourceName;
    public List<TileBase> Tiles;
}

