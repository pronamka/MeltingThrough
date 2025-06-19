using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject platformPrefab;
    public int platformCount = 5;
    public float levelWidth = 20f;
    public float minDeltaY = 1.5f;
    public float maxDeltaY = 3f;
    public float startY = 5f;

    void Start()
    {
        Vector3 spawnPosition = new Vector3();

        spawnPosition.y = startY;

        for (int i = 0; i < platformCount; i++)
        {
            spawnPosition.x = Random.Range(-levelWidth / 2f, levelWidth / 2f);
            spawnPosition.y -= Random.Range(minDeltaY, maxDeltaY);

            Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        }

    }
}
