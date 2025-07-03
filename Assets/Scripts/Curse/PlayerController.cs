using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float moveSpeed = 5f;
    public float damage = 10f;
    public float defense = 0f;

    private float baseSpeed;
    private float baseDamage;
    private float baseDefense;
    private float baseMaxHealth;

    void Start()
    {
        // ��������� ������� ��������
        baseSpeed = moveSpeed;
        baseDamage = damage;
        baseDefense = defense;
        baseMaxHealth = maxHealth;
        currentHealth = maxHealth;
    }

    public void ModifySpeed(float modifier)
    {
        moveSpeed += modifier;
        Debug.Log($"Speed modified by {modifier}. New speed: {moveSpeed}");
    }

    public void ModifyDamage(float modifier)
    {
        damage += modifier;
        Debug.Log($"Damage modified by {modifier}. New damage: {damage}");
    }

    public void ModifyDefense(float modifier)
    {
        defense += modifier;
        Debug.Log($"Defense modified by {modifier}. New defense: {defense}");
    }

    public void ModifyMaxHealth(float modifier)
    {
        maxHealth += modifier;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        Debug.Log($"Max health modified by {modifier}. New max health: {maxHealth}");
    }

    public void ResetStats()
    {
        moveSpeed = baseSpeed;
        damage = baseDamage;
        defense = baseDefense;
        maxHealth = baseMaxHealth;
        currentHealth = maxHealth;
    }
}