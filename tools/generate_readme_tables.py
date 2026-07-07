"""
Generate updated README tables with three-column support (v0.107.1 | v0.103.2 | v0.99.1).
Uses existing patch files and diff files to determine per-version values.
"""
import re
import os

os.chdir(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

# ============================================================
# KNOWN CARD DATA - compiled from patch files + diff analysis
# ============================================================
# Format: slug -> list of (property, v0.107.1, v0.103.2, v0.99.1)
# Use '' when same as adjacent cell to the right (will auto-fill)

CARD_DATA = {
    # --- DEFECT ---
    "fusion": [
        ("Cost / Keywords / Upgrade", "1, Exhaust / Remove Exhaust", "2, None / -1 Cost", "2, None / -1 Cost"),
    ],
    "hotfix": [
        ("Keywords / Upgrade", "Exhaust / Remove Exhaust", "Exhaust / Remove Exhaust", "None / +1 Focus"),
    ],
    "hyperbeam": [
        ("Damage / Focus Loss", "28 / -3", "26 / -3", "26 / -3"),
    ],
    "rip-and-tear": [
        ("Rarity", "Event", "Event", "Uncommon"),
    ],
    "synthesis": [
        ("Damage / Upgrade", "14 / +6", "12 / +6", "12 / +6"),
    ],
    "voltaic": [
        ("Cost", "3", "3", "2"),
    ],

    # --- COLORLESS ---
    "believe-in-you": [
        ("Energy", "2(3)", "2(3)", "3(4)"),
    ],
    "eternal-armor": [
        ("Plating / Upgrade", "9(+3)", "9(+3)", "7(+2)"),
    ],
    "fasten": [
        ("Extra Block", "4", "5", "5"),
    ],
    "hidden-gem": [
        ("OnPlay / Generation", "Excludes enchanted replay / Cannot generate in combat", "Excludes enchanted replay / Cannot generate in combat", "No filter / Anywhere"),
    ],
    "huddle-up": [
        ("Keywords", "Exhaust", "Exhaust", "None"),
    ],
    "production": [
        ("Upgrade", "+1 Energy", "+1 Energy", "Remove Exhaust"),
    ],
    "seeker-strike": [
        ("Damage / Cards", "9 / 3", "9 / 3", "6 / 3"),
    ],

    # --- ANCIENT ---
    "neows-fury": [
        ("Upgrade", "+4 Damage, +1 Cards", "+4 Damage, +1 Cards", "+4 Damage"),
    ],

    # --- CURSE ---
    "folly": [
        ("Keywords", "Unplayable, Eternal, Innate, Ethereal", "Unplayable, Eternal, Innate, Ethereal", "Unplayable, Eternal, Innate"),
    ],

    # --- IRONCLAD ---
    "break": [
        ("Cost / Upgrade", "1 / +10 Dmg, +2 Vuln", "1 / +10 Dmg, +2 Vuln", "2 / +5 Dmg, +2 Vuln"),
    ],
    "cinder": [
        ("Damage / Upgrade / OnPlay", "18 / +6 / Exhaust random hand card", "18 / +6 / Exhaust random hand card", "17 / +5 / Exhaust top draw pile card"),
    ],
    "colossus": [
        ("Rarity", "Uncommon", "Uncommon", "Rare"),
    ],
    "conflagration": [
        ("Vars / OnPlay / Upgrade", "Dmg 2, Repeat 4 / Attack all with repeats / +1 Repeat", "CalcBase 8, ExtraDmg 2 / Scales with attacks / +1 Base, +1 Extra", "CalcBase 8, ExtraDmg 2 / Scales with attacks / +1 Base, +1 Extra"),
    ],
    "dominate": [
        ("Vars / OnPlay / Upgrade", "Vuln 1 + StrPerVuln 1 / Apply Vuln then gain Str / +1 Vuln", "Vuln 1 + StrPerVuln 1 / Apply Vuln then gain Str / +1 Vuln", "StrPerVuln 1 only / Gain Str from existing Vuln / Remove Exhaust"),
    ],
    "expect-a-fight": [
        ("OnPlay", "Gain energy + NoEnergyGainPower", "Gain energy + NoEnergyGainPower", "Gain energy"),
    ],
    "fight-me": [
        ("Self Strength", "3", "3", "2"),
    ],
    "forgotten-ritual": [
        ("Keywords", "Exhaust", "Exhaust", "None"),
    ],
    "hemokinesis": [
        ("Damage / Self-damage", "18 / 2", "18 / 2", "15 / 2"),
    ],
    "juggernaut": [
        ("JuggernautPower / Upgrade", "6 / +2", "5 / +2", "5 / +2"),
    ],
    "spite": [
        ("Vars / OnPlay / Upgrade", "Dmg 5, Repeat 2 / Hit 1-2x based on HP loss / +1 Repeat", "Dmg 5, Repeat 2 / Hit 1-2x based on HP loss / +1 Repeat", "Dmg 6, Cards 1 / Deal dmg; draw 1 if took damage / +3 Dmg"),
    ],
    "stoke": [
        ("Keywords / OnPlay / Upgrade", "None / Exhaust hand, draw new random cards / None", "None / Exhaust hand, draw new random cards / None", "Exhaust / Exhaust hand, draw same count / -1 Cost"),
    ],
    "tremble": [
        ("Vulnerable / Keywords", "3 / Exhaust", "3 / Exhaust", "2 / None"),
    ],

    # --- NECROBINDER ---
    "banshees-cry": [
        ("Cost / Upgrade", "9 / -2 Cost", "9 / -2 Cost", "6 / +6 Damage"),
    ],
    "borrowed-time": [
        ("Keywords / OnPlay / Upgrade", "None / Gain energy + cards cost +1 / None", "None / Gain energy + cards cost +1 / None", "None / Apply Doom + gain energy / +1 Energy"),
    ],
    "danse-macabre": [
        ("Power / Upgrade", "4 / +2", "4 / +2", "3 / +1"),
    ],
    "death-march": [
        ("ExtraDamage / Upgrade", "4 / +2", "3 / +1", "3 / +1"),
    ],
    "debilitate": [
        ("Damage / Power", "10 / 2", "10 / 3", "7 / 3"),
    ],
    "defy": [
        ("Upgrade", "+3 Block", "+3 Block", "+1 Block, +1 Weak"),
    ],
    "dirge": [
        ("Keywords", "Exhaust", "Exhaust", "None"),
    ],
    "grave-warden": [
        ("Souls / Hover / Upgrade", "Unupgraded Souls / Soul (no upg param) / +3 Block", "Unupgraded Souls / Soul (no upg param) / +3 Block", "Upgraded Souls if card upg / Soul (upg=true) / +2 Block"),
    ],
    "sculpting-strike": [
        ("Damage", "9", "9", "8"),
    ],
    "seance": [
        ("Cost / Hover / OnPlay / Upgrade", "1 / Show Soul / Transform draw pile to Soul / None", "1 / Show Soul / Transform draw pile to Soul / None", "0 / Show upgraded Soul / Transform draw pile to upgraded Soul / None"),
    ],
    "the-scythe": [
        ("Increase", "4", "3", "3"),
    ],

    # --- REGENT ---
    "alignment": [
        ("Star Cost", "3", "3", "2"),
    ],
    "arsenal": [
        ("Upgrade", "+Innate", "+Innate", "+1 ArsenalPower"),
    ],
    "astral-pulse": [
        ("Damage / Hit Count / Upgrade", "6, 2 hits / +2 Dmg", "14, 1 hit / +4 Dmg", "14, 1 hit / +4 Dmg"),
    ],
    "begone": [
        ("Type / Target / Vars / OnPlay", "Skill / Self / None / Transform to MinionStrike", "Skill / Self / None / Transform to MinionStrike", "Attack / AnyEnemy / Dmg 4(+1) / Deal dmg, transform to MinionDiveBomb"),
    ],
    "bundle-of-joy": [
        ("Cost", "1", "1", "2"),
    ],
    "bulwark": [
        ("Block / Forge", "12 / 10", "13 / 10", "13 / 10"),
    ],
    "celestial-might": [
        ("Upgrade", "+1 Repeat", "+1 Repeat", "+2 Damage"),
    ],
    "charge": [
        ("Transforms to", "MinionStrike", "MinionStrike", "MinionDiveBomb"),
    ],
    "collision-course": [
        ("Damage / Upgrade", "11 / +4", "11 / +4", "9 / +3"),
    ],
    "crescent-spear": [
        ("CalculationBase", "8", "6", "6"),
    ],
    "falling-star": [
        ("Damage", "8", "8", "7"),
    ],
    "furnace": [
        ("Forge", "5", "4", "4"),
    ],
    "gather-light": [
        ("Block", "8", "", "7"),
    ],
    "glitterstream": [
        ("Next Turn Block", "5", "5", "4"),
    ],
    "glow": [
        ("Cards / OnPlay", "1 / Gain stars + draw 1 + DrawCardsNextTurnPower", "1 / Gain stars + draw 1 + DrawCardsNextTurnPower", "2 / Gain stars + draw 2"),
    ],
    "grand-finale": [
        ("Damage / Upgrade", "60 / +15", "60 / +15", "50 / +10"),
    ],
    "guiding-star": [
        ("OnPlay", "Deal dmg + draw immediately", "", "Deal dmg + DrawCardsNextTurnPower"),
    ],
    "heirloom-hammer": [
        ("Damage", "20", "20", "17"),
    ],
    "i-am-invincible": [
        ("Block", "10", "10", "9"),
    ],
    "kingly-kick": [
        ("Damage / Upgrade", "27 / +8", "", "24 / +6"),
    ],
    "kingly-punch": [
        ("Increase / Upgrade", "4 / +2 Dmg, +2 Incr", "", "3 / +2 Increase"),
    ],
    "minion-dive-bomb": [
        ("Cost", "0", "0", "1"),
    ],
    "minion-strike": [
        ("Damage", "6", "6", "7"),
    ],
    "monarchs-gaze": [
        ("Cost", "2", "3", "3"),
    ],
    "parry": [
        ("ParryPower / Upgrade", "10 / +4", "10 / +4", "6 / +3"),
    ],
    "patter": [
        ("Block", "8", "9", "8"),
    ],
    "refine-blade": [
        ("Forge", "9", "9", "6"),
    ],
    "reflect": [
        ("Block / Upgrade", "15 / +5", "17 / +4", "17 / +4"),
    ],
    "solar-strike": [
        ("Damage", "9", "9", "8"),
    ],
    "spoils-of-battle": [
        ("Vars / OnPlay / Upgrade", "Forge 5, Cards 2 / Forge + draw 2 / +3 Forge", "Forge 5, Cards 2 / Forge + draw 2 / +3 Forge", "Forge 10 / Forge only / +5 Forge"),
    ],
    "sword-sage": [
        ("Power", "No energy cost increase", "No energy cost increase", "Add energy cost to SovereignBlade"),
    ],
    "the-sealed-throne": [
        ("Upgrade", "-1 cost", "Innate", "Innate"),
    ],
    "void-form": [
        ("Keywords / Upgrade", "Ethereal / Remove Ethereal", "Ethereal / Remove Ethereal", "None / +1 VoidFormPower"),
    ],
    "wrought-in-war": [
        ("Forge", "7", "7", "5"),
    ],

    # --- SILENT ---
    "acrobatics": [
        ("Rarity", "Uncommon", "Uncommon", "Common"),
    ],
    "anticipate": [
        ("Dexterity / Upgrade", "2 / +1", "2 / +1", "3 / +2"),
    ],
    "blade-of-ink": [
        ("Vars / OnPlay / Upgrade", "Shivs 2(+1) with Inky / Create Shivs / +1 Shiv", "Shivs 2(+1) with Inky / Create Shivs / +1 Shiv", "Strength 2(+1) / Apply BladeOfInkPower / +1 Str"),
    ],
    "corrosive-wave": [
        ("Poison", "2", "2", "3"),
    ],
    "flick-flack": [
        ("Damage", "6", "6", "7"),
    ],
    "leading-strike": [
        ("Shivs / Damage", "2 / 3", "2 / 3", "1 / 7"),
    ],
    "memento-mori": [
        ("CalculationBase", "9", "9", "8"),
    ],
    "pinpoint": [
        ("Damage / Upgrade", "15 / +4", "15 / +4", "17 / +5"),
    ],
    "pounce": [
        ("Damage / Upgrade", "14 / +6", "12 / +6", "12 / +6"),
    ],
    "predator": [
        ("Rarity", "Common", "Uncommon", "Uncommon"),
    ],
    "serpent-form": [
        ("Upgrade", "+2 Power", "+2 Power", "+1 Power"),
    ],
    "skewer": [
        ("Damage", "8", "8", "7"),
    ],
    "speedster": [
        ("Upgrade", "+Innate", "+Innate", "+1 SpeedsterPower"),
    ],
    "untouchable": [
        ("Block / Upgrade", "6 / +3", "6 / +2", "9 / +3"),
    ],
}

def format_row(card_name, data):
    """Format a card row for the README table."""
    rows = []
    for entry in data:
        property_name = entry[0]
        cells = entry[1:]
        # Bold the card name only on the first property row
        if entry == data[0]:
            rows.append(f"| **{card_name}** | {property_name} | {' | '.join(cells)} |")
        else:
            rows.append(f"| | {property_name} | {' | '.join(cells)} |")
    return '\n'.join(rows)

# Character groupings
CHARACTERS = {
    "DEFECT": ["fusion", "hotfix", "hyperbeam", "rip-and-tear", "synthesis", "voltaic"],
    "COLORLESS": ["believe-in-you", "eternal-armor", "fasten", "hidden-gem", "huddle-up", "production", "seeker-strike"],
    "ANCIENT": ["neows-fury"],
    "CURSE": ["folly"],
    "IRONCLAD": ["break", "cinder", "colossus", "conflagration", "dominate", "expect-a-fight", "fight-me", "forgotten-ritual", "hemokinesis", "juggernaut", "spite", "stoke", "tremble"],
    "NECROBINDER": ["banshees-cry", "borrowed-time", "danse-macabre", "death-march", "debilitate", "defy", "dirge", "grave-warden", "sculpting-strike", "seance", "the-scythe"],
    "REGENT": ["alignment", "arsenal", "astral-pulse", "begone", "bulwark", "bundle-of-joy", "celestial-might", "charge", "collision-course", "crescent-spear", "falling-star", "furnace", "gather-light", "glitterstream", "glow", "grand-finale", "guiding-star", "heirloom-hammer", "i-am-invincible", "kingly-kick", "kingly-punch", "minion-dive-bomb", "minion-strike", "monarchs-gaze", "parry", "patter", "refine-blade", "reflect", "solar-strike", "spoils-of-battle", "sword-sage", "the-sealed-throne", "void-form", "wrought-in-war"],
    "SILENT": ["acrobatics", "anticipate", "blade-of-ink", "corrosive-wave", "flick-flack", "leading-strike", "memento-mori", "pinpoint", "pounce", "predator", "serpent-form", "skewer", "speedster", "untouchable"],
}

lines = []
lines.append("## Supported Cards")
lines.append("")
lines.append("All cards listed below can be reverted to their v0.99.1 or v0.103.2 versions. Descriptions are also reverted to match the old version text, so tooltips and card text display correctly.")
lines.append("")

for char_name, card_slugs in CHARACTERS.items():
    lines.append("")
    lines.append(f"### {char_name}")
    lines.append("")
    lines.append("| Card | Property | v0.107.1 (Current) | v0.103.2 | v0.99.1 |")
    lines.append("|------|----------|-------------------|----------|---------|")
    for slug in card_slugs:
        if slug in CARD_DATA:
            lines.append(format_row(slug.replace('-', ' ').title(), CARD_DATA[slug]))

new_section = "\n".join(lines)

with open("README.md") as f:
    content = f.read()

start = content.index("## Supported Cards")
end = content.index("\n## Configuration")
content = content[:start] + new_section + "\n\n" + content[end:]

with open("README.md", "w") as f:
    f.write(content)

print("README.md updated")
