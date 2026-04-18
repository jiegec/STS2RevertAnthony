# RevertAnthony Mod - Developer Notes

## Architecture

This mod allows users to select which version of each card to use (current game version or an older version like v0.99.1). It uses Harmony patches to intercept card properties and behavior at runtime.

## Key Components

- **`RevertAnthony.cs`** - Main mod entry point. Loads/saves JSON config, integrates with ModConfig GUI, manages version state.
- **`SupportedCard`** (defined in `RevertAnthony.cs`) - Declares each supported card with its slug, display name, and available old versions.
- **`Patches/*.cs`** - One file per card (or shared base class patches). Contains Harmony patches.
- **`RevertAnthonyConfig.json`** - User configuration file storing per-card version choices.

## How to Add a New Card

### 1. Identify Differences

Compare the card between game versions using the decompiled sources. Ask the user where they are if not provided.

Look for differences in:
- **Constructor**: `base(energyCost, CardType, CardRarity, TargetType)`
- **`CanonicalVars`**: Dynamic variables (damage, block, draw, etc.)
- **`OnPlay()`**: Card behavior when played
- **`OnUpgrade()`**: How the card changes when upgraded
- **`ExtraHoverTips`**: Additional tooltip information
- **`HasEnergyCostX` / `CanonicalEnergyCost`**: Energy cost properties
- **`Rarity`**: Card rarity

### 2. Register the Card

Add to `SupportedCards` list in `RevertAnthony.cs`:

```csharp
new SupportedCard("card-slug", "Display Name", "v0.99.1"),
// For multiple old versions:
new SupportedCard("card-slug", "Display Name", "v0.99.1", "v0.103.1"),
```

The slug must match the card's ModelId (e.g., `borrowed-time`, `hemokinesis`).

### 3. Create Patch File

Create `Patches/{CardName}Patches.cs`:

#### Pattern A: Overridden Properties/Methods (Patch Derived Class)

Use when the card overrides the property/method in its own class:

```csharp
using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

[HarmonyPatch(typeof(CardClassName), "MethodName")]
static class CardClassName_MethodName_Patch
{
    static bool Prefix(ref ReturnType __result)
    {
        if (RevertAnthony.IsVersion("card-slug", "v0.99.1"))
        {
            __result = oldValue;
            return false; // Skip original method
        }
        return true; // Run original method
    }
}
```

#### Pattern B: Non-Overridden Properties (Patch Base Class)

Use when the property is inherited from `CardModel` and NOT overridden:

```csharp
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

[HarmonyPatch(typeof(CardModel), "PropertyName", MethodType.Getter)]
static class CardModel_PropertyName_Patch
{
    static void Postfix(CardModel __instance, ref ReturnType __result)
    {
        if (__instance is TargetCardClass && RevertAnthony.IsVersion("card-slug", "v0.99.1"))
        {
            __result = oldValue;
        }
    }
}
```

**CRITICAL**: Use string literals for protected properties (e.g., `"Rarity"`, `"CanonicalEnergyCost"`) — `nameof()` won't work across assembly boundaries.

### 4. Common Patch Types

#### Changing CanonicalVars (overridden)

```csharp
[HarmonyPatch(typeof(CardClassName), "CanonicalVars", MethodType.Getter)]
static class CardClassName_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("card-slug", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(14m, ValueProp.Move),
                new HpLossVar(2m)
            };
            return false;
        }
        return true;
    }
}
```

#### Changing OnPlay Behavior

```csharp
[HarmonyPatch(typeof(CardClassName), "OnPlay")]
static class CardClassName_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext ctx, CardPlay play, CardClassName __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("card-slug", "v0.99.1"))
            return true;

        __result = OldOnPlay(ctx, play, __instance);
        return false;
    }

    static async Task OldOnPlay(PlayerChoiceContext ctx, CardPlay play, CardClassName instance)
    {
        // Old behavior here
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await DamageCmd.Attack(instance.DynamicVars.Damage.BaseValue)
            .FromCard(instance)
            .Targeting(play.Target!);
    }
}
```

#### Changing Rarity (NOT overridden - patch base class)

```csharp
[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
static class CardModel_Rarity_Patch
{
    static void Postfix(CardModel __instance, ref CardRarity __result)
    {
        if (__instance is TargetCardClass && RevertAnthony.IsVersion("card-slug", "v0.99.1"))
        {
            __result = CardRarity.Common;
        }
    }
}
```

## Important Rules

1. **One file per card** - Keep patches organized in separate files.
2. **Use `IsVersion(slug, version)`** - Never hardcode version checks.
3. **Prefix vs Postfix**:
   - Use `Prefix(returning false)` for overridden methods to skip original
   - Use `Postfix` on base class methods to modify results
4. **Protected members** - Use string literals (e.g., `"CanonicalEnergyCost"`) instead of `nameof()`
5. **Clear canonical cache** - When a user switches versions, `ClearCanonicalCache()` resets the canonical instance so new mutable clones pick up patched values
6. **ModConfig types** - Available types: `Header`, `Dropdown`, `Toggle`, `TextInput`, `Slider`, `Button`, `Separator`, `ColorPicker`. There is NO `Description` type.

## Localization (Card Descriptions)

When a card's **description changes** between versions (like BorrowedTime), you must provide the old description text so the game displays it correctly.

### Extract Old Descriptions

Run the extraction tool from the **project root**:

```bash
python3 tools/extract_old_localizations.py \
    "/path/to/old/game/Slay the Spire 2.pck" \
    --output-dir RevertAnthony/localization
```

This:
1. Auto-detects the game version from `release_info.json` next to the PCK
2. Generates version slug automatically (`v0.99.1` → `V0991`)
3. Extracts card descriptions for all supported cards
4. Saves them to `RevertAnthony/localization/{lang}/cards.json`

The game's mod loader automatically loads these as override localization tables.

### Patch Description Property

In your patch file, override `Description`:

```csharp
[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class CardModel_Description_Patch
{
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is BorrowedTime && RevertAnthony.IsVersion("borrowed-time", "v0.99.1"))
        {
            __result = new LocString("cards", "BORROWED_TIME_V0991.description");
        }
    }
}
```

The key format is `{CARD_KEY}_{VERSION_SLUG}.description` (e.g., `BORROWED_TIME_V0991.description`).

### Export Localization Files

Add the generated JSON files to `export_presets.cfg`:

```ini
export_files=PackedStringArray(
    "res://RevertAnthony/mod_image.png",
    "res://RevertAnthony/localization/eng/cards.json",
    "res://RevertAnthony/localization/zhs/cards.json"
)
```

## Build

```bash
./build.sh
```

This:
1. Copies `sts2.dll` and `0Harmony.dll` from the game directory
2. Builds the .NET project
3. Exports the Godot PCK
4. Packages everything into `dist/`
