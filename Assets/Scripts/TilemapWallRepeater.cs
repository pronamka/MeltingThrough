using UnityEngine;

public class TilemapWallRepeater : MonoBehaviour
{
    public GameObject wallGridPrefab;
    public GameObject floorPrefab;
    public int repeatCount;
    public float gridWidth;
    public float gridHeight;

    public GameObject boss;

    void Start()
    {
        Vector3 position;
        for (int i = -3; i < repeatCount; i++)
        {
            position = new Vector3(0f, -i*gridHeight, 0f);
            Instantiate(wallGridPrefab, position, Quaternion.identity);
        }
        position = new Vector3(0f, -repeatCount * gridHeight, 0f);
        Instantiate(floorPrefab, position, Quaternion.identity);
        Instantiate(boss, position, Quaternion.identity);
    }
}