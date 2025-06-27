using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CurseFactory
{
    private Dictionary<int, (float, float)> intensityMap = new Dictionary<int, (float, float)>()
    {
        {1, (0.2f, 0.5f)},
        {2, (0.5f, 0.7f)},
        {3, (0.7f, 1f)},
        {4, (1f, 1.5f)},
        {5, (1.5f, 2f)}
    };

    private Dictionary<int, float> valueMap = new Dictionary<int, float>()
    {
        {1, 1},
        {2, 2},
        {3, 3},
        {4, 4},
        {5, 5}
    };

    private GameObject player;

    public CurseFactory(GameObject playerObject)
    {
        player = playerObject;
    }

    public AbstractCurse createCurse(CurseType type, int level)
    {
        (float, float) intensityRange = intensityMap[level];
        float intensity = Random.Range(intensityRange.Item1, intensityRange.Item2);

        float value = valueMap[level];

        AbstractCurse curse = type switch
        {
            CurseType.SlowMovement => new SlownessCurse(),
            CurseType.WeakAttack => new WeaknessCurse(),
            CurseType.LowHealth => new LowHealthCurse(),
            CurseType.ShortJump => new ShortJumpCurse(),
            CurseType.SlowMana => new ManaRegenerationCurse(),
            CurseType.HeavyFall => new HeavyFallCurse()
        };

        curse.Intensity = intensity;
        curse.Value = value;
        curse.SetUp(player);

        return curse;
    }
}
