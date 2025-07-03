using UnityEngine;

public enum CurseCategory
{
    Stat,        // ������ �� ����� ������
    Visual,      // ���������� �������
    UI,          // ������� ����������
    Gameplay,    // ������� ��������
    Special      // ����������� �������
}

public enum CurseType
{
    // Stat effects
    Health,
    Speed,
    Damage,
    Defense,

    // Visual effects
    Blur,
    Distortion,
    ColorFilter,
    Vignette,

    // UI effects
    InvertedControls,
    HiddenUI,
    FakeUI,

    // Gameplay effects
    GravityFlip,
    TimeDistortion,
    RandomTeleport,

    // Special
    Composite
}