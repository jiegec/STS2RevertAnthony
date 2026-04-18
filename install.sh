#!/bin/bash
set -x

OS="$(uname -s)"

case "$OS" in
    Linux*)
        mkdir -p ~/.steam/steam/steamapps/common/Slay\ the\ Spire\ 2/mods/RevertAnthony
        cp -r dist/* ~/.steam/steam/steamapps/common/Slay\ the\ Spire\ 2/mods/RevertAnthony
        ;;
    Darwin*)
        mkdir -p ~/Library/Application\ Support/Steam/steamapps/common/Slay\ the\ Spire\ 2/SlayTheSpire2.app/Contents/MacOS/mods/RevertAnthony
        cp -r dist/* ~/Library/Application\ Support/Steam/steamapps/common/Slay\ the\ Spire\ 2/SlayTheSpire2.app/Contents/MacOS/mods/RevertAnthony
        ;;
    *)
        echo "Unknown operating system: $OS"
        exit 1
        ;;
esac

