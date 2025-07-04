using UnityEngine;

[CreateAssetMenu(fileName = "Devastation", menuName = "Game/Curses/Stat/Devastation")]
public class Devastation : CurseData
{

    public override void ApplyEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMana>().maxMana *= 0.7f;
    }

    public override void RemoveEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMana>().maxMana /= 0.7f;
    }
}