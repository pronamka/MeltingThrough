using UnityEngine;

public abstract class AbstractCurse
{
    protected GameObject Player;

    public float Intensity;
    public float Value;

    public abstract string Name { get; }
    public abstract string Description { get; }

    public abstract CurseType Type { get;  }

    public void SetUp(GameObject playerObject)
    {
        Player = playerObject;
    }

    public abstract void Activate();

    public abstract void Deactivate();

}
