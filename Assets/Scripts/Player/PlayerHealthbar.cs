using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthbar : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image emptyHealth;
    [SerializeField] private Image fullHealth;

    private void Update()
    {
        fullHealth.fillAmount = playerHealth.GetHealthPercentage();
    }
}
