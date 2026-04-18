# RevertAnthony - Slay the Spire 2 Mod

RevertAnthony allows you to individually revert cards to older game versions. Each card gets a dropdown in **ModConfig Gear** to pick either the current version or a historical version.

## Usage

After launching the game with the mod installed, open **ModConfig** → **RevertAnthony** to select which version of each card to use. Choices are saved to `RevertAnthonyConfig.json`.

Changing a setting immediately takes effect on newly spawned cards; existing cards in your current run keep their old values until cloned again.

## Supported Cards

### Borrowed Time — `v0.99.1`
| Property | Current (`v0.103.2`) | Old (`v0.99.1`) |
|----------|----------------------|-----------------|
| Cost | 1 | **0** |
| Effect | Gain 4 Energy. Next turn all cards cost +1 more. | **Gain 1 Energy. Apply 3 Doom.** |
| OnUpgrade | Gain +2 Energy | **Gain +1 Energy** |
| Hover Tips | BorrowedTimePower tip | **DoomPower + Energy tip** |

### Hemokinesis — `v0.99.1`
| Property | Current (`v0.103.2`) | Old (`v0.99.1`) |
|----------|----------------------|-----------------|
| Damage | 15 | **14** |

### Skewer — `v0.99.1`
| Property | Current (`v0.103.2`) | Old (`v0.99.1`) |
|----------|----------------------|-----------------|
| Damage | 8 | **7** |

### Acrobatics — `v0.99.1`
| Property | Current (`v0.103.2`) | Old (`v0.99.1`) |
|----------|----------------------|-----------------|
| Rarity | Uncommon | **Common** |

## Configuration

`RevertAnthonyConfig.json` stores per-card version choices:
```json
{
    "schema_version": 1,
    "card_versions": {
        "borrowed-time": "Latest",
        "hemokinesis": "v0.99.1",
        "acrobatics": "Latest",
        "skewer": "v0.99.1"
    }
}
```

## Adding more cards

See [AGENTS.md](AGENTS.md) for the developer guide on adding new cards and old versions.

## Building from source

```bash
./build.sh && ./install.sh
```
