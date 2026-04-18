# RevertAnthony - Slay the Spire 2 Mod

A mod for Slay the Spire 2 that allows you to individually revert cards to older game versions (currently v0.99.1). Each card gets a dropdown in **ModConfig** to pick either the current version or a historical version, with changes taking effect immediately on newly spawned cards.

**Supported game versions:** v0.103.2

## Features

- **Individual card reversion**: Lots of cards can be reverted to their v0.99.1 versions
- **Batch operations**: Set all cards or all cards from a specific character to a version in one click
- **Description reversion**: Card descriptions are also reverted to match the old version text
- **GUI Configuration**: Full in-game configuration via [ModConfig](https://github.com/xhyrzldf/ModConfig-STS2)
- **Manual Configuration**: Direct JSON configuration for advanced users

## Installation

1. Download the latest release from [GitHub releases](https://github.com/jiegec/STS2RevertAnthony/releases) or [Nexus mods](https://www.nexusmods.com/slaythespire2/mods/...)
2. Extract the mod files to your Slay the Spire 2 mods folder (`mods` folder should reside in the same folder as the game executable):
   - **Windows**: `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\`
   - **macOS**: `~/Library/Application\ Support/Steam/steamapps/common/Slay\ the\ Spire\ 2/SlayTheSpire2.app/Contents/MacOS/mods/`
   - **Linux**: `~/.steam/steam/steamapps/common/Slay\ the\ Spire\ 2/mods`
3. Launch Slay the Spire 2 - the mod will load automatically

## Building from Source

### Prerequisites

- .NET 9.0 SDK or later
- Godot 4.5.1 with Mono support
- Slay the Spire 2 (for the sts2.dll and 0Harmony.dll reference)

### Build Steps

```bash
# Clone the repository
git clone https://github.com/jiegec/STS2RevertAnthony
cd STS2RevertAnthony

# Build the mod
./build.sh

# Install the mod
./install.sh
```

## Supported Cards

All cards listed below can be reverted to their v0.99.1 versions. Descriptions are also reverted to match the old version text, so tooltips and card text display correctly.

### DEFECT

| Card | Property | Current | v0.99.1 |
|------|----------|---------|---------|
| **Hotfix** | Keywords / Upgrade | Exhaust / Remove Exhaust | **None / +1 Focus** |
| **Rip and Tear** | Rarity | Event | **Uncommon** |
| **Voltaic** | Cost | 3 | **2** |

### COLORLESS

| Card | Property | Current | v0.99.1 |
|------|----------|---------|---------|
| **Believe In You** | Energy | 2(3) | **3(4)** |
| **Eternal Armor** | Plating / Upgrade | 9(+3) | **7(+2)** |
| **Hidden Gem** | OnPlay / Generation | Excludes enchanted replay / Combat only | **No filter / Anywhere** |
| **Huddle Up** | Keywords | Exhaust | **None** |
| **Production** | Upgrade | +1 Energy | **Remove Exhaust** |
| **Seeker Strike** | Damage / Cards | 9 / 3 | **6 / 3** |

### ANCIENT

| Card | Property | Current | v0.99.1 |
|------|----------|---------|---------|
| **Neow's Fury** | Upgrade | +4 Damage, +1 Cards | **+4 Damage** |

### IRONCLAD

| Card | Property | Current | v0.99.1 |
|------|----------|---------|---------|
| **Break** | Cost / Upgrade | 1 / +10 Dmg, +2 Vuln | **2 / +5 Dmg, +2 Vuln** |
| **Cinder** | Damage / Upgrade / OnPlay | 18 / +6 / Exhaust random hand card | **17 / +5 / Exhaust top draw pile card** |
| **Colossus** | Rarity | Uncommon | **Rare** |
| **Dominate** | Vars / OnPlay / Upgrade | Vuln 1 + StrPerVuln 1 / Apply Vuln then gain Str / +1 Vuln | **StrPerVuln 1 only / Gain Str from existing Vuln / Remove Exhaust** |
| **Expect A Fight** | OnPlay | Gain energy + NoEnergyGainPower | **Gain energy** |
| **Fight Me** | Self Strength | 3 | **2** |
| **Forgotten Ritual** | Keywords | Exhaust | **None** |
| **Hemokinesis** | Damage / Self-damage | 18 / 3 | **15 / 3** |
| **Spite** | Vars / OnPlay / Upgrade | Dmg 5, Repeat 2 / Hit 1-2x based on HP loss / +1 Repeat | **Dmg 6, Cards 1 / Deal dmg; draw 1 if took damage / +3 Dmg** |
| **Stoke** | Keywords / OnPlay / Upgrade | None / Exhaust hand, draw new random cards / None | **Exhaust / Exhaust hand, draw same count / -1 Cost** |
| **Tremble** | Vulnerable / Keywords | 3 / Exhaust | **2 / None** |

### NECROBINDER

| Card | Property | Current | v0.99.1 |
|------|----------|---------|---------|
| **Banshee's Cry** | Cost / Upgrade | 9 / -2 Cost | **6 / +6 Damage** |
| **Borrowed Time** | Keywords / OnPlay / Upgrade | None / Gain energy + cards cost +1 / None | **Exhaust / Apply Doom + gain energy / Remove Exhaust** |
| **Danse Macabre** | Power / Upgrade | 4 / +2 | **3 / +1** |
| **Debilitate** | Damage | 10 | **7** |
| **Defy** | Upgrade | +3 Block | **+1 Block, +1 Weak** |
| **Dirge** | Keywords | Exhaust | **None** |
| **Folly** | Keywords | Unplayable, Eternal, Innate, Ethereal | **Unplayable, Eternal, Innate** |
| **Grave Warden** | Souls / Hover / Upgrade | Unupgraded Souls / Soul (no upg param) / +3 Block | **Upgraded Souls if card upg / Soul (upg=true) / +2 Block** |
| **Sculpting Strike** | Damage | 9 | **8** |
| **Seance** | Cost / Hover / OnPlay / Upgrade | 1 / Show Soul / Transform draw pile to Soul / None | **0 / Show upgraded Soul / Transform draw pile to upgraded Soul / +1 Plated Armor** |

### REGENT

| Card | Property | Current | v0.99.1 |
|------|----------|---------|---------|
| **Alignment** | Star Cost | 3 | **2** |
| **Arsenal** | Upgrade | +Innate | **+1 ArsenalPower** |
| **Begone** | Type / Target / Vars / OnPlay | Skill / Self / None / Transform to MinionStrike | **Attack / AnyEnemy / Dmg 4(+1) / Deal dmg, transform to MinionDiveBomb** |
| **Bundle of Joy** | Cost | 1 | **2** |
| **Celestial Might** | Upgrade | +1 Repeat | **+2 Damage** |
| **Charge** | Transforms to | MinionStrike | **MinionDiveBomb** |
| **Collision Course** | Damage / Upgrade | 11 / +4 | **9 / +3** |
| **Falling Star** | Damage | 8 | **7** |
| **Gather Light** | Block | 8 | **7** |
| **Glitterstream** | Next Turn Block | 5 | **4** |
| **Glow** | Cards / OnPlay | 1 / Gain stars + draw 1 + DrawCardsNextTurnPower | **2 / Gain stars + draw 2** |
| **Grand Finale** | Damage / Upgrade | 60 / +15 | **50 / +10** |
| **Guiding Star** | OnPlay | Deal dmg + draw immediately | **Deal dmg + DrawCardsNextTurnPower** |
| **Heirloom Hammer** | Damage | 20 | **17** |
| **I Am Invincible** | Block | 10 | **9** |
| **Kingly Kick** | Damage / Upgrade | 27 / +8 | **24 / +6** |
| **Kingly Punch** | Increase / Upgrade | 3 / +2 Dmg, +2 Incr | **4 / +2 Increase** |
| **Minion Dive Bomb** | Cost | 0 | **1** |
| **Minion Strike** | Damage | 6 | **7** |
| **Parry** | ParryPower / Upgrade | 10 / +4 | **6 / +3** |
| **Patter** | Block | 9 | **8** |
| **Refine Blade** | Forge | 9 | **6** |
| **Solar Strike** | Damage | 9 | **8** |
| **Spoils of Battle** | Vars / OnPlay / Upgrade | Forge 5, Cards 2 / Forge + draw 2 / +3 Forge | **Forge 10 / Forge only / +5 Forge** |
| **Void Form** | Keywords / Upgrade | Ethereal / Remove Ethereal | **None / +1 VoidFormPower** |
| **Wrought in War** | Forge | 7 | **5** |

### SILENT

| Card | Property | Current | v0.99.1 |
|------|----------|---------|---------|
| **Acrobatics** | Rarity | Uncommon | **Common** |
| **Anticipate** | Dexterity / Upgrade | 2 / +1 | **3 / +2** |
| **Blade of Ink** | Vars / OnPlay / Upgrade | Shivs 2(+1) with Inky / Create Shivs / +1 Shiv | **Strength 2(+1) / Apply BladeOfInkPower / +1 Str** |
| **Corrosive Wave** | Poison | 2 | **3** |
| **Flick Flack** | Damage | 6 | **7** |
| **Follow Through** | Vars / Target / OnPlay / Upgrade | Dmg 7, CardCount 5 / AnyEnemy / Hit 1-2x / +2 Dmg | **Dmg 6, Weak 1 / AllEnemies / Aoe + Weak if last was skill / +2 Dmg, +1 Weak** |
| **Leading Strike** | Shivs / Damage | 2 / 3 | **1 / 7** |
| **Memento Mori** | CalculationBase | 9 | **8** |
| **Pinpoint** | Damage / Upgrade | 15 / +4 | **17 / +5** |
| **Serpent Form** | Upgrade | +2 Power | **+1 Power** |
| **Skewer** | Damage | 8 | **7** |
| **Speedster** | Upgrade | +Innate | **+1 SpeedsterPower** |
| **Untouchable** | Block / Upgrade | 6 / +2 | **9 / +3** |

## Configuration

### GUI Settings (Recommended)

RevertAnthony integrates with [**ModConfig**](https://github.com/xhyrzldf/ModConfig-STS2). When ModConfig is installed, RevertAnthony appears in the game's **Settings > Mods** menu. If ModConfig is not installed, the mod still works normally, but you'll need to edit the JSON configuration file manually (see below).

With ModConfig GUI, you can:

- **Batch Operations**:
  - **Set all cards to**: Apply a version to ALL cards at once (only cards that support it)
  - **Set all [Character] cards to**: Apply a version to all cards from a specific character
  - Batch dropdowns automatically reset to "Unchanged" after the operation completes
- **Individual card settings**: Each card has its own dropdown with available versions
  - Cards are grouped by character for easy navigation
  - Only versions supported by each card are shown in the dropdown
- **Changes are saved automatically** to the config file location (see below for details)

### Manual JSON Configuration

Alternatively, you can customize card versions by manually editing `RevertAnthonyConfig.json`:

- **Existing users**: If you already have a config file at `mods/RevertAnthonyConfig.json`, it will continue to be used (no migration needed)
- **New users**: The config will be saved alongside `RevertAnthony.dll` (found recursively in the mods folder). If the DLL cannot be found, it falls back to `mods/RevertAnthonyConfig.json`
- **Note**: Config is saved to the same location where it was read from. The mod will not automatically migrate the config file to a different location.

```json
{
    "schema_version": 1,
    "card_versions": {
        "borrowed-time": "v0.99.1",
        "hemokinesis": "Latest"
    }
}
```

- **schema_version**: Config file format version (1 is the current version)
- **card_versions**: Dictionary mapping card slugs to version strings
  - Use `"Latest"` for the current game version
  - Use `"v0.99.1"` for the old version (if the card supports it)

If the config file is missing or invalid, default settings (all cards set to Latest) are used.

## Adding More Cards

See [AGENTS.md](AGENTS.md) for the developer guide on adding new cards and old versions.

## How It Works

The mod uses Harmony patches to intercept card properties at runtime:

- **Overridden properties** (patched on the card's own class): `CanonicalVars`, `OnPlay`, `ExtraHoverTips`, `ShouldGlowGoldInternal`
- **Non-overridden properties** (patched on `CardModel` base class): `Rarity`, `CanonicalEnergyCost`, `TargetType`, `Type`, `Description`
- **Removed methods** (patched on base class with Postfix): `OnUpgrade`, `AfterCardPlayed`

When a card's version is set to an old version, the patches return the historical values. When set to Latest, the original game behavior is preserved.

## Changelog

### v1.0.0

- Initial release
- Support for reverting 66 cards to v0.99.1 versions
- Batch operations for global and per-character version switching
- ModConfig GUI integration
- Description localization support (English and Simplified Chinese)
- Tested on game versions v0.102.0 and v0.103.2
