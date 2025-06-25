using UnityEngine;

public class CurseDrop : MonoBehaviour
{
    [SerializeField] private float dropChance = 0.7f; 
    [SerializeField] private float minCurseValue = 0.3f;
    [SerializeField] private float maxCurseValue = 2f;
    [SerializeField] private GameObject cursePickupPrefab;

    private Health enemyHealth;
    private bool hasDropped = false;

    private void Awake()
    {
        enemyHealth = GetComponent<Health>();

        
        if (cursePickupPrefab == null)
        {
            CreateCursePickupPrefab();
        }
    }

    private void Update()
    {
        if (enemyHealth.isDead && !hasDropped)
        {
            TryDropCurse();
            hasDropped = true;
        }
    }

    private void CreateCursePickupPrefab()
    {
        
        cursePickupPrefab = new GameObject("CursePickup");
        cursePickupPrefab.AddComponent<CursePickup>();

        
        cursePickupPrefab.SetActive(false);
    }

    private void TryDropCurse()
    {
        if (Random.Range(0f, 1f) <= dropChance)
        {
            DropCurse();
        }
    }

    private void DropCurse()
    {
        
        CurseType[] types = (CurseType[])System.Enum.GetValues(typeof(CurseType));
        CurseType randomType = types[Random.Range(0, types.Length)];

        
        float curseValue = Random.Range(minCurseValue, maxCurseValue);

        
        Vector3 dropPosition = transform.position + Vector3.up * 0.5f;
        GameObject curseObject = Instantiate(cursePickupPrefab, dropPosition, Quaternion.identity);
        curseObject.SetActive(true);

        
        CursePickup pickup = curseObject.GetComponent<CursePickup>();
        pickup.Initialize(randomType, curseValue);

        Debug.Log($"Враг дропнул проклятие: {randomType} (значение: {curseValue})");
    }
}