using UnityEngine;

/*public class PlayerAttackModifier : MonoBehaviour
{
    private PlayerAttack playerAttack;
    private float basePrimaryDamage = 25f; 

    private void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();
    }

    private void Update()
    {
        if (CurseManager.Instance != null)
        {
            ApplyDamageModifier(CurseManager.Instance.GetDamageModifier());
        }
    }

    private void ApplyDamageModifier(float modifier)
    {
        float newDamage = basePrimaryDamage * modifier;

 
        var damageField = typeof(PlayerAttack).GetField("primaryAttackDamage",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (damageField != null)
        {
            damageField.SetValue(playerAttack, newDamage);
        }
    }
}*/