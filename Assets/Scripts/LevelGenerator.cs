using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private GameObject[] platformPrefabs;

    [SerializeField, Range(10, 500)]
    private int totalPlatforms = 100;

    [Header("Level Dimensions")]
    [SerializeField, Range(50f, 1000f)]
    private float levelWidth = 300f;

    [SerializeField]
    private float startY = 10f;

    [SerializeField]
    private float endY = -500f;

    [Header("Basic Spacing")]
    [SerializeField, Range(1f, 15f)]
    private float minJumpDistance = 3f;

    [SerializeField, Range(1f, 15f)]
    private float maxJumpDistance = 8f;

    [SerializeField, Range(1f, 20f)]
    private float minVerticalGap = 2f;

    [SerializeField, Range(1f, 20f)]
    private float maxVerticalGap = 6f;

    [Header("Epic Patterns")]
    [SerializeField, Range(0f, 1f)] private float clusterChance = 0.25f;
    [SerializeField, Range(2, 8)] private int minClusterSize = 3;
    [SerializeField, Range(2, 8)] private int maxClusterSize = 6;
    [SerializeField, Range(1f, 10f)] private float clusterMinHeightGap = 2f;

    [SerializeField, Range(0f, 1f)] private float spiralChance = 0.15f;
    [SerializeField, Range(4, 12)] private int spiralPlatforms = 8;
    [SerializeField, Range(5f, 25f)] private float spiralRadius = 15f;
    [SerializeField, Range(1f, 10f)] private float spiralMinHeightGap = 2f;

    [SerializeField, Range(0f, 1f)] private float zigzagChance = 0.2f;
    [SerializeField, Range(3, 10)] private int zigzagLength = 6;
    [SerializeField, Range(1f, 10f)] private float zigzagMinHeightGap = 2f;

    [SerializeField, Range(0f, 1f)] private float bridgeChance = 0.18f;
    [SerializeField, Range(3, 12)] private int bridgeLength = 7;
    [SerializeField, Range(1f, 10f)] private float bridgeMinHeightGap = 3f;

    [SerializeField, Range(0f, 1f)] private float towerChance = 0.12f;
    [SerializeField, Range(3, 8)] private int towerHeight = 5;
    [SerializeField, Range(1f, 10f)] private float towerMinHeightGap = 3f;

    [SerializeField, Range(0f, 1f)] private float diamondChance = 0.1f;
    [SerializeField, Range(1f, 10f)] private float diamondMinHeightGap = 2f;

    [SerializeField, Range(0f, 1f)] private float waveChance = 0.15f;
    [SerializeField, Range(4, 15)] private int waveLength = 8;
    [SerializeField, Range(1f, 10f)] private float waveMinHeightGap = 2f;

    [Header("Difficulty")]
    [SerializeField, Range(0f, 2f)] private float difficultyMultiplier = 1.2f;
    [SerializeField, Range(0f, 1f)] private float chaosLevel = 0.3f;
    [SerializeField] private AnimationCurve difficultyProgression = AnimationCurve.EaseInOut(0f, 0.2f, 1f, 1f);

    [Header("Advanced Settings")]
    [SerializeField] private bool allowOverlaps = false;
    [SerializeField, Range(1f, 10f)] private float minPlatformDistance = 2f;
    [SerializeField] private int seed = 0;

    private List<Vector3> spawnedPositions = new List<Vector3>();
    private List<GameObject> platforms = new List<GameObject>();
    private System.Random rng;

    void Start()
    {
        GenerateEpicLevel();
    }

    private void GenerateEpicLevel()
    {
        InitializeGenerator();
        ClearLevel();

        Vector3 currentPos = new Vector3(0f, startY, 0f);
        int platformsSpawned = 0;

        SpawnPlatform(currentPos);
        platformsSpawned++;

        while (platformsSpawned < totalPlatforms && currentPos.y > endY)
        {
            float progress = (float)platformsSpawned / totalPlatforms;
            float difficulty = difficultyProgression.Evaluate(progress) * difficultyMultiplier;

            PatternType pattern = ChooseEpicPattern();

            int generatedCount = 0;
            Vector3 nextPos = currentPos;

            switch (pattern)
            {
                case PatternType.MegaCluster:
                    generatedCount = CreateMegaCluster(currentPos, difficulty, out nextPos);
                    break;

                case PatternType.CrazySpiral:
                    generatedCount = CreateCrazySpiral(currentPos, difficulty, out nextPos);
                    break;

                case PatternType.ZigzagMadness:
                    generatedCount = CreateZigzagMadness(currentPos, difficulty, out nextPos);
                    break;

                case PatternType.FloatingBridge:
                    generatedCount = CreateFloatingBridge(currentPos, difficulty, out nextPos);
                    break;

                case PatternType.SkyscraperTower:
                    generatedCount = CreateSkyscraperTower(currentPos, difficulty, out nextPos);
                    break;

                case PatternType.DiamondFormation:
                    generatedCount = CreateDiamondFormation(currentPos, difficulty, out nextPos);
                    break;

                case PatternType.SineWave:
                    generatedCount = CreateSineWave(currentPos, difficulty, out nextPos);
                    break;

                default:
                    generatedCount = CreateSingleJump(currentPos, difficulty, out nextPos);
                    break;
            }

            platformsSpawned += generatedCount;
            currentPos = nextPos;

            if (Random.Range(0f, 1f) < chaosLevel)
            {
                ApplyMegaChaos(ref currentPos, difficulty);
            }
        }
    }

    private PatternType ChooseEpicPattern()
    {
        float roll = Random.Range(0f, 1f);

        if (roll < clusterChance) return PatternType.MegaCluster;
        roll -= clusterChance;

        if (roll < spiralChance) return PatternType.CrazySpiral;
        roll -= spiralChance;

        if (roll < zigzagChance) return PatternType.ZigzagMadness;
        roll -= zigzagChance;

        if (roll < bridgeChance) return PatternType.FloatingBridge;
        roll -= bridgeChance;

        if (roll < towerChance) return PatternType.SkyscraperTower;
        roll -= towerChance;

        if (roll < diamondChance) return PatternType.DiamondFormation;
        roll -= diamondChance;

        if (roll < waveChance) return PatternType.SineWave;

        return PatternType.SingleJump;
    }

    private int CreateMegaCluster(Vector3 center, float difficulty, out Vector3 nextPos)
    {
        int clusterSize = Random.Range(minClusterSize, maxClusterSize + 1);
        clusterSize = Mathf.RoundToInt(clusterSize * (1f + difficulty * 0.5f));

        Vector3 clusterCenter = GetNextPosition(center, difficulty);
        float clusterRadius = Random.Range(8f, 20f) * (1f + difficulty);

        List<Vector3> clusterPositions = new List<Vector3>();

        for (int i = 0; i < clusterSize; i++)
        {
            float angle = (float)i / clusterSize * Mathf.PI * 2f;
            float distance = Random.Range(clusterRadius * 0.3f, clusterRadius);

            for (int ring = 0; ring < 2; ring++)
            {
                Vector3 pos = clusterCenter + new Vector3(
                    Mathf.Cos(angle + ring * 0.5f) * distance * (1f - ring * 0.3f),
                    Random.Range(-3f, 3f) + ring * 2f,
                    0f
                );

                if (IsValidPosition(pos))
                {
                    SpawnPlatform(pos);
                    clusterPositions.Add(pos);
                }
            }
        }

        if (IsValidPosition(clusterCenter))
        {
            SpawnPlatform(clusterCenter);
            clusterPositions.Add(clusterCenter);
        }

        nextPos = clusterCenter + Vector3.down * Random.Range(clusterMinHeightGap, maxVerticalGap * 2f);
        return clusterPositions.Count;
    }

    private int CreateCrazySpiral(Vector3 start, float difficulty, out Vector3 nextPos)
    {
        Vector3 spiralStart = GetNextPosition(start, difficulty);
        int platforms = Mathf.RoundToInt(spiralPlatforms * (1f + difficulty * 0.3f));
        float radius = spiralRadius * (1f + difficulty * 0.5f);

        List<Vector3> spiralPositions = new List<Vector3>();

        for (int i = 0; i < platforms; i++)
        {
            float progress = (float)i / platforms;
            float angle = progress * Mathf.PI * 4f;
            float currentRadius = radius * (1f - progress * 0.7f);

            Vector3 pos = spiralStart + new Vector3(
                Mathf.Cos(angle) * currentRadius,
                -i * Random.Range(spiralMinHeightGap, 3f) + Mathf.Sin(angle * 2f) * 2f,
                0f
            );

            if (IsValidPosition(pos))
            {
                SpawnPlatform(pos);
                spiralPositions.Add(pos);
            }
        }

        nextPos = spiralStart + Vector3.down * (platforms * 2f + Random.Range(spiralMinHeightGap, maxVerticalGap));
        return spiralPositions.Count;
    }

    private int CreateZigzagMadness(Vector3 start, float difficulty, out Vector3 nextPos)
    {
        Vector3 zigzagStart = GetNextPosition(start, difficulty);
        int length = Mathf.RoundToInt(zigzagLength * (1f + difficulty * 0.4f));

        Vector3 currentPos = zigzagStart;
        float direction = Random.Range(0f, 1f) > 0.5f ? 1f : -1f;
        int platformCount = 0;

        for (int i = 0; i < length; i++)
        {
            Vector3 mainPos = currentPos + new Vector3(
                direction * Random.Range(minJumpDistance, maxJumpDistance) * (1f + difficulty),
                -Random.Range(zigzagMinHeightGap, maxVerticalGap),
                0f
            );

            if (IsValidPosition(mainPos))
            {
                SpawnPlatform(mainPos);
                platformCount++;

                if (i % 2 == 0 && Random.Range(0f, 1f) < 0.6f)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        Vector3 extraPos = mainPos + new Vector3(
                            Random.Range(-4f, 4f),
                            Random.Range(-2f, 2f),
                            0f
                        );

                        if (IsValidPosition(extraPos))
                        {
                            SpawnPlatform(extraPos);
                            platformCount++;
                        }
                    }
                }
            }

            currentPos = mainPos;
            direction *= -1f;
        }

        nextPos = currentPos + Vector3.down * Random.Range(zigzagMinHeightGap, maxVerticalGap);
        return platformCount;
    }

    private int CreateFloatingBridge(Vector3 start, float difficulty, out Vector3 nextPos)
    {
        Vector3 bridgeStart = GetNextPosition(start, difficulty);
        int length = Mathf.RoundToInt(bridgeLength * (1f + difficulty * 0.3f));
        float bridgeWidth = Random.Range(20f, 40f) * (1f + difficulty);

        List<Vector3> bridgePositions = new List<Vector3>();

        for (int i = 0; i < length; i++)
        {
            float progress = (float)i / (length - 1);

            Vector3 bridgePos = bridgeStart + new Vector3(
                progress * bridgeWidth - bridgeWidth * 0.5f,
                Mathf.Sin(progress * Mathf.PI) * Random.Range(5f, 12f),
                0f
            );

            if (IsValidPosition(bridgePos))
            {
                SpawnPlatform(bridgePos);
                bridgePositions.Add(bridgePos);

                if (i % 2 == 1 && Random.Range(0f, 1f) < 0.7f)
                {
                    Vector3 supportPos = bridgePos + Vector3.down * Random.Range(bridgeMinHeightGap, 8f);
                    if (IsValidPosition(supportPos))
                    {
                        SpawnPlatform(supportPos);
                        bridgePositions.Add(supportPos);
                    }
                }
            }
        }

        nextPos = bridgeStart + Vector3.down * Random.Range(bridgeMinHeightGap * 2f, maxVerticalGap * 2f);
        return bridgePositions.Count;
    }

    private int CreateSkyscraperTower(Vector3 start, float difficulty, out Vector3 nextPos)
    {
        Vector3 towerBase = GetNextPosition(start, difficulty);
        int height = Mathf.RoundToInt(towerHeight * (1f + difficulty * 0.5f));

        List<Vector3> towerPositions = new List<Vector3>();

        for (int i = 0; i < height; i++)
        {
            Vector3 mainTowerPos = towerBase + new Vector3(
                Random.Range(-2f, 2f),
                i * Random.Range(towerMinHeightGap, 6f),
                0f
            );

            if (IsValidPosition(mainTowerPos))
            {
                SpawnPlatform(mainTowerPos);
                towerPositions.Add(mainTowerPos);

                if (i % 2 == 1)
                {
                    for (int side = -1; side <= 1; side += 2)
                    {
                        Vector3 balconyPos = mainTowerPos + new Vector3(
                            side * Random.Range(4f, 8f),
                            Random.Range(-1f, 1f),
                            0f
                        );

                        if (IsValidPosition(balconyPos))
                        {
                            SpawnPlatform(balconyPos);
                            towerPositions.Add(balconyPos);
                        }
                    }
                }
            }
        }

        nextPos = towerBase + Vector3.down * Random.Range(towerMinHeightGap, maxVerticalGap);
        return towerPositions.Count;
    }

    private int CreateDiamondFormation(Vector3 start, float difficulty, out Vector3 nextPos)
    {
        Vector3 diamondCenter = GetNextPosition(start, difficulty);
        float size = Random.Range(8f, 16f) * (1f + difficulty * 0.4f);

        List<Vector3> diamondPositions = new List<Vector3>();

        Vector3[] diamondPoints = {
            diamondCenter + Vector3.up * size,
            diamondCenter + Vector3.right * size,
            diamondCenter + Vector3.down * size,
            diamondCenter + Vector3.left * size,
            diamondCenter
        };

        foreach (Vector3 point in diamondPoints)
        {
            if (IsValidPosition(point))
            {
                SpawnPlatform(point);
                diamondPositions.Add(point);
            }
        }

        for (int i = 0; i < 4; i++)
        {
            Vector3 midPoint = Vector3.Lerp(diamondPoints[i], diamondPoints[(i + 1) % 4], 0.5f);
            if (IsValidPosition(midPoint))
            {
                SpawnPlatform(midPoint);
                diamondPositions.Add(midPoint);
            }
        }

        nextPos = diamondCenter + Vector3.down * (size * 2f + Random.Range(diamondMinHeightGap, maxVerticalGap));
        return diamondPositions.Count;
    }

    private int CreateSineWave(Vector3 start, float difficulty, out Vector3 nextPos)
    {
        Vector3 waveStart = GetNextPosition(start, difficulty);
        int length = Mathf.RoundToInt(waveLength * (1f + difficulty * 0.3f));
        float waveWidth = Random.Range(25f, 50f);
        float amplitude = Random.Range(8f, 16f) * (1f + difficulty);

        List<Vector3> wavePositions = new List<Vector3>();

        for (int i = 0; i < length; i++)
        {
            float progress = (float)i / length;

            Vector3 wavePos = waveStart + new Vector3(
                progress * waveWidth - waveWidth * 0.5f,
                Mathf.Sin(progress * Mathf.PI * Random.Range(2f, 4f)) * amplitude - i * waveMinHeightGap,
                0f
            );

            if (IsValidPosition(wavePos))
            {
                SpawnPlatform(wavePos);
                wavePositions.Add(wavePos);

                if (Random.Range(0f, 1f) < 0.4f)
                {
                    Vector3 harmonic = wavePos + new Vector3(
                        0f,
                        Mathf.Cos(progress * Mathf.PI * 6f) * 3f,
                        0f
                    );

                    if (IsValidPosition(harmonic))
                    {
                        SpawnPlatform(harmonic);
                        wavePositions.Add(harmonic);
                    }
                }
            }
        }

        nextPos = waveStart + Vector3.down * (length * waveMinHeightGap + Random.Range(waveMinHeightGap, maxVerticalGap));
        return wavePositions.Count;
    }

    private int CreateSingleJump(Vector3 start, float difficulty, out Vector3 nextPos)
    {
        nextPos = GetNextPosition(start, difficulty);
        SpawnPlatform(nextPos);
        return 1;
    }

    private Vector3 GetNextPosition(Vector3 current, float difficulty)
    {
        float enhancedJumpDistance = Mathf.Lerp(minJumpDistance, maxJumpDistance, difficulty);
        float enhancedVerticalGap = Mathf.Lerp(minVerticalGap, maxVerticalGap, difficulty);

        float deltaX = Random.Range(-enhancedJumpDistance, enhancedJumpDistance);
        float deltaY = Random.Range(enhancedVerticalGap * 0.5f, enhancedVerticalGap);

        Vector3 newPos = current + new Vector3(deltaX, -deltaY, 0f);
        newPos.x = Mathf.Clamp(newPos.x, -levelWidth * 0.5f, levelWidth * 0.5f);

        return newPos;
    }

    private void ApplyMegaChaos(ref Vector3 position, float difficulty)
    {
        Vector3 chaosOffset = new Vector3(
            Random.Range(-15f, 15f) * chaosLevel * difficulty,
            Random.Range(-8f, 8f) * chaosLevel * difficulty,
            0f
        );

        position += chaosOffset;
        position.x = Mathf.Clamp(position.x, -levelWidth * 0.5f, levelWidth * 0.5f);
    }

    private void SpawnPlatform(Vector3 position)
    {
        if (platformPrefabs == null || platformPrefabs.Length == 0)
        {
            Debug.LogError("Platform prefabs not assigned or empty");
            return;
        }

        GameObject selectedPrefab = platformPrefabs[Random.Range(0, platformPrefabs.Length)];
        if (selectedPrefab == null)
        {
            Debug.LogError("Selected platform prefab is null");
            return;
        }

        GameObject platform = Instantiate(selectedPrefab, position, Quaternion.identity);

        platform.layer = LayerMask.NameToLayer("Ground");

        platforms.Add(platform);
        spawnedPositions.Add(position);
    }

    private bool IsValidPosition(Vector3 position)
    {
        if (!allowOverlaps)
        {
            foreach (Vector3 existing in spawnedPositions)
            {
                if (Vector3.Distance(position, existing) < minPlatformDistance)
                {
                    return false;
                }
            }
        }

        return position.x >= -levelWidth * 0.5f && position.x <= levelWidth * 0.5f;
    }

    private void InitializeGenerator()
    {
        if (seed == 0) seed = Random.Range(1, 999999);
        Random.InitState(seed);
        rng = new System.Random(seed);
    }

    private void ClearLevel()
    {
        foreach (GameObject platform in platforms)
        {
            if (platform != null)
            {
                if (Application.isPlaying)
                    Destroy(platform);
                else
                    DestroyImmediate(platform);
            }
        }
        platforms.Clear();
        spawnedPositions.Clear();
    }

    private int CountPatterns()
    {
        return System.Enum.GetValues(typeof(PatternType)).Length;
    }

    [ContextMenu("Generate Epic Level")]
    private void RegenerateLevel()
    {
        GenerateEpicLevel();
    }

    [ContextMenu("Random Seed")]
    private void RandomizeSeed()
    {
        seed = Random.Range(1, 999999);
        GenerateEpicLevel();
    }

    private void OnValidate()
    {
        totalPlatforms = Mathf.Max(1, totalPlatforms);
        levelWidth = Mathf.Max(10f, levelWidth);
        minJumpDistance = Mathf.Max(0.1f, minJumpDistance);
        maxJumpDistance = Mathf.Max(minJumpDistance, maxJumpDistance);
        minVerticalGap = Mathf.Max(0.1f, minVerticalGap);
        maxVerticalGap = Mathf.Max(minVerticalGap, maxVerticalGap);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 topLeft = new Vector3(-levelWidth * 0.5f, startY + 10f, 0f);
        Vector3 topRight = new Vector3(levelWidth * 0.5f, startY + 10f, 0f);
        Vector3 bottomLeft = new Vector3(-levelWidth * 0.5f, endY, 0f);
        Vector3 bottomRight = new Vector3(levelWidth * 0.5f, endY, 0f);

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}

public enum PatternType
{
    SingleJump,
    MegaCluster,
    CrazySpiral,
    ZigzagMadness,
    FloatingBridge,
    SkyscraperTower,
    DiamondFormation,
    SineWave
}