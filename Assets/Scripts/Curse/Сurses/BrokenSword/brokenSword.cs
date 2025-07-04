using UnityEngine;

[CreateAssetMenu(fileName = "Broken sword", menuName = "Game/Curses/Gameplay/BrokenSword")]
public class BrokenSword : CurseData
{

    public override void ApplyEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerAttack>().primaryAttackDamage *= 0.7f;
    }

    public override void RemoveEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerAttack>().primaryAttackDamage /= 0.7f;
    }
}