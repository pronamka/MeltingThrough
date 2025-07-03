using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class EnemyDropSystem : MonoBehaviour
{
    [Header("Drop Settings")]
    [SerializeField] private float dropChance = 0.3f;
    [SerializeField] private float curseDrop Chance = 0.1f;
    [SerializeField] private GameObject cursePickupPrefab;
    
    [Header("Drop Tables")]
    [SerializeField] private List<CurseData> availableCurses = new List<CurseData>();
    [SerializeField] private List<GameObject> commonDrops = new List<GameObject>();
    
    private static EnemyDropSystem instance;
    
    public static EnemyDropSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EnemyDropSystem>();
                if (instance == null)
                {
                    GameObject go = new GameObject("EnemyDropSystem");
                    instance = go.AddComponent<EnemyDropSystem>();
                }
            }
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    public void HandleEnemyDeath(GameObject enemy, Vector3 deathPosition)
    {
        // Определяем, что именно дропнуть
        if (ShouldDropCurse())
        {
            DropCurse(deathPosition);
        }
        else if (ShouldDropCommonItem())
        {
            DropCommonItem(deathPosition);
        }
    }
    
    private bool ShouldDropCurse()
    {
        return Random.Range(0f, 1f) < curseDrop Chance;
    }
    
    private bool ShouldDropCommonItem()
    {
        return Random.Range(0f, 1f) < dropChance;
    }
    
    private void DropCurse(Vector3 position)
    {
        if (availableCurses.Count == 0 || cursePickupPrefab == null)
        {
            return;
        }
        
        // Выбираем случайное проклятие
        CurseData curseToDropа = availableCurses[Random.Range(0, availableCurses.Count)];
        
        // Создаем объект подбора
        GameObject pickupObject = Instantiate(cursePickupPrefab, position, Quaternion.identity);
        
        // Инициализируем подбор с выбранным проклятием
        CursePickup pickup = pickupObject.GetComponent<CursePickup>();
        if (pickup != null)
        {
            pickup.Initialize(curseToDropа);
        }
        
        // Добавляем небольшую физику для более натурального падения
        Rigidbody2D rb = pickupObject.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = pickupObject.AddComponent<Rigidbody2D>();
        }
        
        // Применяем случайную силу для разброса
        Vector2 randomForce = new Vector2(
            Random.Range(-3f, 3f),
            Random.Range(2f, 5f)
        );
        rb.AddForce(randomForce, ForceMode2D.Impulse);
        
        // Через некоторое время отключаем физику
        StartCoroutine(DisablePhysicsAfterTime(rb, 2f));
    }
    
    private void DropCommonItem(Vector3 position)
    {
        if (commonDrops.Count == 0)
        {
            return;
        }
        
        // Выбираем случайный обычный предмет
        GameObject itemToDrop = commonDrops[Random.Range(0, commonDrops.Count)];
        
        // Создаем объект
        GameObject droppedItem = Instantiate(itemToDrop, position, Quaternion.identity);
        
        // Добавляем физику
        Rigidbody2D rb = droppedItem.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = droppedItem.AddComponent<Rigidbody2D>();
        }
        
        // Применяем случайную силу
        Vector2 randomForce = new Vector2(
            Random.Range(-2f, 2f),
            Random.Range(1f, 3f)
        );
        rb.AddForce(randomForce, ForceMode2D.Impulse);
    }
    
    private System.Collections.IEnumerator DisablePhysicsAfterTime(Rigidbody2D rb, float time)
    {
        yield return new WaitForSeconds(time);
        
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
    
    public void AddAvailableCurse(CurseData curse)
    {
        if (!availableCurses.Contains(curse))
        {
            availableCurses.Add(curse);
        }
    }
    
    public void RemoveAvailableCurse(CurseData curse)
    {
        availableCurses.Remove(curse);
    }
    
    public void SetDropChance(float newDropChance)
    {
        dropChance = Mathf.Clamp01(newDropChance);
    }
    
    public void SetCurseDropChance(float newCurseDropChance)
    {
        curseDrop Chance = Mathf.Clamp01(newCurseDropChance);
    }
}