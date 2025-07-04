using UnityEngine;

[CreateAssetMenu(fileName = "Chains of time", menuName = "Game/Curses/Stat/ChainsOfTime")]
public class ChainsOfTime : CurseData
{

    public override void ApplyEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement>().speed *= 0.75f;
    }

    public override void RemoveEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement>().speed /= 0.75f;
    }
}