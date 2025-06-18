using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthbar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image emptyHealth;
    [SerializeField] private Image fullHealth;

    private void Update()
    {
        fullHealth.fillAmount = playerHealth.GetHealthPercentage();
    }
}
