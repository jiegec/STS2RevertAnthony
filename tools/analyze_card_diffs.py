"""
Analyze card and power diffs between game versions for supported cards.
Usage: python3 tools/analyze_card_diffs.py
Define VERSIONS below to compare different version sets.
"""
import re
import os
from collections import defaultdict

os.chdir(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

VERSIONS = ["v0.99.1", "v0.103.2", "v0.107.1"]

MIGRATION_PATTERNS = [
    (
        re.compile(r'await PowerCmd\.Apply(?:<[^>]+>)?\(base\.Owner\.Creature, '),
        re.compile(r'await PowerCmd\.Apply(?:<[^>]+>)?\(choiceContext, base\.Owner\.Creature, '),
    ),
    (
        re.compile(r'await CreatureCmd\.TriggerAnim\(base\.Owner\.Creature,\s*"Cast",\s*base\.Owner\.Character\.CastAnimDelay\)'),
        re.compile(r'await CreatureCmd\.TriggerAnim\(base\.Owner\.Creature,\s*"PowerUp",\s*base\.Owner\.Character\.PowerUpAnimDelay\)'),
    ),
]

def is_migration_pair(removed, added):
    return any(old.search(removed) and new.search(added) for old, new in MIGRATION_PATTERNS)

def pascal_to_kebab(name):
    return re.sub(r'(?<!^)(?=[A-Z])', '-', name).lower()

def parse_diff_file(filepath, subdir_pattern, version_a, version_b):
    """Parse a diff file and extract per-file changes matching subdir_pattern."""
    with open(filepath) as f:
        content = f.read()

    sections = re.split(r'^diff ', content, flags=re.MULTILINE)

    changes = {}
    for section in sections:
        if not section.strip():
            continue
        m = re.search(r'/(?:' + subdir_pattern + r')/(\w+)\.cs', section, re.IGNORECASE)
        if not m:
            continue
        name = m.group(1)
        slug = pascal_to_kebab(name)

        hunks = re.findall(r'@@.*?@@\n(.*?)(?=\n@@|\Z)', section, re.DOTALL)

        summary_lines = []
        for hunk in hunks:
            removes = []
            adds = []
            for line in hunk.split('\n'):
                if line.startswith('-'):
                    stripped = line[1:].strip()
                    if not stripped.startswith('using '):
                        removes.append(stripped)
                elif line.startswith('+'):
                    stripped = line[1:].strip()
                    if not stripped.startswith('using '):
                        adds.append(stripped)

            matched_removes = set()
            matched_adds = set()
            for i, r in enumerate(removes):
                for j, a in enumerate(adds):
                    if j in matched_adds:
                        continue
                    if is_migration_pair(r, a):
                        matched_removes.add(i)
                        matched_adds.add(j)
                        break

            for i, r in enumerate(removes):
                if i not in matched_removes:
                    summary_lines.append(f'[{version_a}] {r}')
            for j, a in enumerate(adds):
                if j not in matched_adds:
                    summary_lines.append(f'[{version_b}] {a}')

        changes[slug] = {
            'pascal': name,
            'summary': summary_lines[:30],
            'has_changes': len(summary_lines) > 0
        }

    return changes

def diff_key(version_a, version_b):
    return f"{version_a}_to_{version_b}"

# Build version pairs from the VERSIONS list
version_pairs = [(VERSIONS[i], VERSIONS[i+1]) for i in range(len(VERSIONS) - 1)]

# Parse card and power diffs for each version pair
card_data = {}
power_data = {}
for va, vb in version_pairs:
    dk = diff_key(va, vb)

    card_filename = f"code-{va}-{vb}-cards.diff"
    if os.path.exists(card_filename):
        diffs = parse_diff_file(card_filename, r'Models/Cards', va, vb)
        for slug, info in diffs.items():
            if slug not in card_data:
                card_data[slug] = {}
            card_data[slug][dk] = info.get('has_changes', False)
            card_data[slug][f"{dk}_summary"] = info.get('summary', [])

    power_filename = f"code-{va}-{vb}-powers.diff"
    if os.path.exists(power_filename):
        diffs = parse_diff_file(power_filename, r'Models/Powers', va, vb)
        for slug, info in diffs.items():
            if slug not in power_data:
                power_data[slug] = {}
            power_data[slug][dk] = info.get('has_changes', False)
            power_data[slug][f"{dk}_summary"] = info.get('summary', [])

# Get supported cards and their slugs
supported_cards = []
card_slugs_set = set()
with open('RevertAnthony.cs') as f:
    for line in f:
        m = re.search(r'new SupportedCard\(\"([^\"]+)\"', line)
        if m:
            supported_cards.append(m.group(1))
            card_slugs_set.add(m.group(1))

# Heuristic: map a card slug to a potential power slug
# e.g. "debilitate" -> "debilitate-power" (if power class is "DebilitatePower")
def card_slug_to_power_slug(slug):
    pascal = ''.join(word.capitalize() for word in slug.split('-'))
    return pascal.lower() + '-power'

# Categorize a card's diff status across all version pairs
def categorize(slug, pairs, data):
    change_flags = {}
    for va, vb in pairs:
        dk = diff_key(va, vb)
        change_flags[dk] = data.get(slug, {}).get(dk, False)
    changed_count = sum(1 for v in change_flags.values() if v)
    if changed_count == 0:
        return 'unchanged'
    if changed_count == len(pairs):
        return 'all_differ'
    changed_pairs = [dk for dk, v in change_flags.items() if v]
    return 'changed_in_' + '_'.join(changed_pairs)

# Print per-card analysis
print("=" * 80)
print("PER-CARD DIFF ANALYSIS")
print(f"Versions: {' → '.join(VERSIONS)}")
print("=" * 80)

for slug in sorted(supported_cards):
    c = card_data.get(slug, {})
    category = categorize(slug, version_pairs, card_data)
    print(f"\n--- {slug} ---")
    print(f"Category: {category}")

    for va, vb in version_pairs:
        dk = diff_key(va, vb)
        changes = c.get(f"{dk}_summary", [])
        if changes:
            print(f"\n  [{va} → {vb}] Card changes:")
            for line in changes[:8]:
                print(f"    {line}")

    # Check if there's a related power that changed
    power_slug = card_slug_to_power_slug(slug)
    p = power_data.get(power_slug, {})
    if p:
        for va, vb in version_pairs:
            dk = diff_key(va, vb)
            pchanges = p.get(f"{dk}_summary", [])
            if pchanges:
                print(f"\n  [{va} → {vb}] Related power {p.get('pascal', power_slug)} changes:")
                for line in pchanges[:6]:
                    print(f"    {line}")

    # Also check exact power name (slug might already match)
    p2 = power_data.get(slug, {})
    if p2 and p2 is not p:
        for va, vb in version_pairs:
            dk = diff_key(va, vb)
            pchanges = p2.get(f"{dk}_summary", [])
            if pchanges:
                print(f"\n  [{va} → {vb}] Related power {p2.get('pascal', slug)} changes:")
                for line in pchanges[:6]:
                    print(f"    {line}")

print("\n\n")
print("=" * 80)
print("CARD SUMMARY")
print(f"Versions: {' → '.join(VERSIONS)}")
print("=" * 80)

categories = defaultdict(list)
for slug in supported_cards:
    categories[categorize(slug, version_pairs, card_data)].append(slug)

for cat, cards in sorted(categories.items()):
    print(f"\n{cat} ({len(cards)} cards):")
    for c in sorted(cards):
        print(f"  - {c}")

print("\n\n")
print("=" * 80)
print("POWER CHANGES (all powers, not just card-related)")
print(f"Versions: {' → '.join(VERSIONS)}")
print("=" * 80)

power_categories = defaultdict(list)
for slug in power_data:
    power_categories[categorize(slug, version_pairs, power_data)].append(slug)

for cat, powers in sorted(power_categories.items()):
    print(f"\n{cat} ({len(powers)} powers):")
    for p in sorted(powers):
        pascal = power_data[p].get('pascal', p)
        summaries = []
        for va, vb in version_pairs:
            dk = diff_key(va, vb)
            s = power_data[p].get(f"{dk}_summary", [])
            if s:
                summaries.append(f"{va}→{vb}: {len(s)} lines")
        print(f"\n  {pascal} ({', '.join(summaries)})")
        for va, vb in version_pairs:
            dk = diff_key(va, vb)
            changes = power_data[p].get(f"{dk}_summary", [])
            if changes:
                print(f"    [{va} → {vb}]")
                for line in changes[:10]:
                    print(f"      {line}")
