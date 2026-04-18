#!/bin/bash
set -x

OS="$(uname -s)"

case "$OS" in
    Linux*)
        cp ~/.steam/steam/steamapps/common/Slay\ the\ Spire\ 2/data_sts2_linuxbsd_x86_64/sts2.dll .
        cp ~/.steam/steam/steamapps/common/Slay\ the\ Spire\ 2/data_sts2_linuxbsd_x86_64/0Harmony.dll .
        ../Godot_v4.5.1-stable_mono_linux_x86_64/Godot_v4.5.1-stable_mono_linux.x86_64 --build-solutions --quit --headless
        rm -rf dist
        mkdir -p dist
        cp ./.godot/mono/temp/bin/Debug/RevertAnthony.dll dist/
        ../Godot_v4.5.1-stable_mono_linux_x86_64/Godot_v4.5.1-stable_mono_linux.x86_64 --export-pack "Windows Desktop" dist/RevertAnthony.pck --headless
        cp RevertAnthony.json dist/RevertAnthony.json
        ;;
    Darwin*)
        cp ~/Library/Application\ Support/steam/steamapps/common/Slay\ the\ Spire\ 2/SlayTheSpire2.app/Contents/Resources/data_sts2_macos_arm64/sts2.dll .
        cp ~/Library/Application\ Support/steam/steamapps/common/Slay\ the\ Spire\ 2/SlayTheSpire2.app/Contents/Resources/data_sts2_macos_arm64/0Harmony.dll .
        /Applications/Godot_mono.app/Contents/MacOS/Godot --build-solutions --quit --headless
        rm -rf dist
        mkdir -p dist
        cp ./.godot/mono/temp/bin/Debug/RevertAnthony.dll dist/
        /Applications/Godot_mono.app/Contents/MacOS/Godot --export-pack "Windows Desktop" dist/RevertAnthony.pck --headless
        cp RevertAnthony.json dist/RevertAnthony.json
        ;;
    *)
        echo "Unknown operating system: $OS"
        exit 1
        ;;
esac

