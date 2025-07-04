using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float spawnCooldown = 3f;
    [SerializeField] private int maxEnemies = 8;
    [SerializeField] private float spawnDistance = 20f;

    [Header("Cleaner Settings")]
    [SerializeField] private float checkInterval = 1.0f;
    [SerializeField] private float screenDistanceMultiplier = 1.05f;

    [Header("Detection Layers")]
    [SerializeField] private LayerMask groundLayer = 1 << 6;
    [SerializeField] private LayerMask obstacleLayer = -1;
    [SerializeField] private float groundCheckDistance = 50f;
    [SerializeField] private float spawnHeightAboveGround = 2f;

    [Header("Safety Settings")]
    [SerializeField] private float safetyMargin = 1f;
    [SerializeField] private int maxSpawnAttempts = 50;

    private Camera mainCamera;
    private Transform player;

    private void Start()
    {
        mainCamera = Camera.main;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        InvokeRepeating(nameof(CleanFarEnemies), checkInterval, checkInterval);

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnCooldown);

            if (CountLivingEnemies() < maxEnemies)
            {
                TrySpawnSafely();
            }
        }
    }

    public void CleanFarEnemies()
    {
        if (player == null || mainCamera == null) return;

        float vertSize = mainCamera.orthographicSize;
        float horizSize = vertSize * mainCamera.aspect;

        float maxDist = Mathf.Sqrt(vertSize * vertSize + horizSize * horizSize) * screenDistanceMultiplier;

        EnemyState[] enemies = FindObjectsOfType<EnemyState>();
        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy.IsDead() || enemy.tag == "Boss") continue;

            float dist = Vector2.Distance(enemy.transform.position, player.position);
            if (dist >= maxDist)
            {
                Destroy(enemy.gameObject);
            }
        }
    }

    private void TrySpawnSafely()
    {
        if (enemyPrefabs.Length == 0) return;

        GameObject selectedPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Vector2 enemySize = GetPrefabSize(selectedPrefab);

        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Vector3 candidatePos = GetOffscreenPosition();
            Vector3 safeSpawnPos = FindSafeSpawnPosition(candidatePos, enemySize);

            if (safeSpawnPos != Vector3.zero &&
                !IsPositionVisible(safeSpawnPos) &&
                IsPositionTrulySafe(safeSpawnPos, enemySize))
            {
                SpawnEnemyAt(safeSpawnPos, selectedPrefab);
                return;
            }
        }
    }

    private Vector3 FindSafeSpawnPosition(Vector3 startPos, Vector2 enemySize)
    {
        RaycastHit2D groundHit = Physics2D.Raycast(startPos, Vector2.down, groundCheckDistance, groundLayer);

        if (groundHit.collider == null)
        {
            return Vector3.zero;
        }

        float enemyHeight = enemySize.y;
        float totalHeightNeeded = enemyHeight / 2f + spawnHeightAboveGround;
        Vector3 potentialSpawnPos = groundHit.point + Vector2.up * totalHeightNeeded;

        Vector3 ceilingCheckStart = potentialSpawnPos + Vector3.up * (enemyHeight / 2f);
        RaycastHit2D ceilingHit = Physics2D.Raycast(ceilingCheckStart, Vector2.up, enemyHeight + safetyMargin, obstacleLayer);

        if (ceilingHit.collider != null)
        {
            return Vector3.zero;
        }

        return potentialSpawnPos;
    }

    private bool IsPositionTrulySafe(Vector3 spawnPos, Vector2 enemySize)
    {
        Vector2 checkSize = enemySize + Vector2.one * safetyMargin;

        Collider2D centerCheck = Physics2D.OverlapPoint(spawnPos, obstacleLayer);
        if (centerCheck != null)
        {
            return false;
        }

        RaycastHit2D boxCheck = Physics2D.BoxCast(spawnPos, checkSize, 0f, Vector2.zero, 0.01f, obstacleLayer);
        if (boxCheck.collider != null)
        {
            return false;
        }

        Vector3[] corners = {
            spawnPos + new Vector3(-checkSize.x/2, checkSize.y/2, 0),
            spawnPos + new Vector3(checkSize.x/2, checkSize.y/2, 0),
            spawnPos + new Vector3(-checkSize.x/2, -checkSize.y/2, 0),
            spawnPos + new Vector3(checkSize.x/2, -checkSize.y/2, 0)
        };

        foreach (Vector3 corner in corners)
        {
            Collider2D cornerCheck = Physics2D.OverlapPoint(corner, obstacleLayer);
            if (cornerCheck != null)
            {
                return false;
            }
        }

        if (IsAnotherEnemyNearby(spawnPos, enemySize))
        {
            return false;
        }

        return true;
    }

    private bool IsAnotherEnemyNearby(Vector3 spawnPos, Vector2 enemySize)
    {
        float checkRadius = Mathf.Max(enemySize.x, enemySize.y) + safetyMargin;
        Collider2D[] nearby = Physics2D.OverlapCircleAll(spawnPos, checkRadius);

        foreach (Collider2D col in nearby)
        {
            if (col.GetComponent<EnemyState>() != null)
            {
                return true;
            }
        }

        return false;
    }

    private Vector2 GetPrefabSize(GameObject prefab)
    {
        Collider2D mainCollider = prefab.GetComponent<Collider2D>();
        if (mainCollider != null && !mainCollider.isTrigger)
        {
            return mainCollider.bounds.size;
        }

        Collider2D[] colliders = prefab.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            if (!col.isTrigger)
            {
                return col.bounds.size;
            }
        }

        return new Vector2(1f, 2f);
    }

    private Vector3 GetOffscreenPosition()
    {
        Vector3 basePos = player != null ? player.position : mainCamera.transform.position;

        float angle;
        if (Random.value < 0.7f)
        {
            angle = Random.value < 0.5f ? Random.Range(120f, 240f) : Random.Range(300f, 60f);
        }
        else
        {
            angle = Random.Range(60f, 120f);
        }

        angle *= Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        return basePos + (Vector3)(direction * spawnDistance);
    }

    private bool IsPositionVisible(Vector3 worldPos)
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(worldPos);
        float buffer = 0.2f;

        return viewportPos.x >= -buffer && viewportPos.x <= 1 + buffer &&
               viewportPos.y >= -buffer && viewportPos.y <= 1 + buffer &&
               viewportPos.z > 0;
    }

    private void SpawnEnemyAt(Vector3 position, GameObject prefab)
    {
        GameObject enemy = Instantiate(prefab, position, Quaternion.identity);

        StartCoroutine(ValidateSpawnedEnemy(enemy, position));
    }

    private IEnumerator ValidateSpawnedEnemy(GameObject enemy, Vector3 originalPos)
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        if (enemy == null) yield break;

        EnemyState enemyState = enemy.GetComponent<EnemyState>();
        if (enemyState == null) yield break;

        yield return new WaitForSeconds(0.5f);

        if (!enemyState.IsGrounded() && enemy.tag != "Boss")
        {
            Destroy(enemy);
        }
    }

    private int CountLivingEnemies()
    {
        EnemyState[] enemies = FindObjectsOfType<EnemyState>();
        int count = 0;

        foreach (EnemyState enemy in enemies)
        {
            if (enemy != null && !enemy.IsDead())
            {
                count++;
            }
        }

        return count;
    }

   
    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Vector3 center = player.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, spawnDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(center, center + Vector3.up * spawnHeightAboveGround);
    }
}