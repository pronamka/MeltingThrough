using UnityEngine;

[CreateAssetMenu(fileName = "The law of severance", menuName = "Game/Curses/Gameplay/The law of severance")]
public class TheLawOfSeverance : CurseData
{
    
    public override void ApplyEffect(PlayerController player)
    {
        Debug.Log("added/////////////////////////////////////////////////////////");
    }

    public override void RemoveEffect(PlayerController player)
    {
        Debug.Log("deleted");
    }
}