using UnityEngine;

[CreateAssetMenu(fileName = "Curse of guilt", menuName = "Game/Curses/Gameplay/CurseOfGuilt")]
public class CurseOfGuilt : CurseData
{

    public override void ApplyEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement>().jumpForce *= 0.8f;
    }

    public override void RemoveEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerMovement>().jumpForce /= 0.8f;
    }
}