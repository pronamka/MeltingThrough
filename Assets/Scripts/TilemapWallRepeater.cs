using UnityEngine;

public class TilemapWallRepeater : MonoBehaviour
{
    public GameObject wallGridPrefab;
    public int repeatCount;
    public float gridWidth;
    public float gridHeight;

    void Start()
    {
        for (int i = -3; i < repeatCount; i++)
        {
            Vector3 position = new Vector3(0f, -i*gridHeight, 0f);
            Instantiate(wallGridPrefab, position, Quaternion.identity);
        }
    }
}