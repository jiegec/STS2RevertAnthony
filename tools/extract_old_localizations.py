#!/usr/bin/env python3
"""
Extract old card descriptions from a game version's PCK and save them for RevertAnthony mod.

Usage:
    python3 extract_old_localizations.py <path_to_old_game_pck> [--output-dir <path>] [--version <version_name>]

Example:
    python3 extract_old_localizations.py "/path/to/Slay the Spire 2.pck" --output-dir ../RevertAnthony/localization --version v0.99.1
"""

import argparse
import json
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
    """Extract localization tables from PCK. Returns {language: {table_name: {key: value}}}."""
    if languages is None:
        languages = ["eng", "zhs"]
    
    file_base, entries = parse_pck(pck_path)
    
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


def filter_card_descriptions(localizations, card_slugs, version_label):
    """Extract only card descriptions for specified card slugs, renaming keys."""
    result = {}
    
    for card_slug in card_slugs:
        # Convert slug to uppercase key format: borrowed-time -> BORROWED_TIME
        loc_key = card_slug.replace("-", "_").upper()
        desc_key = f"{loc_key}.description"
        new_key = f"{loc_key}_{version_label}.description"
        
        for lang, tables in localizations.items():
            if "cards" not in tables:
                continue
            if desc_key in tables["cards"]:
                if lang not in result:
                    result[lang] = {}
                result[lang][new_key] = tables["cards"][desc_key]
    
    return result


def main():
    parser = argparse.ArgumentParser(
        description="Extract old card descriptions from a game version and save them for RevertAnthony mod"
    )
    parser.add_argument("pck_path", help="Path to the game's .pck file")
    parser.add_argument("--output-dir", "-o", default="RevertAnthony/localization", 
                        help="Output directory for localization files")
    parser.add_argument("--version", "-v", default=None, 
                        help="Version label for keys (auto-detected if not provided)")
    parser.add_argument("--cards", "-c", nargs="+", 
                        default=["borrowed-time", "hemokinesis", "acrobatics", "skewer"],
                        help="Card slugs to extract (default: borrowed-time hemokinesis acrobatics skewer)")
    parser.add_argument("--languages", "-l", nargs="+", 
                        default=["eng", "zhs"],
                        help="Languages to extract (default: eng zhs)")
    
    args = parser.parse_args()
    
    pck_path = Path(args.pck_path)
    if not pck_path.exists():
        print(f"Error: PCK file not found: {pck_path}", file=sys.stderr)
        return 1
    
    # Auto-detect version from release_info.json
    version_label = args.version
    if version_label is None:
        release_info_path = pck_path.parent / "release_info.json"
        if release_info_path.exists():
            try:
                with open(release_info_path, "r", encoding="utf-8") as f:
                    release_info = json.load(f)
                    detected_version = release_info.get("version", "")
                    if detected_version:
                        # Convert "v0.99.1" → "V0991"
                        version_label = "V" + detected_version.lstrip("vV").replace(".", "")
                        print(f"Auto-detected version: {detected_version} → slug: {version_label}")
            except (json.JSONDecodeError, OSError) as e:
                print(f"Warning: Could not read release_info.json: {e}", file=sys.stderr)
        
        if version_label is None:
            version_label = "OLD"
            print(f"Warning: Could not auto-detect version, using default: {version_label}", file=sys.stderr)
    
    output_dir = Path(args.output_dir)
    output_dir.mkdir(parents=True, exist_ok=True)
    
    print(f"Extracting localizations from {pck_path}...")
    localizations = extract_localizations(pck_path, args.languages)
    
    print(f"Filtering {len(args.cards)} cards...")
    filtered = filter_card_descriptions(localizations, args.cards, version_label)
    
    if not filtered:
        print("Warning: No card descriptions found for the specified cards", file=sys.stderr)
        return 1
    
    # Write localization files
    for lang, entries in filtered.items():
        if not entries:
            continue
        
        lang_dir = output_dir / lang
        lang_dir.mkdir(exist_ok=True)
        
        output_path = lang_dir / "cards.json"
        with open(output_path, "w", encoding="utf-8") as f:
            json.dump(entries, f, ensure_ascii=False, indent=2)
        
        print(f"Wrote {len(entries)} entries to {output_path}")
    
    return 0


if __name__ == "__main__":
    sys.exit(main())
