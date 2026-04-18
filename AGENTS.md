# RevertAnthony Mod - Developer Notes

## Architecture

This mod allows users to select which version of each card to use (current game version or an older version like v0.99.1). It uses Harmony patches to intercept card properties and behavior at runtime.

## Key Components

- **`RevertAnthony.cs`** - Main mod entry point. Loads/saves JSON config, integrates with ModConfig GUI, manages version state.
- **`SupportedCard`** (defined in `RevertAnthony.cs`) - Declares each supported card with its slug, display name, and available old versions.
- **`Patches/*.cs`** - One file per card. Contains Harmony patches.
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
new SupportedCard("card-slug", "CHARACTER", "v0.99.1"),
```

The slug must match the card's ModelId (e.g., `borrowed-time`, `hemokinesis`).

### 3. Create Patch File

Create `Patches/{CardName}Patches.cs`.

**CRITICAL RULE**: Check if the property/method is **overridden** in the current version:
- **OVERRIDDEN**: Patch the derived class with `Prefix(returning false)`
- **NOT OVERRIDDEN**: Patch `CardModel` with `Postfix` + `if (__instance is CardName)`

#### Overridden Properties/Methods

Use when the card overrides in its own class (e.g., `CanonicalVars`, `OnPlay`, `ExtraHoverTips`):

```csharp
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
        return true;
    }
}
```

#### Non-Overridden Properties

Use when inherited from `CardModel` and NOT overridden (e.g., `Rarity`, `CanonicalEnergyCost`, `TargetType`, `Type`, `Description`, `ShouldGlowGoldInternal`):

```csharp
[HarmonyPatch(typeof(CardModel), "PropertyName", MethodType.Getter)]
static class CardName_PropertyName_Patch
{
    static void Postfix(CardModel __instance, ref ReturnType __result)
    {
        if (__instance is CardClassName && RevertAnthony.IsVersion("card-slug", "v0.99.1"))
        {
            __result = oldValue;
        }
    }
}
```

#### Special Case: Methods Removed in Current Version

If a method was **removed** from the derived class in current version (e.g., `Begone.OnUpgrade`, `Dominate.ShouldGlowGoldInternal`), patch the **base class** instead:

```csharp
[HarmonyPatch(typeof(CardModel), "OnUpgrade")]
static class CardName_OnUpgrade_Patch
{
    static void Postfix(CardModel __instance)
    {
        if (__instance is CardClassName && RevertAnthony.IsVersion("card-slug", "v0.99.1"))
        {
            // Old behavior
            __instance.DynamicVars.Damage.UpgradeValueBy(1m);
        }
    }
}
```

**CRITICAL**: Use string literals for protected properties (e.g., `"Rarity"`, `"CanonicalEnergyCost"`) — `nameof()` won't work across assembly boundaries.

### 4. Common Patch Types

#### CanonicalVars (overridden)

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

#### OnPlay (overridden)

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

#### Energy Cost (NOT overridden - patch base class)

```csharp
[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
static class CardName_EnergyCost_Patch
{
    static void Postfix(CardModel __instance, ref int __result)
    {
        if (__instance is CardClassName && RevertAnthony.IsVersion("card-slug", "v0.99.1"))
        {
            __result = 0; // Old cost
        }
    }
}
```

#### Rarity (NOT overridden - patch base class)

```csharp
[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
static class CardName_Rarity_Patch
{
    static void Postfix(CardModel __instance, ref CardRarity __result)
    {
        if (__instance is CardClassName && RevertAnthony.IsVersion("card-slug", "v0.99.1"))
        {
            __result = CardRarity.Common;
        }
    }
}
```

#### Target Type (NOT overridden - patch base class)

```csharp
[HarmonyPatch(typeof(CardModel), "TargetType", MethodType.Getter)]
static class CardName_TargetType_Patch
{
    static void Postfix(CardModel __instance, ref TargetType __result)
    {
        if (__instance is CardClassName && RevertAnthony.IsVersion("card-slug", "v0.99.1"))
        {
            __result = TargetType.AnyEnemy;
        }
    }
}
```

#### Card Type (NOT overridden - patch base class)

```csharp
[HarmonyPatch(typeof(CardModel), "Type", MethodType.Getter)]
static class CardName_CardType_Patch
{
    static void Postfix(CardModel __instance, ref CardType __result)
    {
        if (__instance is CardClassName && RevertAnthony.IsVersion("card-slug", "v0.99.1"))
        {
            __result = CardType.Attack;
        }
    }
}
```

#### Description (NOT overridden - patch base class)

```csharp
[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class CardName_Description_Patch
{
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is CardClassName && RevertAnthony.IsVersion("card-slug", "v0.99.1"))
        {
            __result = new LocString("cards", "CARD_NAME_V0991.description");
        }
    }
}
```

#### ShouldGlowGoldInternal (NOT overridden - patch base class)

```csharp
[HarmonyPatch(typeof(CardModel), "ShouldGlowGoldInternal", MethodType.Getter)]
static class CardName_ShouldGlowGoldInternal_Patch
{
    static void Postfix(CardModel __instance, ref bool __result)
    {
        if (__instance is CardClassName && RevertAnthony.IsVersion("card-slug", "v0.99.1"))
        {
            __result = true; // Old glow condition
        }
    }
}
```

## Important Rules

1. **One file per card** - Keep patches organized in separate files.
2. **Use `IsVersion(slug, version)`** - Never hardcode version checks.
3. **Always check if overridden** - Use `grep` on the decompiled source to verify if a property/method is overridden in current version.
4. **Prefix vs Postfix**:
   - Use `Prefix(returning false)` for **overridden** methods to skip original
   - Use `Postfix` for **non-overridden** base class properties/methods
5. **Methods removed in current version** - If a method was removed from the derived class, patch the base class with `Postfix` + instance check
6. **Protected members** - Use string literals (e.g., `"Rarity"`, `"CanonicalEnergyCost"`) instead of `nameof()`
7. **Clear canonical cache** - When a user switches versions, `ClearCanonicalCache()` resets the canonical instance so new mutable clones pick up patched values
8. **ModConfig types** - Available types: `Header`, `Dropdown`, `Toggle`, `TextInput`, `Slider`, `Button`, `Separator`, `ColorPicker`. There is NO `Description` type.

## Localization (Card Descriptions)

When a card's **description changes** between versions, you must provide the old description text.

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
