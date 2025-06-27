using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;

public class TileGenerator : MonoBehaviour
{
    public event Action OnGenerationComplete;
    public HashSet<Vector3Int> OccupiedPositions { get; private set; } = new HashSet<Vector3Int>();

    public Tilemap islandTilemap;
    public TileBase islandTile;
    public int islandCount = 10;
    public int minIslandWidth = 3;
    public int maxIslandWidth = 10;
    public int minIslandHeight = 2;
    public int maxIslandHeight = 6;

    [SerializeField] public int levelMinX = -150;
    [SerializeField] public int levelMaxX = 150;
    [SerializeField] public int levelMinY = 0;
    [SerializeField] public int levelMaxY = 30;

    [Range(0, 1)] public float topBumpiness = 0.5f;
    [Range(0, 1)] public float bottomBumpiness = 0.5f;
    [Range(0, 1)] public float leftBumpiness = 0.5f;
    [Range(0, 1)] public float rightBumpiness = 0.5f;
    [Range(0, 1)] public float holeChance = 0.12f;

    void Start()
    {
        GenerateIslands();

    }

    public void GenerateIslands()
    {
        islandTilemap.ClearAllTiles();
        OccupiedPositions.Clear();

        int columns = Mathf.CeilToInt(Mathf.Sqrt(islandCount));
        int rows = Mathf.CeilToInt((float)islandCount / columns);

        float regionWidth = (levelMaxX - levelMinX + 1) / (float)columns;
        float regionHeight = (levelMaxY - levelMinY + 1) / (float)rows;

        int islandIndex = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (islandIndex >= islandCount)
                    break;

                int xStart = Mathf.RoundToInt(levelMinX + col * regionWidth);
                int xEnd = Mathf.RoundToInt(levelMinX + (col + 1) * regionWidth) - 1;
                int yStart = Mathf.RoundToInt(levelMinY + row * regionHeight);
                int yEnd = Mathf.RoundToInt(levelMinY + (row + 1) * regionHeight) - 1;

                int width = Mathf.Clamp(UnityEngine.Random.Range(minIslandWidth, maxIslandWidth + 1), 1, xEnd - xStart + 1);
                int height = Mathf.Clamp(UnityEngine.Random.Range(minIslandHeight, maxIslandHeight + 1), 1, yEnd - yStart + 1);

                int startX = xStart + (regionWidth > width ? UnityEngine.Random.Range(0, Mathf.Max(1, (int)regionWidth - width)) : 0);
                int minIslandTop = yStart + height;
                int maxIslandTop = yEnd;
                int topYBase = UnityEngine.Random.Range(minIslandTop, maxIslandTop + 1);

                int sideRange = Mathf.Max(1, height / 2);
                int leftTop = topYBase + UnityEngine.Random.Range(-sideRange, sideRange + 1);
                int leftBottom = leftTop - height + 1 + UnityEngine.Random.Range(-sideRange, sideRange + 1);
                int rightTop = topYBase + UnityEngine.Random.Range(-sideRange, sideRange + 1);
                int rightBottom = rightTop - height + 1 + UnityEngine.Random.Range(-sideRange, sideRange + 1);

                leftTop = Mathf.Clamp(leftTop, yStart + 1, yEnd);
                rightTop = Mathf.Clamp(rightTop, yStart + 1, yEnd);
                leftBottom = Mathf.Clamp(leftBottom, yStart, leftTop - 1);
                rightBottom = Mathf.Clamp(rightBottom, yStart, rightTop - 1);

                int[] topY = new int[width];
                int[] bottomY = new int[width];

                for (int x = 0; x < width; x++)
                {
                    float t = width > 1 ? x / (float)(width - 1) : 0f;

                    int interpTop = Mathf.RoundToInt(Mathf.Lerp(leftTop, rightTop, t));
                    int interpBottom = Mathf.RoundToInt(Mathf.Lerp(leftBottom, rightBottom, t));

                    if (x != 0 && x != width - 1 && UnityEngine.Random.value < topBumpiness)
                        interpTop = Mathf.Clamp(interpTop + UnityEngine.Random.Range(-1, 2), interpBottom + 1, yEnd);
                    if (x != 0 && x != width - 1 && UnityEngine.Random.value < bottomBumpiness)
                        interpBottom = Mathf.Clamp(interpBottom + UnityEngine.Random.Range(-1, 2), yStart, interpTop - 1);

                    topY[x] = interpTop;
                    bottomY[x] = interpBottom;
                }

                for (int x = 0; x < width; x++)
                {
                    if (UnityEngine.Random.value < holeChance) continue;
                    for (int y = bottomY[x]; y <= topY[x]; y++)
                    {
                        Vector3Int tilePos = new Vector3Int(startX + x, y, 0);
                        islandTilemap.SetTile(tilePos, islandTile);
                        OccupiedPositions.Add(tilePos);
                    }
                }

                islandIndex++;
            }
        }
        OnGenerationComplete?.Invoke();
    }

    public bool IsPositionOnIsland(Vector3 worldPosition)
    {
        Vector3Int cell = islandTilemap.WorldToCell(worldPosition);
        return OccupiedPositions.Contains(cell);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 bottomLeft = new Vector3(levelMinX, levelMinY, 0);
        Vector3 topLeft = new Vector3(levelMinX, levelMaxY + 1, 0);
        Vector3 topRight = new Vector3(levelMaxX + 1, levelMaxY + 1, 0);
        Vector3 bottomRight = new Vector3(levelMaxX + 1, levelMinY, 0);

        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
    }
#endif
}