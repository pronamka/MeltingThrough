using UnityEngine;

public enum CurseCategory
{
    Stat,        // Влияет на статы игрока
    Visual,      // Визуальные эффекты
    UI,          // Эффекты интерфейса
    Gameplay,    // Игровые механики
    Special      // Комплексные эффекты
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