using UnityEngine;
using UnityEngine.UI;

public class PlayerManabar : MonoBehaviour
{
    [SerializeField] private PlayerMana playerMana;
    [SerializeField] private Image emptyMana;
    [SerializeField] private Image fullMana;

    private void Update()
    {
        fullMana.fillAmount = playerMana.GetManaPercentage();
    }
}
