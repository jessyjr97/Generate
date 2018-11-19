using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour {

    public enum GenerationType { random, perlinNoise }
    public GenerationType generationType;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public bool autoUpdate;

    public Tilemap tilemap;
    public TerrainType[] regions;
    public TerrainType[] gisements;

    private TileBase FindTileFromRegion(float valeur) {
        for (int i = 0; i < regions.Length; i++)
            if (valeur <= regions[i].value)
                return regions[i].tile;
        return regions[0].tile;
    }

    private TileBase FindTileFromGisement(float valeur) {
        for (int i = 0; i < gisements.Length; i++)
            if (valeur <= gisements[i].value)
                return gisements[i].tile;
        return gisements[0].tile;
    }

    public void SetTileMap(TileBase[] customTilemap) {
        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapHeight; x++)
                tilemap.SetTile(new Vector3Int(x, y, 0), customTilemap[y * mapWidth + x]);
    }

    public void GenerateMapRandom() {
        TileBase[] customTilemap = new TileBase[mapWidth * mapHeight];
        for (int y = 0; y < mapWidth; y++)
            for (int x = 0; x < mapHeight; x++) {
                var rnd = Random.Range(0f, 1f);
                customTilemap[y * mapWidth + x] = FindTileFromRegion(rnd);
            };
        SetTileMap(customTilemap);
    }

    public void GenerateMapPerlinNoise() {
        var noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        var noiseMapGisement = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed + 1, noiseScale * 10, octaves, persistance, lacunarity, offset);
        TileBase[] customTilemap = new TileBase[mapWidth * mapHeight];
        TileBase[] customTilemapGisement = new TileBase[mapWidth * mapHeight];
        for (int y = 0; y < mapWidth; y++)
            for (int x = 0; x < mapHeight; x++) {
                var value = noiseMap[x, y];
                var valueGisement = noiseMapGisement[x, y];
                if (value >= 0.8f)
                    customTilemap[y * mapWidth + x] = FindTileFromGisement(valueGisement);
                else
                    customTilemap[y * mapWidth + x] = FindTileFromRegion(value);
            };
        SetTileMap(customTilemap);
    }

    public void OnValidate() {
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 1)
            octaves = 1;
    }

    public void GenerateMap() {
        if (generationType == GenerationType.random)
            GenerateMapRandom();
        else if (generationType == GenerationType.perlinNoise)
            GenerateMapPerlinNoise();
    }
}
[System.Serializable]
public struct TerrainType {
    public string name;
    public float value;
    public TileBase tile;
}