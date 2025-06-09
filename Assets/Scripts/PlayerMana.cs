using System;
using UnityEngine;

public class PlayerMana : MonoBehaviour
{
    [SerializeField] private float maxMana;
    [SerializeField] private float regenerationRate;
    [SerializeField] private float regenerationDelay;
    private float currentMana;

    private float timeSinceUsedMana = 0;

    private void Awake()
    {
        currentMana = maxMana;
    }

    private void Update()
    {
        timeSinceUsedMana += Time.deltaTime;
        if (currentMana == maxMana) return;

        if (timeSinceUsedMana > regenerationDelay)
        {
            currentMana = Math.Min(maxMana, currentMana + regenerationRate * Time.deltaTime);
        }
    }

    public void UseMana(float amount)
    {
        currentMana = Math.Max(0, currentMana - amount);
        timeSinceUsedMana = 0;
    }

    public bool HasEnoughMana(float amount)
    {
        return currentMana >= amount;
    }

    public float GetManaPercentage()
    {
        return currentMana / maxMana;
    }

    public void StopRegeneration()
    {
        regenerationRate = 0;
    }
}
