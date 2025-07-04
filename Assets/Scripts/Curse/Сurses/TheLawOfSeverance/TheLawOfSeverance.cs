using UnityEngine;

[CreateAssetMenu(fileName = "The law of severance", menuName = "Game/Curses/Gameplay/The law of severance")]
public class TheLawOfSeverance : CurseData
{

    public override void ApplyEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerState>().diesOnCollision = true;        
    }

    public override void RemoveEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerState>().diesOnCollision = false;

    }
}