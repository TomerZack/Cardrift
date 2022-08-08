using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TilemapControl : MonoBehaviour
{
    public Tile[] tiles = new Tile[30];
    Tilemap mapRenderer;
    int mapY = 12;
    public float speed = 5;
    void Start()
    {
        mapRenderer = GetComponent<Tilemap>();
    }

    
    void Update()
    {
        Vector2 position = GetComponent<Transform>().position;
        position.y = position.y - Time.deltaTime * speed;
        GetComponent<Transform>().position = position;
    }

    public void generateRoad(int tileId, int environment)
    {
        mapY = mapY + 4;
        Vector3Int position = new Vector3Int(0, mapY, 0);
        mapRenderer.SetTile(position, tiles[tileId + (environment * 10)]);
        position.y = position.y - 28;
        mapRenderer.SetTile(position, null);
    }
    public bool checkY()
    {
        if (GetComponent<Transform>().position.y + mapY <= 8) return true;
        return false;
    } 
}
