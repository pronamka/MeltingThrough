using UnityEngine;

[CreateAssetMenu(fileName = "Echo of doubt", menuName = "Game/Curses/Stat/EchoOfDoubt")]
public class EchoOfDoubt : CurseData
{

    public override void ApplyEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMana>().regenerationRate *= 0.7f;
    }

    public override void RemoveEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMana>().regenerationRate /= 0.7f;
    }
}