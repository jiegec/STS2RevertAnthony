#!/bin/bash
set -x -e

OS="$(uname -s)"

case "$OS" in
    Linux*)
        STS2_DLL="$HOME/.steam/steam/steamapps/common/Slay the Spire 2/data_sts2_linuxbsd_x86_64/sts2.dll"
        HARMONY_DLL="$HOME/.steam/steam/steamapps/common/Slay the Spire 2/data_sts2_linuxbsd_x86_64/0Harmony.dll"
        GODOT="../Godot_v4.5.1-stable_mono_linux_x86_64/Godot_v4.5.1-stable_mono_linux.x86_64"
        ;;
    Darwin*)
        STS2_DLL="$HOME/Library/Application Support/steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/Resources/data_sts2_macos_arm64/sts2.dll"
        HARMONY_DLL="$HOME/Library/Application Support/steam/steamapps/common/Slay the Spire 2/SlayTheSpire2.app/Contents/Resources/data_sts2_macos_arm64/sts2.dll"
        GODOT="/Applications/Godot_mono.app/Contents/MacOS/Godot"
        ;;
    *)
        echo "Unknown operating system: $OS"
        exit 1
        ;;
esac

cp "$STS2_DLL" .
cp "$HARMONY_DLL" .
$GODOT --build-solutions --quit --headless --verbose
rm -rf dist
mkdir -p dist
cp ./.godot/mono/temp/bin/Debug/RevertAnthony.dll dist/
$GODOT --export-pack "Windows Desktop" dist/RevertAnthony.pck --headless
cp RevertAnthony.json dist/RevertAnthony.json

VERSION=$(jq -r ".version" RevertAnthony.json)
rm -f RevertAnthony-v$VERSION.zip
cd dist && zip -r ../RevertAnthony-v$VERSION.zip .
