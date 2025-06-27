using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{

    [Header("===== VISUAL SETTINGS =====")]
    public Tilemap islandTilemap;
    public TileBase islandTile;
    public GameObject[] platformPrefabs;

    [Header("===== PLATFORM GENERATION (FIRST) =====")]
    [SerializeField, Range(50, 30000)] private int totalPlatforms = 300;
    [SerializeField] private float platformLevelWidth = 1000f;
    [SerializeField] private float platformStartY = 200f;
    [SerializeField] private float platformEndY = -300f;

    [Header("Platform Movement Settings")]
    [SerializeField, Range(3f, 15f)] private float minJumpDistance = 4f;
    [SerializeField, Range(6f, 25f)] private float maxJumpDistance = 12f;
    [SerializeField, Range(2f, 8f)] private float minVerticalDrop = 2f;
    [SerializeField, Range(4f, 12f)] private float maxVerticalDrop = 5f;
    [SerializeField, Range(2f, 8f)] private float platformSafetyRadius = 2f;

    [Header("Horizontal Movement Settings")]
    [SerializeField, Range(5f, 40f)] private float maxHorizontalShift = 20f;
    [SerializeField, Range(0f, 1f)] private float horizontalVariation = 0.7f;
    [SerializeField, Range(0f, 1f)] private float extremeShiftChance = 0.2f;

    [Header("Platform Pattern Chances")]
    [SerializeField, Range(0f, 1f)] private float straightPathChance = 0.4f;
    [SerializeField, Range(0f, 1f)] private float zigzagChance = 0.3f;
    [SerializeField, Range(0f, 1f)] private float spiralChance = 0.15f;
    [SerializeField, Range(0f, 1f)] private float bridgeChance = 0.1f;
    [SerializeField, Range(0f, 1f)] private float clusterChance = 0.05f;

    [Header("===== ISLAND GENERATION (SECOND) =====")]
    [SerializeField, Range(3, 1500)] private int islandCount = 15;
    [SerializeField, Range(3, 15)] private int minIslandWidth = 4;
    [SerializeField, Range(5, 25)] private int maxIslandWidth = 10;
    [SerializeField, Range(2, 10)] private int minIslandHeight = 3;
    [SerializeField, Range(3, 15)] private int maxIslandHeight = 7;

    [Header("Island Shape Parameters")]
    [Range(0, 1)] public float topBumpiness = 0.3f;
    [Range(0, 1)] public float bottomBumpiness = 0.3f;
    [Range(0, 1)] public float leftBumpiness = 0.3f;
    [Range(0, 1)] public float rightBumpiness = 0.3f;
    [Range(0, 0.2f)] public float holeChance = 0.05f;

    [Header("Island Placement")]
    [SerializeField] private int islandLevelMinX = -400;
    [SerializeField] private int islandLevelMaxX = 400;
    [SerializeField] private int islandLevelMinY = -50;
    [SerializeField] private int islandLevelMaxY = 150;
    [SerializeField, Range(2f, 25f)] private float minDistanceFromPlatforms = 8f;

    [Header("===== ADVANCED SETTINGS =====")]
    [SerializeField] private int generationSeed = 0;
    [SerializeField] private bool debugVisualization = false;

    [SerializeField] private AnimationCurve difficultyProgression = AnimationCurve.EaseInOut(0f, 0.2f, 1f, 1f);

    private List<Vector3> platformPositions = new List<Vector3>();
    private List<GameObject> spawnedPlatforms = new List<GameObject>();
    private List<IslandData> generatedIslands = new List<IslandData>();
    private HashSet<Vector3Int> islandTiles = new HashSet<Vector3Int>();


    public HashSet<Vector3Int> OccupiedPositions { get; private set; } = new HashSet<Vector3Int>();
    public event Action OnGenerationComplete;

    private struct IslandData
    {
        public Vector3 center;
        public Vector2 size;
        public Bounds bounds;
    }


    void Awake()
    {

        GeneratePerfectLevel();
    }

    public void GeneratePerfectLevel()
    {
        InitializeGeneration();
        ClearExistingLevel();
        GenerateCompletePlatformLayout();
        GenerateIslandsInEmptySpaces();
        OnGenerationComplete?.Invoke();

    }

    #region Platform Generation (First Phase)

    private void GenerateCompletePlatformLayout()
    {
        platformPositions.Clear();


        Vector3 currentPosition = new Vector3(0, platformStartY, 0);
        SpawnPlatformAt(currentPosition);

        int platformsCreated = 1;
        int attempts = 0;
        int maxAttempts = totalPlatforms * 3;


        while (platformsCreated < totalPlatforms && attempts < maxAttempts)
        {

            attempts++;
            float levelProgress = (float)platformsCreated / totalPlatforms;
            float difficulty = difficultyProgression.Evaluate(levelProgress);


            PlatformPattern pattern = ChoosePlatformPattern(levelProgress);
            Vector3 nextPosition;
            int newPlatforms = 0;

            switch (pattern)
            {
                case PlatformPattern.StraightPath:
                    newPlatforms = CreateStraightPath(currentPosition, difficulty, out nextPosition);
                    break;
                case PlatformPattern.ZigzagPath:
                    newPlatforms = CreateZigzagPath(currentPosition, difficulty, out nextPosition);
                    break;
                case PlatformPattern.SpiralPattern:
                    newPlatforms = CreateSpiralPattern(currentPosition, difficulty, out nextPosition);
                    break;
                case PlatformPattern.BridgePattern:
                    newPlatforms = CreateBridgePattern(currentPosition, difficulty, out nextPosition);
                    break;
                case PlatformPattern.ClusterPattern:
                    newPlatforms = CreateClusterPattern(currentPosition, difficulty, out nextPosition);
                    break;
                default:
                    newPlatforms = CreateSimpleJump(currentPosition, difficulty, out nextPosition);
                    break;
            }

            if (newPlatforms > 0)
            {
                currentPosition = nextPosition;
                platformsCreated += newPlatforms;
            }
            else
            {
                currentPosition = FindAlternativePlatformPosition(currentPosition);
            }


            if (currentPosition.y < platformEndY)

            {
                currentPosition = new Vector3(
                    UnityEngine.Random.Range(-platformLevelWidth * 0.4f, platformLevelWidth * 0.4f),
                    currentPosition.y + UnityEngine.Random.Range(20f, 40f),
                    0f
                );
            }
        }
    }

    private PlatformPattern ChoosePlatformPattern(float levelProgress)
    {
        float roll = UnityEngine.Random.value;

        if (roll < straightPathChance) return PlatformPattern.StraightPath;
        roll -= straightPathChance;

        if (roll < zigzagChance) return PlatformPattern.ZigzagPath;
        roll -= zigzagChance;

        if (roll < spiralChance) return PlatformPattern.SpiralPattern;
        roll -= spiralChance;

        if (roll < bridgeChance) return PlatformPattern.BridgePattern;
        roll -= bridgeChance;

        if (roll < clusterChance) return PlatformPattern.ClusterPattern;

        return PlatformPattern.SimpleJump;
    }

    private int CreateStraightPath(Vector3 startPos, float difficulty, out Vector3 endPos)
    {
        int pathLength = UnityEngine.Random.Range(5, 12);
        float baseDirection = UnityEngine.Random.Range(-1f, 1f);

        List<Vector3> pathPositions = new List<Vector3>();
        Vector3 currentPos = startPos;

        for (int i = 0; i < pathLength; i++)
        {
            float horizontalShift = GetHorizontalShift(baseDirection);

            Vector3 nextPos = currentPos + new Vector3(
                horizontalShift,
                -UnityEngine.Random.Range(minVerticalDrop, maxVerticalDrop),
                0f
            );

            if (IsValidPlatformPosition(nextPos))
            {
                SpawnPlatformAt(nextPos);
                pathPositions.Add(nextPos);
                currentPos = nextPos;
            }
            else
            {
                break;
            }
        }


        endPos = pathPositions.Count > 0 ? pathPositions.Last() : startPos + Vector3.down * minVerticalDrop;
        return pathPositions.Count;

    }

    private int CreateZigzagPath(Vector3 startPos, float difficulty, out Vector3 endPos)
    {
        int zigzagLength = UnityEngine.Random.Range(6, 15);
        float direction = UnityEngine.Random.value > 0.5f ? 1f : -1f;

        List<Vector3> zigzagPositions = new List<Vector3>();
        Vector3 currentPos = startPos;

        for (int i = 0; i < zigzagLength; i++)
        {

            float horizontalMove = direction * UnityEngine.Random.Range(minJumpDistance * 1.2f, maxJumpDistance * 1.5f);

            if (UnityEngine.Random.value < extremeShiftChance)
            {
                horizontalMove *= UnityEngine.Random.Range(1.5f, 2.5f);
            }

            Vector3 nextPos = currentPos + new Vector3(
                horizontalMove,
                -UnityEngine.Random.Range(minVerticalDrop, maxVerticalDrop),

                0f
            );

            if (IsValidPlatformPosition(nextPos))
            {
                SpawnPlatformAt(nextPos);
                zigzagPositions.Add(nextPos);
                currentPos = nextPos;
                direction *= -1f;
            }
            else
            {
                break;
            }
        }


        endPos = zigzagPositions.Count > 0 ? zigzagPositions.Last() : startPos + Vector3.down * minVerticalDrop;
        return zigzagPositions.Count;

    }

    private int CreateSpiralPattern(Vector3 startPos, float difficulty, out Vector3 endPos)
    {
        int spiralSteps = UnityEngine.Random.Range(8, 18);
        float radius = UnityEngine.Random.Range(15f, 35f);

        List<Vector3> spiralPositions = new List<Vector3>();
        Vector3 spiralCenter = startPos + Vector3.down * minVerticalDrop;

        for (int i = 0; i < spiralSteps; i++)
        {

            float angle = (float)i / spiralSteps * Mathf.PI * 3f;
            float currentRadius = radius * (1f - (float)i / spiralSteps * 0.4f);

            Vector3 position = spiralCenter + new Vector3(
                Mathf.Cos(angle) * currentRadius,
                -i * UnityEngine.Random.Range(2f, 5f),

                0f
            );

            if (IsValidPlatformPosition(position))
            {

                SpawnPlatformAt(position);
                spiralPositions.Add(position);
            }
        }

        endPos = spiralPositions.Count > 0 ? spiralPositions.Last() : startPos + Vector3.down * maxVerticalDrop;
        return spiralPositions.Count;

    }

    private int CreateBridgePattern(Vector3 startPos, float difficulty, out Vector3 endPos)
    {
        int bridgeLength = UnityEngine.Random.Range(8, 15);
        float bridgeWidth = UnityEngine.Random.Range(35f, 65f);

        List<Vector3> bridgePositions = new List<Vector3>();
        Vector3 bridgeStart = startPos + Vector3.down * UnityEngine.Random.Range(minVerticalDrop, maxVerticalDrop);

        for (int i = 0; i < bridgeLength; i++)
        {
            float progress = (float)i / (bridgeLength - 1);


            Vector3 position = bridgeStart + new Vector3(
                (progress - 0.5f) * bridgeWidth,
                Mathf.Sin(progress * Mathf.PI) * UnityEngine.Random.Range(3f, 8f),

                0f
            );

            if (IsValidPlatformPosition(position))
            {
                SpawnPlatformAt(position);
                bridgePositions.Add(position);
            }
        }

        endPos = bridgePositions.Count > 0 ? bridgePositions.Last() : startPos + Vector3.down * maxVerticalDrop;

        return bridgePositions.Count;
    }

    private int CreateClusterPattern(Vector3 startPos, float difficulty, out Vector3 endPos)
    {
        int clusterSize = UnityEngine.Random.Range(6, 12);
        float clusterRadius = UnityEngine.Random.Range(12f, 28f);

        List<Vector3> clusterPositions = new List<Vector3>();
        Vector3 clusterCenter = startPos + Vector3.down * UnityEngine.Random.Range(minVerticalDrop, maxVerticalDrop);

        if (IsValidPlatformPosition(clusterCenter))
        {
            SpawnPlatformAt(clusterCenter);
            clusterPositions.Add(clusterCenter);
        }

        for (int i = 0; i < clusterSize; i++)
        {
            float angle = (float)i / clusterSize * Mathf.PI * 2f + UnityEngine.Random.Range(-0.5f, 0.5f);
            float distance = clusterRadius * UnityEngine.Random.Range(0.6f, 1.2f);

            Vector3 position = clusterCenter + new Vector3(
                Mathf.Cos(angle) * distance,
                Mathf.Sin(angle) * distance * 0.4f + UnityEngine.Random.Range(-4f, 4f),

                0f
            );

            if (IsValidPlatformPosition(position))
            {

                SpawnPlatformAt(position);
                clusterPositions.Add(position);
            }
        }

        endPos = clusterCenter + Vector3.down * UnityEngine.Random.Range(maxVerticalDrop, maxVerticalDrop * 2f);
        return clusterPositions.Count;
    }

    private int CreateSimpleJump(Vector3 startPos, float difficulty, out Vector3 endPos)
    {
        float horizontalShift = GetHorizontalShift();

        endPos = startPos + new Vector3(
            horizontalShift,
            -UnityEngine.Random.Range(minVerticalDrop, maxVerticalDrop),
            0f
        );


        if (IsValidPlatformPosition(endPos))
        {
            SpawnPlatformAt(endPos);
            return 1;
        }

        return 0;
    }


    private float GetHorizontalShift(float baseDirection = 0f)
    {
        if (UnityEngine.Random.value > horizontalVariation)
        {
            if (baseDirection == 0f)
                baseDirection = UnityEngine.Random.Range(-1f, 1f);

            return baseDirection * UnityEngine.Random.Range(minJumpDistance, maxJumpDistance);
        }
        else
        {
            float direction = baseDirection != 0f ? baseDirection : (UnityEngine.Random.value > 0.5f ? 1f : -1f);
            float baseShift = direction * UnityEngine.Random.Range(minJumpDistance, maxHorizontalShift);

            if (UnityEngine.Random.value < extremeShiftChance)
            {
                baseShift *= UnityEngine.Random.Range(1.5f, 2.5f);
            }


            return baseShift;
        }

    }

    private bool IsValidPlatformPosition(Vector3 position)
    {
        float halfWidth = platformLevelWidth * 0.5f;
        if (position.x < -halfWidth || position.x > halfWidth)
            return false;

        if (position.y < platformEndY || position.y > platformStartY + 20f)
            return false;

        foreach (var existingPos in platformPositions)
        {
            if (Vector3.Distance(position, existingPos) < platformSafetyRadius)
                return false;
        }

        return true;
    }

    private Vector3 FindAlternativePlatformPosition(Vector3 currentPos)
    {
        for (int attempts = 0; attempts < 25; attempts++)
        {
            float horizontalShift = GetHorizontalShift();


            Vector3 alternative = currentPos + new Vector3(
                horizontalShift,
                -UnityEngine.Random.Range(minVerticalDrop, maxVerticalDrop * 2f),
                0f
            );

            if (IsValidPlatformPosition(alternative))
                return alternative;
        }

        return currentPos + Vector3.down * maxVerticalDrop;

    }

    #endregion

    #region Island Generation (Second Phase)

    private void GenerateIslandsInEmptySpaces()
    {
        if (islandTilemap == null) return;

        islandTilemap.ClearAllTiles();
        OccupiedPositions.Clear();
        generatedIslands.Clear();
        islandTiles.Clear();

        int successfulIslands = 0;
        int maxAttempts = islandCount * 10;

        for (int attempt = 0; attempt < maxAttempts && successfulIslands < islandCount; attempt++)
        {
            Vector3 candidatePosition = FindEmptySpaceForIsland();

            if (candidatePosition != Vector3.zero)
            {
                if (GenerateIslandAt(candidatePosition))
                {
                    successfulIslands++;
                }
            }
        }
    }

    private Vector3 FindEmptySpaceForIsland()
    {
        for (int attempts = 0; attempts < 50; attempts++)
        {
            Vector3 candidate = new Vector3(
                UnityEngine.Random.Range(islandLevelMinX + 15, islandLevelMaxX - 15),
                UnityEngine.Random.Range(islandLevelMinY + 10, islandLevelMaxY - 10),
                0f
            );

            if (IsGoodIslandPosition(candidate))
                return candidate;
        }

        return Vector3.zero;
    }

    private bool IsGoodIslandPosition(Vector3 position)
    {

        foreach (var platformPos in platformPositions)
        {
            if (Vector3.Distance(position, platformPos) < minDistanceFromPlatforms)
                return false;
        }

        foreach (var island in generatedIslands)
        {
            float safeDistance = (island.size.x + maxIslandWidth) * 0.5f + 10f;
            if (Vector3.Distance(position, island.center) < safeDistance)
                return false;
        }

        return true;
    }

    private bool GenerateIslandAt(Vector3 centerPosition)
    {
        int width = UnityEngine.Random.Range(minIslandWidth, maxIslandWidth + 1);
        int height = UnityEngine.Random.Range(minIslandHeight, maxIslandHeight + 1);

        if (centerPosition.x - width * 0.5f < islandLevelMinX ||
            centerPosition.x + width * 0.5f > islandLevelMaxX ||
            centerPosition.y - height * 0.5f < islandLevelMinY ||
            centerPosition.y + height * 0.5f > islandLevelMaxY)
        {
            return false;
        }

        IslandData island = new IslandData
        {
            center = centerPosition,
            size = new Vector2(width, height),
            bounds = new Bounds(centerPosition, new Vector3(width + 4, height + 4, 0))
        };

        if (GenerateOriginalIslandShape(centerPosition, width, height))
        {
            generatedIslands.Add(island);
            return true;
        }

        return false;

    }

    private bool GenerateOriginalIslandShape(Vector3 center, int width, int height)
    {

        int startX = Mathf.RoundToInt(center.x - width * 0.5f);
        int startY = Mathf.RoundToInt(center.y - height * 0.5f);

        int sideRange = Mathf.Max(1, height / 2);
        int topYBase = startY + height - 1;

        int leftTop = topYBase + UnityEngine.Random.Range(-sideRange, sideRange + 1);
        int leftBottom = leftTop - height + 1 + UnityEngine.Random.Range(-sideRange, sideRange + 1);
        int rightTop = topYBase + UnityEngine.Random.Range(-sideRange, sideRange + 1);
        int rightBottom = rightTop - height + 1 + UnityEngine.Random.Range(-sideRange, sideRange + 1);

        leftTop = Mathf.Clamp(leftTop, startY + 1, startY + height);
        rightTop = Mathf.Clamp(rightTop, startY + 1, startY + height);
        leftBottom = Mathf.Clamp(leftBottom, startY, leftTop - 1);
        rightBottom = Mathf.Clamp(rightBottom, startY, rightTop - 1);

        int[] topY = new int[width];
        int[] bottomY = new int[width];

        for (int x = 0; x < width; x++)

        {
            float t = width > 1 ? x / (float)(width - 1) : 0f;

            int interpTop = Mathf.RoundToInt(Mathf.Lerp(leftTop, rightTop, t));
            int interpBottom = Mathf.RoundToInt(Mathf.Lerp(leftBottom, rightBottom, t));

            if (x != 0 && x != width - 1 && UnityEngine.Random.value < topBumpiness)
                interpTop = Mathf.Clamp(interpTop + UnityEngine.Random.Range(-1, 2), interpBottom + 1, startY + height);
            if (x != 0 && x != width - 1 && UnityEngine.Random.value < bottomBumpiness)
                interpBottom = Mathf.Clamp(interpBottom + UnityEngine.Random.Range(-1, 2), startY, interpTop - 1);

            topY[x] = interpTop;
            bottomY[x] = interpBottom;
        }

        List<Vector3Int> tilesToPlace = new List<Vector3Int>();

        for (int x = 0; x < width; x++)
        {
            if (UnityEngine.Random.value < holeChance) continue;

            for (int y = bottomY[x]; y <= topY[x]; y++)
            {
                Vector3Int tilePos = new Vector3Int(startX + x, y, 0);
                Vector3 worldPos = new Vector3(tilePos.x, tilePos.y, 0);

                bool tooCloseToPlat = false;
                foreach (var platformPos in platformPositions)
                {
                    if (Vector3.Distance(worldPos, platformPos) < minDistanceFromPlatforms * 0.5f)
                    {
                        tooCloseToPlat = true;
                        break;
                    }
                }

                if (!tooCloseToPlat)
                {
                    tilesToPlace.Add(tilePos);
                }
            }
        }

        if (tilesToPlace.Count > width * height * 0.3f)
        {
            foreach (var tilePos in tilesToPlace)
            {
                islandTilemap.SetTile(tilePos, islandTile);
                OccupiedPositions.Add(tilePos);
                islandTiles.Add(tilePos);
            }
            return true;
        }

        return false;
    }

    public bool IsPositionOnIsland(Vector3 worldPosition)
    {
        if (islandTilemap == null) return false;
        Vector3Int cell = islandTilemap.WorldToCell(worldPosition);
        return OccupiedPositions.Contains(cell);
    }

    #endregion

    #region Utility Methods

    private void SpawnPlatformAt(Vector3 position)
    {
        if (platformPrefabs == null || platformPrefabs.Length == 0) return;

        GameObject selectedPrefab = platformPrefabs[UnityEngine.Random.Range(0, platformPrefabs.Length)];
        if (selectedPrefab == null) return;

        GameObject platform = Instantiate(selectedPrefab, position, Quaternion.identity);
        spawnedPlatforms.Add(platform);
        platformPositions.Add(position);
    }

    private void ClearExistingLevel()
    {
        foreach (var platform in spawnedPlatforms)
        {
            if (platform != null)
            {
                if (Application.isPlaying)
                    Destroy(platform);
                else
                    DestroyImmediate(platform);
            }
        }
        spawnedPlatforms.Clear();
        platformPositions.Clear();

        if (islandTilemap != null)
            islandTilemap.ClearAllTiles();

        generatedIslands.Clear();
        islandTiles.Clear();
        OccupiedPositions.Clear();
    }

    private void InitializeGeneration()
    {
        if (generationSeed == 0)
            generationSeed = UnityEngine.Random.Range(1, 999999);

        UnityEngine.Random.InitState(generationSeed);
    }

    #endregion

    #region Public Interface

    [ContextMenu("Generate Perfect Level")]
    public void RegenerateLevel()
    {
        GeneratePerfectLevel();
    }

    [ContextMenu("Random Seed")]
    public void RandomizeSeed()
    {
        generationSeed = UnityEngine.Random.Range(1, 999999);
        GeneratePerfectLevel();
    }

    #endregion

    #region Debug Visualization

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {

        if (!debugVisualization) return;

        Gizmos.color = Color.green;
        foreach (var pos in platformPositions)
        {
            Gizmos.DrawWireSphere(pos, platformSafetyRadius);
        }

        Gizmos.color = Color.yellow;
        for (int i = 1; i < platformPositions.Count; i++)
        {
            Gizmos.DrawLine(platformPositions[i-1], platformPositions[i]);
        }

        Gizmos.color = Color.red;
        foreach (var island in generatedIslands)
        {
            Gizmos.DrawWireCube(island.center, new Vector3(island.size.x, island.size.y, 0));
        }

        Gizmos.color = Color.blue;
        Vector3 platformBounds = new Vector3(platformLevelWidth, platformStartY - platformEndY, 0);
        Vector3 platformCenter = new Vector3(0, (platformStartY + platformEndY) * 0.5f, 0);
        Gizmos.DrawWireCube(platformCenter, platformBounds);


        Gizmos.color = Color.cyan;
        Vector3 islandBounds = new Vector3(islandLevelMaxX - islandLevelMinX, islandLevelMaxY - islandLevelMinY, 0);
        Vector3 islandCenter = new Vector3((islandLevelMaxX + islandLevelMinX) * 0.5f, (islandLevelMaxY + islandLevelMinY) * 0.5f, 0);
        Gizmos.DrawWireCube(islandCenter, islandBounds);
    }
#endif

    #endregion
}

public enum PlatformPattern
{
    SimpleJump,
    StraightPath,
    ZigzagPath,
    SpiralPattern,
    BridgePattern,
    ClusterPattern
}