"""
Analyze card diffs between game versions for supported cards.
Usage: python3 tools/analyze_card_diffs.py
Define VERSIONS below to compare different version sets.
"""
import re
import os
from collections import defaultdict

os.chdir(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

VERSIONS = ["v0.99.1", "v0.103.2", "v0.107.1"]

def pascal_to_kebab(name):
    return re.sub(r'(?<!^)(?=[A-Z])', '-', name).lower()

def parse_card_diff(filepath, version_a, version_b):
    with open(filepath) as f:
        content = f.read()

    sections = re.split(r'^diff ', content, flags=re.MULTILINE)

    card_changes = {}
    for section in sections:
        if not section.strip():
            continue
        m = re.search(r'/Cards/(\w+)\.cs', section)
        if not m:
            continue
        pascal_name = m.group(1)
        slug = pascal_to_kebab(pascal_name)

        hunks = re.findall(r'@@.*?@@\n(.*?)(?=\n@@|\Z)', section, re.DOTALL)

        summary_lines = []
        for hunk in hunks:
            for line in hunk.split('\n'):
                if line.startswith('-'):
                    summary_lines.append(f'[{version_a}] {line[1:].strip()}')
                elif line.startswith('+'):
                    summary_lines.append(f'[{version_b}] {line[1:].strip()}')

        card_changes[slug] = {
            'pascal': pascal_name,
            'hunks': hunks,
            'summary': summary_lines[:30],
            'has_changes': bool(hunks and any(h.strip() for h in hunks))
        }

    return card_changes

def diff_key(version_a, version_b):
    return f"{version_a}_to_{version_b}"

# Build version pairs from the VERSIONS list
version_pairs = [(VERSIONS[i], VERSIONS[i+1]) for i in range(len(VERSIONS) - 1)]

# Parse all diff files
card_data = {}  # slug -> { diff_key -> has_changes, diff_key_summary -> [...] }
for va, vb in version_pairs:
    filename = f"code-{va}-{vb}-cards.diff"
    dk = diff_key(va, vb)
    diffs = parse_card_diff(filename, va, vb)
    for slug, info in diffs.items():
        if slug not in card_data:
            card_data[slug] = {}
        card_data[slug][dk] = info.get('has_changes', False)
        card_data[slug][f"{dk}_summary"] = info.get('summary', [])

# Get supported cards
supported_cards = []
with open('RevertAnthony.cs') as f:
    for line in f:
        m = re.search(r'new SupportedCard\(\"([^\"]+)\"', line)
        if m:
            supported_cards.append(m.group(1))

# Determine category for each card
def categorize(slug, pairs):
    change_flags = {}
    for va, vb in pairs:
        dk = diff_key(va, vb)
        change_flags[dk] = card_data.get(slug, {}).get(dk, False)

    changed_count = sum(1 for v in change_flags.values() if v)
    if changed_count == 0:
        return 'unchanged'
    if changed_count == len(pairs):
        return 'all_differ'
    # Find which pairs changed
    changed_pairs = [dk for dk, v in change_flags.items() if v]
    return 'changed_in_' + '_'.join(changed_pairs)

# Print per-card analysis
print("=" * 80)
print("PER-CARD DIFF ANALYSIS")
print(f"Versions: {' → '.join(VERSIONS)}")
print("=" * 80)

for slug in sorted(supported_cards):
    c = card_data.get(slug, {})
    category = categorize(slug, version_pairs)
    print(f"\n--- {slug} ---")
    print(f"Category: {category}")

    for va, vb in version_pairs:
        dk = diff_key(va, vb)
        changes = c.get(f"{dk}_summary", [])
        if changes:
            print(f"\n  {va} → {vb} changes:")
            for line in changes[:10]:
                print(f"    {line}")

print("\n\n")
print("=" * 80)
print("SUMMARY")
print(f"Versions: {' → '.join(VERSIONS)}")
print("=" * 80)

categories = defaultdict(list)
for slug in supported_cards:
    categories[categorize(slug, version_pairs)].append(slug)

for cat, cards in sorted(categories.items()):
    print(f"\n{cat} ({len(cards)} cards):")
    for c in sorted(cards):
        print(f"  - {c}")
