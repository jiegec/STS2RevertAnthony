#!/usr/bin/env python3
"""
Extract old card descriptions from game version PCK files and save them for RevertAnthony mod.
Compares descriptions across multiple versions and only saves ones that differ.

Usage:
    python3 extract_old_localizations.py <path_to_pck1> [<path_to_pck2> ...] [--output-dir <path>]

Example:
    python3 extract_old_localizations.py "/path/to/v0.99.1.pck" "/path/to/v0.103.2.pck" --output-dir ../RevertAnthony/localization
"""

import argparse
import json
import re
import struct
import sys
from pathlib import Path


def parse_pck(pck_path):
    """Parse Godot PCK file and return list of (path, offset, size)."""
    entries = []
    with open(pck_path, "rb") as f:
        magic = f.read(4)
        if magic != b"GDPC":
            raise ValueError(f"Invalid PCK magic: {magic!r}")
        
        pack_version = struct.unpack("<I", f.read(4))[0]
        if pack_version <= 2:
            raise ValueError(f"Unsupported pack version: {pack_version}")
        
        # Skip version and flags
        f.read(12)  # major, minor, patch
        f.read(4)   # flags
        file_base = struct.unpack("<Q", f.read(8))[0]
        
        dir_offset = struct.unpack("<Q", f.read(8))[0]
        f.seek(dir_offset)
        
        file_count = struct.unpack("<I", f.read(4))[0]
        
        for _ in range(file_count):
            path_len = struct.unpack("<I", f.read(4))[0]
            path_bytes = f.read(path_len)
            path = path_bytes.split(b"\x00", 1)[0].decode("utf-8", errors="replace")
            offset = struct.unpack("<Q", f.read(8))[0]
            size = struct.unpack("<Q", f.read(8))[0]
            f.read(16)  # md5 skip
            flags = struct.unpack("<I", f.read(4))[0]
            entries.append((path, offset, size))
    
    return file_base, entries


def extract_localizations(pck_path, languages=None):
    """Extract localization tables from PCK. Returns {language: {table_name: {key: value}}}.
    If languages is None, auto-detect all languages present in the PCK."""
    file_base, entries = parse_pck(pck_path)
    
    # Auto-detect languages if not specified
    if languages is None:
        languages = set()
        for path, _, _ in entries:
            if path.startswith("localization/") and path.endswith(".json"):
                parts = path.split("/")
                if len(parts) >= 2:
                    languages.add(parts[1])
        languages = sorted(languages)
        if languages:
            print(f"Auto-detected languages: {', '.join(languages)}")
    
    result = {lang: {} for lang in languages}
    
    with open(pck_path, "rb") as f:
        for path, offset, size in entries:
            for lang in languages:
                prefix = f"localization/{lang}/"
                if not path.startswith(prefix) or not path.endswith(".json"):
                    continue
                
                table_name = path[len(prefix):-len(".json")]
                real_offset = file_base + offset
                f.seek(real_offset)
                data = f.read(size)
                
                try:
                    table_data = json.loads(data)
                    if isinstance(table_data, dict):
                        if table_name not in result[lang]:
                            result[lang][table_name] = {}
                        result[lang][table_name].update(table_data)
                except (json.JSONDecodeError, UnicodeDecodeError) as e:
                    print(f"Warning: failed to parse {path}: {e}", file=sys.stderr)
    
    return result


def detect_version(pck_path):
    """Auto-detect version from release_info.json next to PCK."""
    release_info_path = pck_path.parent / "release_info.json"
    if release_info_path.exists():
        try:
            with open(release_info_path, "r", encoding="utf-8") as f:
                release_info = json.load(f)
                detected_version = release_info.get("version", "")
                if detected_version:
                    # Convert "v0.99.1" → "V0991"
                    version_label = "V" + detected_version.lstrip("vV").replace(".", "")
                    return detected_version, version_label
        except (json.JSONDecodeError, OSError) as e:
            print(f"Warning: Could not read release_info.json: {e}", file=sys.stderr)
    
    # Fallback: use PCK filename
    stem = pck_path.stem
    # Try to extract version from filename like "v0.99.1.pck"
    match = re.search(r'v(\d+\.\d+\.\d+)', stem)
    if match:
        version = match.group(0)
        label = "V" + version.lstrip("vV").replace(".", "")
        return version, label
    
    return None, None


def find_supported_cards():
    """Find all supported card slugs from RevertAnthony.cs."""
    revert_anthony_path = Path(__file__).parent.parent / "RevertAnthony.cs"
    if not revert_anthony_path.exists():
        print(f"Warning: Could not find {revert_anthony_path}, using default cards", file=sys.stderr)
        return ["borrowed-time"]
    
    slugs = []
    with open(revert_anthony_path, "r", encoding="utf-8") as f:
        content = f.read()
    
    # Find all lines like: new SupportedCard("slug", "CHARACTER", "v0.99.1"),
    pattern = r'new SupportedCard\("([^"]+)"'
    slugs = re.findall(pattern, content)
    
    if not slugs:
        print("Warning: No SupportedCard entries found in RevertAnthony.cs", file=sys.stderr)
        return ["borrowed-time"]
    
    print(f"Auto-detected {len(slugs)} cards from RevertAnthony.cs")
    return slugs


def main():
    parser = argparse.ArgumentParser(
        description="Extract old card descriptions from game version PCK files. Compares across versions and only saves descriptions that differ."
    )
    parser.add_argument("pck_paths", nargs="+", help="Path(s) to the game's .pck file(s)")
    parser.add_argument("--output-dir", "-o", default="RevertAnthony/localization", 
                        help="Output directory for localization files")
    parser.add_argument("--cards", "-c", nargs="+", 
                        default=None,
                        help="Card slugs to extract (default: auto-detect from RevertAnthony.cs)")
    parser.add_argument("--languages", "-l", nargs="+", 
                        default=None,
                        help="Languages to extract (default: auto-detect all)")
    
    args = parser.parse_args()
    
    output_dir = Path(args.output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)
    
    # Auto-detect cards if not provided
    card_slugs = args.cards
    if card_slugs is None:
        card_slugs = find_supported_cards()
    
    # Extract localizations from all PCK files
    all_version_data = {}  # version_label -> {lang: {table: {key: value}}}
    
    for pck_path in args.pck_paths:
        pck_path = Path(pck_path)
        if not pck_path.exists():
            print(f"Error: PCK file not found: {pck_path}", file=sys.stderr)
            return 1
        
        version_str, version_label = detect_version(pck_path)
        if version_label is None:
            version_label = f"V{pck_path.stem}"
            version_str = pck_path.stem
            print(f"Warning: Could not auto-detect version for {pck_path}, using: {version_label}", file=sys.stderr)
        else:
            print(f"Detected version: {version_str} → slug: {version_label}")
        
        print(f"Extracting localizations from {pck_path}...")
        localizations = extract_localizations(pck_path, args.languages)
        all_version_data[version_label] = localizations
    
    # Collect descriptions for each card across all versions
    # card_slug -> lang -> version_label -> description_text
    card_descriptions = {}
    
    for version_label, localizations in all_version_data.items():
        for card_slug in card_slugs:
            loc_key = card_slug.replace("-", "_").upper()
            desc_key = f"{loc_key}.description"
            
            if card_slug not in card_descriptions:
                card_descriptions[card_slug] = {}
            
            for lang, tables in localizations.items():
                if lang not in card_descriptions[card_slug]:
                    card_descriptions[card_slug][lang] = {}
                
                if "cards" in tables and desc_key in tables["cards"]:
                    card_descriptions[card_slug][lang][version_label] = tables["cards"][desc_key]
    
    # Find which descriptions differ across versions
    # We keep descriptions for a version only if they differ from at least one other version
    changed_descriptions = {}  # lang -> {new_key: description_text}
    
    for card_slug, lang_data in card_descriptions.items():
        loc_key = card_slug.replace("-", "_").upper()
        
        for lang, version_data in lang_data.items():
            if len(version_data) < 2:
                # Can't compare with less than 2 versions
                continue
            
            # Check if any version differs from another
            descriptions = list(version_data.values())
            all_same = all(d == descriptions[0] for d in descriptions)
            
            if all_same:
                # Descriptions are identical across all versions - don't save any
                continue
            
            # Descriptions differ - save all versions
            if lang not in changed_descriptions:
                changed_descriptions[lang] = {}
            
            for version_label, desc_text in version_data.items():
                new_key = f"{loc_key}_{version_label}.description"
                changed_descriptions[lang][new_key] = desc_text
    
    if not changed_descriptions:
        print("No description differences found across versions. Nothing to save.")
        return 0
    
    # Write localization files
    total_saved = 0
    for lang, entries in changed_descriptions.items():
        if not entries:
            continue
        
        lang_dir = output_dir / lang
        lang_dir.mkdir(exist_ok=True)
        
        output_path = lang_dir / "cards.json"
        
        with open(output_path, "w", encoding="utf-8") as f:
            json.dump(entries, f, ensure_ascii=False, indent=2, sort_keys=True)
        
        print(f"Wrote {len(entries)} changed descriptions to {output_path}")
        total_saved += len(entries)
    
    print(f"\nTotal: {total_saved} changed descriptions saved across {len(changed_descriptions)} languages")
    return 0


if __name__ == "__main__":
    sys.exit(main())
