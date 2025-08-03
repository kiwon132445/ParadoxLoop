using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [SerializeField] Tilemap resourceMap;
    [SerializeField] ResourceTiles tileDict;
    [SerializeField] int generationRange = 1000;
    [SerializeField] float generationTime = 5;

    float generationTimer;

    public void RandomGenerateResource()
    {
        int x = UnityEngine.Random.Range(-generationRange, generationRange);
        int y = UnityEngine.Random.Range(-generationRange, generationRange);
        int resourceType = UnityEngine.Random.Range(0, Enum.GetValues(typeof(ResourceEnum)).Length - 1);


        if (resourceMap.GetTile(new Vector3Int(x, y)) == null)
        {
            resourceMap.SetTile(new Vector3Int(x, y), tileDict.ResourceList[resourceType].Tiles[0]);
        }
    }

    private void Update()
    {
        if (generationTimer <= 0)
        {
            RandomGenerateResource();
            generationTimer = generationTime;
        }
        else
        {
            generationTimer -= Time.deltaTime;
        }
    }
}
