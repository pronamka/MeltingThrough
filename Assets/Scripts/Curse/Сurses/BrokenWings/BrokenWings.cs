using UnityEngine;

[CreateAssetMenu(fileName = "Broken wings", menuName = "Game/Curses/Gameplay/BrokenWings")]
public class BrokenWings : CurseData
{

    public override void ApplyEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement>().dashForce *= 0.65f;
    }

    public override void RemoveEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement>().dashForce /= 0.65f;
    }
}