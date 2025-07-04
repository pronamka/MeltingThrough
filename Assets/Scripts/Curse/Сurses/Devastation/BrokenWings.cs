using UnityEngine;

[CreateAssetMenu(fileName = "Broken wings", menuName = "Game/Curses/Gameplay/BrokenWings")]
public class BrokenWinds : CurseData
{

    public override void ApplyEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement>().dashForce *= 0.5f;
    }

    public override void RemoveEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement>().dashForce /= 0.5f;
    }
}