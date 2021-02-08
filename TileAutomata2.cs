using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Tilemaps;
using UnityEditor;

public class TileAutomata2 : MonoBehaviour
{

    public int width;
    public int height;

    public RuleTile topTile;
    public AnimatedTile waterTile;

    public string seed;
    public bool useRandomSeed;

    [Range(0, 100)]
    public int randomFillPercent;

    [Range(0, 10)]
    public int randomTreeSpreading;

    [Range(0, 5)]
    public int regionTemperature;

    public Tile oaktreeTile;
    public Tile pinetreeTile;

    public Tilemap terrainMap;
    public Tilemap waterMap;
    public Tilemap foliageMap;

    [Range(0, 10)]
    public int stonePlacement;

    public Tile stone_0;
    public Tile stone_1;
    public Tile stone_2;

    int[,] map;    

    void Start()
    {
        GenerateMap();
    }

    void Update()
    {
        /*if ();
        {
            ClearFoliage();
            GenerateMap();
        }
        */
    }

    void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }
    }

    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = DateTime.UtcNow.ToString();
        }
        Debug.Log(seed);

        System.Random pseudoRandom = new System.Random(seed.GetHashCode());
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;

            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    public void ClearFoliage()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                foliageMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), null);
            }
        }
    }

    void OnDrawGizmos()
    {
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y] == 0)
                    {
                        terrainMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), topTile);

                        int stoneChance = pseudoRandom.Next(0, 4);

                        if (map[x, y] == 0 && map[x, y + 1] == 0 && map[x, y - 1] == 0 && map[x + 1, y] == 0 && map[x - 1, y] == 0)
                        {
                            if (pseudoRandom.Next(0, 20) < randomTreeSpreading)
                            {
                                if (pseudoRandom.Next(0, 5) <= regionTemperature)
                                {
                                    foliageMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), oaktreeTile);
                                }
                                else if (pseudoRandom.Next(0, 5) >= regionTemperature)
                                {
                                    foliageMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), pinetreeTile);
                                }
                                else if (stoneChance < 1)
                                {
                                    foliageMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), stone_0);
                                }
                                else if (stoneChance < 2)
                                {
                                    foliageMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), stone_1);
                                }
                                else if (stoneChance < 3)
                                {
                                    foliageMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), stone_2);
                                }
                            }
                        }
                    }
                    if (map[x, y] == 1)
                    {
                         waterMap.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), waterTile);
                    }
                }
            }
        }
    }
}