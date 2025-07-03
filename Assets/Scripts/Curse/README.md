# Curse System Documentation

## Overview
The curse system provides a flexible framework for applying temporary or permanent negative effects to players in the game. It supports stat modifications, visual effects, and complex composite curses.

## System Architecture

### Core Components

1. **CurseData** - Base ScriptableObject class for all curses
2. **CurseManager** - Singleton that manages active curses
3. **PlayerController** - Handles player stats modifications
4. **VisualEffectManager** - Handles visual effects
5. **CursePickup** - Handles curse pickup mechanics

### Curse Categories

- **Stat** - Affects player statistics (health, speed, damage, defense)
- **Visual** - Applies visual effects (blur, color filters, vignette)
- **UI** - Affects user interface
- **Gameplay** - Affects game mechanics
- **Special** - Composite curses that combine multiple effects

## Implemented Curses

### Stat Curses

#### WeakenedCurse
- **Category**: Stat
- **Type**: Health
- **Effect**: Reduces maximum health
- **File**: `WeakenedCurse.cs`

#### SlowCurse
- **Category**: Stat
- **Type**: Speed
- **Effect**: Reduces movement speed
- **File**: `SlowCurse.cs`

#### BluntedCurse
- **Category**: Stat
- **Type**: Damage
- **Effect**: Reduces damage output
- **File**: `BluntedCurse.cs`

#### DefenseBreakCurse
- **Category**: Stat
- **Type**: Defense
- **Effect**: Reduces defense rating
- **File**: `DefenseBreakCurse.cs`

### Visual Curses

#### RedVisionCurse
- **Category**: Visual
- **Type**: ColorFilter
- **Effect**: Applies red color overlay
- **File**: `RedVisionCurse.cs`

#### DarkVisionCurse
- **Category**: Visual
- **Type**: Vignette
- **Effect**: Applies dark vignette effect
- **File**: `DarkVisionCurse.cs`

#### BlindEye (Existing)
- **Category**: Visual
- **Type**: Blur
- **Effect**: Applies edge blur effect
- **File**: `BlindEye.cs`

### Special Curses

#### NightmareCurse
- **Category**: Special
- **Type**: Composite
- **Effect**: Combines health/speed reduction with visual effects
- **File**: `NightmareCurse.cs`

## Usage Guide

### Creating New Curses

1. Create a new class inheriting from `CurseData`
2. Add the `[CreateAssetMenu]` attribute
3. Override the appropriate methods:
   - `ApplyEffect(PlayerController player)` - for stat effects
   - `RemoveEffect(PlayerController player)` - for removing stat effects
   - `ApplyVisualEffect(VisualEffectManager visualManager)` - for visual effects
   - `RemoveVisualEffect(VisualEffectManager visualManager)` - for removing visual effects

Example:
```csharp
[CreateAssetMenu(fileName = "New Curse", menuName = "Game/Curses/Stat/New Curse")]
public class NewCurse : CurseData
{
    public float effectValue = 1f;
    
    private void OnEnable()
    {
        category = CurseCategory.Stat;
        curseType = CurseType.Health;
    }
    
    public override void ApplyEffect(PlayerController player)
    {
        player.ModifyMaxHealth(-effectValue);
    }
    
    public override void RemoveEffect(PlayerController player)
    {
        player.ModifyMaxHealth(effectValue);
    }
}
```

### Applying Curses

```csharp
// Apply a curse through the CurseManager
CurseManager.Instance.ApplyCurse(curseData);

// Remove a specific curse
CurseManager.Instance.RemoveCurse(curseData);

// Remove all curses
CurseManager.Instance.RemoveAllCurses();
```

### Testing

Three testing components are provided:

1. **CurseSystemTester** - Basic curse cycling tester
2. **CurseIntegrationTester** - Advanced integration testing
3. **CurseSystemDemo** - Interactive demonstration

## PlayerController API

The following methods are available for stat modification:

- `ModifyMaxHealth(float modifier)` - Modifies maximum health
- `ModifySpeed(float modifier)` - Modifies movement speed
- `ModifyDamage(float modifier)` - Modifies damage output
- `ModifyDefense(float modifier)` - Modifies defense rating
- `ResetStats()` - Resets all stats to base values

## VisualEffectManager API

The following methods are available for visual effects:

- `ApplyEdgeBlur(float intensity, float edgeWidth, AnimationCurve falloff)` - Applies edge blur
- `RemoveEdgeBlur()` - Removes edge blur
- `ApplyColorFilter(Color filterColor, float intensity)` - Applies color filter
- `ApplyVignette(float intensity, float smoothness)` - Applies vignette effect
- `RemoveAllEffects()` - Removes all visual effects

## Configuration

### CurseManager Settings

- `curseDropChance` - Probability of curse drops (0-1)
- `availableCurses` - Array of available curses for dropping
- `debugMode` - Enable debug logging and UI

### CurseData Settings

- `curseName` - Display name
- `description` - Curse description
- `category` - Curse category (Stat, Visual, UI, Gameplay, Special)
- `curseType` - Specific curse type
- `value` - Effect magnitude
- `duration` - Effect duration (-1 for permanent)
- `stackable` - Whether the curse can stack

## Best Practices

1. **Always check for null references** in ApplyEffect/RemoveEffect methods
2. **Use appropriate categories and types** for proper system handling
3. **Implement both Apply and Remove methods** for proper cleanup
4. **Test curses thoroughly** using the provided testing components
5. **Use descriptive names** for curse classes and ScriptableObject assets
6. **Set appropriate default values** in OnEnable() method

## Troubleshooting

### Common Issues

1. **Curse not applying**: Check if CurseManager instance exists and curse is not null
2. **Stats not changing**: Verify PlayerController is properly referenced
3. **Visual effects not working**: Ensure VisualEffectManager is in the scene
4. **Curses not removing**: Check if RemoveEffect method is properly implemented

### Debug Mode

Enable debug mode in CurseManager to see detailed logging and debug UI with:
- Active curses count
- Available curses
- Drop chances
- Test controls

### Testing

Use the provided testing components to verify curse functionality:
- CurseSystemDemo for interactive testing
- CurseIntegrationTester for automated testing
- CurseSystemTester for basic functionality testing