using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapLooper : MonoBehaviour
{
    public Transform playerTransform;  // Player reference
    public Tilemap[] tilemaps;  // Assign all 9 tilemaps in Inspector
    public int tilemapSize = 10; // Adjust to match your tilemap size

    private Vector3Int lastPlayerCell;

    void Start()
    {
        if (playerTransform == null || tilemaps.Length != 9)
        {
            //Debug.LogError("Assign the player and exactly 9 tilemaps in the Inspector!");
            return;
        }

        lastPlayerCell = GetPlayerCell();
    }

    void Update()
    {
        Vector3Int playerCell = GetPlayerCell();

        if (Mathf.Abs(playerCell.x - lastPlayerCell.x) >= tilemapSize ||
            Mathf.Abs(playerCell.y - lastPlayerCell.y) >= tilemapSize)
        {
            RepositionTilemaps(playerCell);
            lastPlayerCell = playerCell;
        }
    }

    Vector3Int GetPlayerCell()
    {
        return tilemaps[0].WorldToCell(playerTransform.position);
    }

    void RepositionTilemaps(Vector3Int playerCell)
    {
        foreach (Tilemap tilemap in tilemaps)
        {
            Vector3Int tilemapCell = tilemap.WorldToCell(tilemap.transform.position);

            int offsetX = Mathf.RoundToInt((playerCell.x - tilemapCell.x) / tilemapSize);
            int offsetY = Mathf.RoundToInt((playerCell.y - tilemapCell.y) / tilemapSize);

            if (offsetX != 0 || offsetY != 0)
            {
                Vector3 moveOffset = new Vector3(offsetX * tilemapSize, offsetY * tilemapSize, 0);
                tilemap.transform.position += moveOffset;
                //Debug.Log($"Moved Tilemap {tilemap.name} by {moveOffset}");
            }
        }
    }
}
