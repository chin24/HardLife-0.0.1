﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class World{
    /// <summary>
    /// The World Class that can generate and create a game world based on inputed parameters
    /// </summary>
	#region "Declarations"
    public string name;
    internal int saveNum = 1;

	internal Vector2 worldSize;
    internal Vector2 localSize;
    internal float nodeRadius;
    internal float nodeDiameter;
    int worldSizeX, worldSizeY, localSizeX, localSizeY;
    [Range(45, 60)]
    public int randomFillPercent = 53;
    internal string seed = null;

    public LocalMap[,] localMaps;
	public LocalMap localMap;
    
    public NameGen nameGen = new NameGen();
	#endregion
    ///-----initializer(s)
    public World(Vector2 _worldSize, Vector2 _localSize, float _nodeRadius)
    {
        //layerNames = new [] { "Base Map", "Temperature Map", "Rain Map", "Mountain Map", "Biome Map"};
        //SetLayers(layerNames);
        worldSize = _worldSize;
        localSize = _localSize;
        nodeRadius = _nodeRadius;
        nodeDiameter = nodeRadius * 2;
        worldSizeX = Mathf.RoundToInt(worldSize.x / nodeDiameter);
        worldSizeY = Mathf.RoundToInt(worldSize.y / nodeDiameter);
        localSizeX = Mathf.RoundToInt(localSize.x / nodeDiameter);
        localSizeY = Mathf.RoundToInt(localSize.y / nodeDiameter);
        localMaps = new LocalMap[worldSizeX, worldSizeY];
    }
    //--------------Map Generation Functions----------------

    /// <summary>
    /// Generates Map based on layers, there currently needs to be at lease 4 layers
    ///This includes a Base Map, Temperature Map, Rain Map, and Mountain Map
    /// </summary>
    public void GenerateMap(string _seed = null, string _name = null)
    {
        if (_seed == null || _seed == "")
        {
            seed = Time.time.ToString();
        }

        if (_name == null)
        {

            name = nameGen.GenerateWorldName(seed);
        }
        else
        {
            name = _name;
        }

        //SetLayers(layerNames);

        GenerateLayers();
               

    }

    private void GenerateLayers()
    {
        FresNoise noise = new FresNoise();

        //Declarations
        
        float mountainScale = 10f;
        
        float rainScale = 7f;
        
        float tempScale = 5f;

        //BaseMap Generation
        int[,] baseMap = RandomFillMap(new int[worldSizeX, worldSizeY]);
        baseMap = SmoothMap(baseMap, 2);
        baseMap = SmoothMap(baseMap, 1);

        //Temperature Map Generation
        float[,] tempMap = GenerateTempMap(noise.CalcNoise(worldSizeX, worldSizeY, seed+"temp", tempScale));

        //Mountain Map Generation
        float[,] mMap = noise.CalcNoise(worldSizeX, worldSizeY,  seed+"elevation", mountainScale);

        //Rain Map Generation
        float[,] rainMap = noise.CalcNoise(worldSizeX, worldSizeY, seed+"rain", rainScale);

        //Biome Map
        string[,] biomeMap = GenerateBiomes(baseMap, tempMap,mMap,rainMap);

		tempMap = GenerateAverageTemp(biomeMap, tempMap,mMap,rainMap);

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                //Set Local Map Declarations
                localMaps[x, y].biome = biomeMap[x, y];
                localMaps[x, y].aveTemp = tempMap[x, y];
                localMaps[x, y].elevation = mMap[x, y];
                localMaps[x, y].rain = rainMap[x, y];
            }
        }
    }

	private float[,] GenerateAverageTemp(string[,] biomeMap,float[,] tempMap, float[,] elevMap, float[,] rainMap)
    {
		float baseTemp = 25;
		float tempScale = 5;
		float rainScale = 3;
		float elevScale = 4;
		Dictionary<string,float> biomeTemps = new Dictionary<string,float> ();
		biomeTemps.Add ("Grass", 15);
		biomeTemps.Add ("Ice", -5);
		biomeTemps.Add ("Jungle", 20);
		biomeTemps.Add ("Desert", 25);

        float[,] map = new float[worldSizeX, worldSizeY];

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
				map [x, y] = biomeTemps [biomeMap [x, y]] + (tempMap [x, y] * tempScale * 2 - tempScale)
				- (rainMap [x, y] * rainScale)
				- (elevMap [x, y] * elevScale);
            }
            
        }

		return map;
    }

    private string[,] GenerateBiomes(int[,] baseMap, float[,] tempMap, float[,] elevMap, float[,] rainMap)
    {
        string[,] map = new string[worldSizeX, worldSizeY];

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {

                if( baseMap[x, y] == 1)
                {
                    map[x, y] = BiomeName(tempMap[x,y],rainMap[x,y], elevMap[x,y]);
                }
                else
                    map[x, y] = "Water";

            }
        }
        return map;
    }
    /// <summary>
    /// Returns biome ID based on matrix
    /// </summary>
    /// <param name="tileTemp"> int of tile temp from 0 to 2</param>
    /// <param name="tileRain">int of tile rain amount from 0 to 2</param>
    /// <returns></returns>
    private string BiomeName(float temp, float rain, float elevation)
    {
		string biomeName = "Unknown";
        float[] mountainNC = { .5f, .75f }; //Mountain noise conversion scale
        float[] rainNC = { .33f, .66f };
        float[] tempNC = { .33f, .5f };
		if (temp < tempNC [0]) {
			if (rain < rainNC [0]) {
				if (elevation < mountainNC [0]) {
					biomeName = "Ice";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Ice";
				} else {
					biomeName = "Ice";
				}
			} else if (rain < rainNC [1]) {
				if (elevation < mountainNC [0]) {
					biomeName = "Grass";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Ice";
				} else {
					biomeName = "Ice";
				}
			} else {
				if (elevation < mountainNC [0]) {
					biomeName = "Grass";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Grass";
				} else {
					biomeName = "Ice";
				}
			}
		} else if (temp < tempNC [1]) {
			if (rain < rainNC [0]) {
				if (elevation < mountainNC [0]) {
					biomeName = "Desert";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Grass";
				} else {
					biomeName = "Grass";
				}
			} else if (rain < rainNC [1]) {
				if (elevation < mountainNC [0]) {
					biomeName = "Grass";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Grass";
				} else {
					biomeName = "Ice";
				}
			} else {
				if (elevation < mountainNC [0]) {
					biomeName = "Jungle";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Grass";
				} else {
					biomeName = "Ice";
				}
			}
		} else {
			if (rain < rainNC [0]) {
				if (elevation < mountainNC [0]) {
					biomeName = "Desert";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Desert";
				} else {
					biomeName = "Desert";
				}
			} else if (rain < rainNC [1]) {
				if (elevation < mountainNC [0]) {
					biomeName = "Desert";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Grass";
				} else {
					biomeName = "Grass";
				}
			} else {
				if (elevation < mountainNC [0]) {
					biomeName = "Jungle";
				} else if (elevation < mountainNC [1]) {
					biomeName = "Jungle";
				} else {
					biomeName = "Jungle";
				}
			}
		}

		return biomeName;
    }

    

    private float[,] GenerateTempMap(float[,] tempMap)
    {
        FresNoise noise = new FresNoise();
        int max = worldSizeY / 2;
        float[,] map = new float[worldSizeX, worldSizeY];
        for (int x = 0; x < worldSizeX; x++)
        {
            float hCount = 0f;
            for (int y = 0; y < worldSizeY; y++)
            {
                if (y <= max)
                {
                    float toFloat = hCount / (float)max;
                    float adjustedFloat = (toFloat + .5f * tempMap[x, y])/1.5f;
                    //int scaledNum = noise.ScaleFloatToInt(adjustedFloat, heightMap);
                    
                    map[x, y] = adjustedFloat;
                    hCount++;
                }
                else
                {
                    float toFloat = hCount/ (float)max;
                    float adjustedFloat = (toFloat + .5f * tempMap[x, y]) / 1.5f;
                    //int scaledNum = noise.ScaleFloatToInt(adjustedFloat, heightMap);

                    map[x, y] = adjustedFloat;
                    hCount--;
                }
            }
        }

        return map;
    }
    ///------- Region Functions
    ///
    
    ///----------Helper Functions
    ///

    public bool IsInMapRange(int x, int y)
    {
        if (x >= 0 && x < worldSizeX && y >= 0 && y < worldSizeY)
            return true;
        else
            return false;
    }

    internal int[,] RandomFillMap(int[,] baseMap)
    {

        System.Random randNum = new System.Random(seed.GetHashCode());

        for (int x = 0; x < worldSizeX; x++)
        {
            for (int y = 0; y < worldSizeY; y++)
            {
                if (x == 0 || x == worldSizeX - 1 || y == 0 || y == worldSizeY - 1)
                    baseMap[x, y] = 0;
                else
                    baseMap[x, y] = (randNum.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }

        return baseMap;
    }

    public int[,] SmoothMap(int[,] baseMap, int lDist)
    {
        for (int y = 0; y < worldSizeY; y++)
        {
            for (int x = 0; x < worldSizeX; x++)
            {
                int nbrWaterTiles = GetSurroundingWaterCount(baseMap, x, y, lDist);
                double powX = lDist * 2f + 1;
                int tileCount = (int)Math.Pow(powX, 2) / 2;
                if (nbrWaterTiles > tileCount + 1)
                    baseMap[x, y] = 0;
                else if (nbrWaterTiles < tileCount - 1)
                    baseMap[x, y] = 1;
            }
        }

        return baseMap;
    }
    /// <summary>
    /// returns the number of surrounding water (int 0) tiles there are around a lDist radius of int from an int array
    /// </summary>
    /// <param name="baseMap">map with ints</param>
    /// <param name="gridX">position x to check</param>
    /// <param name="gridY">position y to check</param>
    /// <param name="lDist">distance from x,y to check</param>
    /// <returns></returns>
    int GetSurroundingWaterCount(int[,] baseMap, int gridX, int gridY, int lDist)
    {
        int waterCount = 0;
        for (int nbrX = gridX - lDist; nbrX <= gridX + lDist; nbrX++)
        {
            for (int nbrY = gridY - lDist; nbrY <= gridY + lDist; nbrY++)
            {
                if (IsInMapRange(nbrX, nbrY))
                {
                    if (nbrX != gridX || nbrY != gridY)
                    {
                        if (baseMap[nbrX, nbrY] == 0)
                            waterCount++;
                    }
                }
                else
                {
                    waterCount++;
                }

            }
        }

        return waterCount;
    }

}
