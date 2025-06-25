using UnityEngine;

[System.Serializable]
public enum CurseType
{
    SlowMovement,      
    WeakAttack,        
    LowHealth,         
    SlowMana,          
    HeavyFall,         
    ShortJump          
}

[System.Serializable]
public class Curse
{
    public CurseType type;
    public float value;
    public float intensity;
    public string name;
    public string description;

    public Curse(CurseType type, float value, float intensity, string name, string description)
    {
        this.type = type;
        this.value = value;
        this.intensity = intensity;
        this.name = name;
        this.description = description;
    }
}