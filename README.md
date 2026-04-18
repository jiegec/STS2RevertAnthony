# RevertAnthony - Slay the Spire 2 Mod

RevertAnthony allows you to individually revert cards to older game versions. Each card gets a dropdown in **ModConfig Gear** to pick either the current version or a historical version.

## Usage

After launching the game with the mod installed, open **ModConfig** → **RevertAnthony** to select which version of each card to use. Choices are saved to `RevertAnthonyConfig.json`.

Changing a setting immediately takes effect on newly spawned cards; existing cards in your current run keep their old values until cloned again.

## Supported Cards (v0.99.1)

All cards listed below also have their **descriptions reverted** to match the v0.99.1 text, so tooltips and card text display correctly.

### DEFECT

### DEFECT

| Card | Property | Current | v0.99.1 |
|------|----------|---------|---------|
| Hotfix | Keywords / Upgrade | Exhaust / Remove Exhaust | **None / +1 Focus** |
| RipAndTear | Rarity | Event | **Uncommon** |
| Voltaic | Cost | 3 | **2** |

### COLORLESS

| Card | Property | Current | v0.99.1 |
|------|----------|---------|---------|
| BelieveInYou | Energy | 2(3) | **3(4)** |
| EternalArmor | Plating / Upgrade | 9(+3) | **7(+2)** |
| HuddleUp | Keywords | Exhaust | **None** |
| Production | Upgrade | +1 Energy | **Remove Exhaust** |
| SeekerStrike | Damage / Cards | 9 / 3 | **6 / 3** |

### EVENT

| Card | Property | Current | v0.99.1 |
|------|----------|---------|---------|
| NeowsFury | Upgrade | +4 Damage, +1 Cards | **+4 Damage** |

### IRONCLAD

| Card | Property | Current | v0.99.1 |
|------|----------|---------|---------|
| Break | Cost / Upgrade | 1 / +10 Dmg, +2 Vuln | **2 / +5 Dmg, +2 Vuln** |
| Cinder | Dmg / Upgrade / OnPlay | 18 / +6 / Exhaust random hand card | **17 / +5 / Exhaust top draw pile card** |
| Colossus | Rarity | Uncommon | **Rare** |
| Dominate | Vars / OnPlay / Upgrade | Vuln 1 + StrPerVuln 1 / Apply Vuln then gain Str / +1 Vuln | **StrPerVuln 1 only / Gain Str from existing Vuln / Remove Exhaust** |
| ExpectAFight | OnPlay | Gain energy + NoEnergyGainPower | **Gain energy** |
| FightMe | Self Strength | 3 | **2** |
| ForgottenRitual | Keywords | Exhaust | **None** |
| Spite | Vars / OnPlay / Upgrade | Dmg 5, Repeat 2 / Hit 1-2x based on HP loss / +1 Repeat | **Dmg 6, Cards 1 / Deal dmg; draw 1 if took damage / +3 Dmg** |
| Stoke | Keywords / OnPlay / Upgrade | None / Exhaust hand, draw new random cards / None | **Exhaust / Exhaust hand, draw same count / -1 Cost** |
| Tremble | Vuln / Keywords | 3 / Exhaust | **2 / None** |

### NECROBINDER

| Card | Property | Current | v0.99.1 |
|------|----------|---------|---------|
| BansheesCry | Cost / Upgrade | 9 / -2 Cost | **6 / +6 Damage** |
| DanseMacabre | Power / Upgrade | 4 / +2 | **3 / +1** |
| Debilitate | Damage | 10 | **7** |
| Defy | Upgrade | +3 Block | **+1 Block, +1 Weak** |
| Folly | Keywords | Unplayable, Eternal, Innate, Ethereal | **Unplayable, Eternal, Innate** |
| GraveWarden | Souls / Hover / Upgrade | Unupgraded Souls / Soul (no upg param) / +3 Block | **Upgraded Souls if card upg / Soul (upg=true) / +2 Block** |
| SculptingStrike | Damage | 9 | **8** |

### REGENT

| Card | Property | Current | v0.99.1 |
|------|----------|---------|---------|
| **Alignment** | Star Cost | 3 | **2** |
| Arsenal | Upgrade | +Innate | **+1 ArsenalPower** |
| Begone | Type / Target / Vars / OnPlay | Skill / Self / None / Transform to MinionStrike | **Attack / AnyEnemy / Dmg 4(+1) / Deal dmg, transform to MinionDiveBomb** |
| BundleOfJoy | Cost | 1 | **2** |
| CelestialMight | Upgrade | +1 Repeat | **+2 Damage** |
| Charge | Transforms to | MinionStrike | **MinionDiveBomb** |
| CollisionCourse | Damage / Upgrade | 11 / +4 | **9 / +3** |
| FallingStar | Damage | 8 | **7** |
| GatherLight | Block | 8 | **7** |
| Glitterstream | Next Turn Block | 5 | **4** |
| Glow | Cards / OnPlay | 1 / Gain stars + draw 1 + DrawCardsNextTurnPower | **2 / Gain stars + draw 2** |
| GrandFinale | Damage / Upgrade | 60 / +15 | **50 / +10** |
| GuidingStar | OnPlay | Deal dmg + draw immediately | **Deal dmg + DrawCardsNextTurnPower** |
| HeirloomHammer | Damage | 20 | **17** |
| IAmInvincible | Block | 10 | **9** |
| KinglyKick | Damage / Upgrade | 27 / +8 | **24 / +6** |
| KinglyPunch | Increase / Upgrade | 3 / +2 Dmg, +2 Incr | **4 / +2 Increase** |
| Parry | ParryPower / Upgrade | 10 / +4 | **6 / +3** |
| Patter | Block | 9 | **8** |
| RefineBlade | Forge | 9 | **6** |
| SeekingEdge | OnPlay order | SeekingEdge first, then Forge | **Forge first, then SeekingEdge** |
| SolarStrike | Damage | 9 | **8** |
| SpoilsOfBattle | Vars / OnPlay / Upgrade | Forge 5, Cards 2 / Forge + draw 2 / +3 Forge | **Forge 10 / Forge only / +5 Forge** |
| VoidForm | Keywords / Upgrade | Ethereal / Remove Ethereal | **None / +1 VoidFormPower** |
| WroughtInWar | Forge | 7 | **5** |

### SILENT

| Card | Property | Current | v0.99.1 |
|------|----------|---------|---------|
| Acrobatics | Rarity | Uncommon | **Common** |
| Anticipate | Dexterity / Upgrade | 2 / +1 | **3 / +2** |
| BladeOfInk | Vars / OnPlay / Upgrade | Shivs 2(+1) with Inky / Create Shivs / +1 Shiv | **Strength 2(+1) / Apply BladeOfInkPower / +1 Str** |
| CorrosiveWave | Poison | 2 | **3** |
| Dirge | Keywords | Exhaust | **None** |
| FlickFlack | Damage | 6 | **7** |
| FollowThrough | Vars / Target / OnPlay / Upgrade | Dmg 7, CardCount 5 / AnyEnemy / Hit 1-2x / +2 Dmg | **Dmg 6, Weak 1 / AllEnemies / Aoe + Weak if last was skill / +2 Dmg, +1 Weak** |
| LeadingStrike | Shivs / Damage | 2 / 3 | **1 / 7** |
| MementoMori | CalculationBase | 9 | **8** |
| Pinpoint | Damage / Upgrade | 15 / +4 | **17 / +5** |
| SerpentForm | Upgrade | +2 Power | **+1 Power** |
| Speedster | Upgrade | +Innate | **+1 SpeedsterPower** |
| Skewer | Damage | 8 | **7** |
| Untouchable | Block / Upgrade | 6 / +2 | **9 / +3** |

## Configuration

`RevertAnthonyConfig.json` stores per-card version choices:
```json
{
    "schema_version": 1,
    "card_versions": {
        "borrowed-time": "v0.99.1",
        "hemokinesis": "Latest"
    }
}
```

## Adding more cards

See [AGENTS.md](AGENTS.md) for the developer guide on adding new cards and old versions.

## Building from source

```bash
./build.sh && ./install.sh
```
