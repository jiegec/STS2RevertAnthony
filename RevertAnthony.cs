using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

public class RevertAnthonyConfig
{
    [JsonPropertyName("schema_version")]
    public int SchemaVersion { get; set; } = 1;

    [JsonPropertyName("card_versions")]
    public Dictionary<string, string> CardVersions { get; set; } = new();
}

public class SupportedCard
{
    public string Slug { get; set; }
    public string DisplayName { get; set; }
    public string Category { get; set; }
    public List<string> OldVersions { get; set; }

    public SupportedCard(string slug, string displayName, params string[] oldVersions)
    {
        Slug = slug;
        DisplayName = displayName;
        Category = "Cards";
        OldVersions = new List<string>(oldVersions);
    }
}

[ModInitializer("ModLoaded")]
public static class RevertAnthony
{
    public const string Latest = "Latest";

    public static Dictionary<string, string> CardVersions { get; private set; } = new();

    private static string _configFilePath = null;
    private static Harmony _harmony;

    public static readonly List<SupportedCard> SupportedCards = new()
    {
        new SupportedCard("borrowed-time", "Borrowed Time", "v0.99.1"),
        new SupportedCard("hemokinesis", "Hemokinesis", "v0.99.1"),
        new SupportedCard("acrobatics", "Acrobatics", "v0.99.1"),
        new SupportedCard("skewer", "Skewer", "v0.99.1"),
    };

    public static string SlugToLocKey(string slug) => slug.Replace("-", "_").ToUpperInvariant();

    public static bool IsVersion(string cardSlug, string version) =>
        CardVersions.TryGetValue(cardSlug, out var currentVersion) && currentVersion == version;

    public static void ModLoaded()
    {
        Log.Info("RevertAnthony: Mod loading...");

        LoadConfig();
        _harmony = new Harmony("RevertAnthony");
        _harmony.PatchAll(typeof(RevertAnthony).Assembly);
        Log.Info("RevertAnthony: Harmony patches applied");

        ClearAllCanonicalCaches();

        DeferredRegisterModConfig();
    }

    static void ClearAllCanonicalCaches()
    {
        foreach (var card in SupportedCards)
        {
            if (CardVersions.TryGetValue(card.Slug, out var version) && card.OldVersions.Contains(version))
            {
                ClearCanonicalCache(card.Slug);
            }
        }
    }

    public static void ClearCanonicalCache(string cardSlug)
    {
        try
        {
            var id = new ModelId("card-model", cardSlug);
            var canonical = ModelDb.GetByIdOrNull<CardModel>(id);
            if (canonical == null) return;

            var dynField = typeof(CardModel).GetField("_dynamicVars", BindingFlags.NonPublic | BindingFlags.Instance);
            var energyField = typeof(CardModel).GetField("_energyCost", BindingFlags.NonPublic | BindingFlags.Instance);
            dynField?.SetValue(canonical, null);
            energyField?.SetValue(canonical, null);
            Log.Info($"RevertAnthony: Cleared canonical cache for {cardSlug}");
        }
        catch (Exception ex)
        {
            Log.Error($"RevertAnthony: Failed to clear cache for {cardSlug}: {ex.Message}");
        }
    }

    static void DeferredRegisterModConfig()
    {
        var tree = (SceneTree)Engine.GetMainLoop();
        Action callback = null;
        callback = () =>
        {
            tree.ProcessFrame -= callback;
            RegisterModConfigViaReflection();
        };
        tree.ProcessFrame += callback;
    }

    static void RegisterModConfigViaReflection()
    {
        try
        {
            var apiType = Type.GetType("ModConfig.ModConfigApi, ModConfig");
            var entryType = Type.GetType("ModConfig.ConfigEntry, ModConfig");
            var configType = Type.GetType("ModConfig.ConfigType, ModConfig");
            var managerType = Type.GetType("ModConfig.ModConfigManager, ModConfig");

            if (apiType == null || entryType == null || configType == null || managerType == null)
            {
                Log.Info("RevertAnthony: ModConfig not found, skipping GUI registration");
                return;
            }

            var setValueMethod = apiType.GetMethod("SetValue")!;
            foreach (var card in SupportedCards)
            {
                var currentValue = CardVersions.GetValueOrDefault(card.Slug, Latest);
                setValueMethod.Invoke(null, new object[] { "RevertAnthony", $"card_{card.Slug}_version", currentValue });
            }

            var save = managerType.GetMethod("SaveValues", BindingFlags.NonPublic | BindingFlags.Static)!;
            save.Invoke(null, new object[] { "RevertAnthony" });

            var entries = new List<object>();

            object MakeEntry(string key, string label, object type,
                object defaultValue = null, float min = 0, float max = 100, float step = 1,
                string format = "F0", string[] options = null,
                Dictionary<string, string> labels = null,
                Dictionary<string, string> descriptions = null,
                Action<object> onChanged = null)
            {
                var entry = Activator.CreateInstance(entryType);
                entryType.GetProperty("Key")?.SetValue(entry, key);
                entryType.GetProperty("Label")?.SetValue(entry, label);
                entryType.GetProperty("Type")?.SetValue(entry, type);
                if (defaultValue != null)
                    entryType.GetProperty("DefaultValue")?.SetValue(entry, defaultValue);
                entryType.GetProperty("Min")?.SetValue(entry, min);
                entryType.GetProperty("Max")?.SetValue(entry, max);
                entryType.GetProperty("Step")?.SetValue(entry, step);
                entryType.GetProperty("Format")?.SetValue(entry, format);
                if (options != null)
                    entryType.GetProperty("Options")?.SetValue(entry, options);
                if (labels != null)
                    entryType.GetProperty("Labels")?.SetValue(entry, labels);
                if (descriptions != null)
                    entryType.GetProperty("Descriptions")?.SetValue(entry, descriptions);
                if (onChanged != null)
                    entryType.GetProperty("OnChanged")?.SetValue(entry, onChanged);
                return entry;
            }

            object GetConfigType(string name) => Enum.Parse(configType, name);

            entries.Add(MakeEntry("", "Card Versions", GetConfigType("Header"),
                descriptions: new()
                {
                    { "en", "Select which version to use for each card" },
                    { "zhs", "为每张卡牌选择要使用的版本" }
                },
                labels: new() { { "zhs", "卡牌版本" } }));

            foreach (var card in SupportedCards)
            {
                var slug = card.Slug;
                var options = new List<string> { Latest };
                options.AddRange(card.OldVersions);
                var versionList = string.Join(", ", card.OldVersions);

                // Use the game's built-in localization for card names
                string locKey = SlugToLocKey(slug);
                var titleLoc = new LocString("cards", locKey + ".title");
                string cardLabel = titleLoc.Exists() ? titleLoc.GetFormattedText() : card.DisplayName;

                entries.Add(MakeEntry($"card_{slug}_version", card.DisplayName,
                    GetConfigType("Dropdown"),
                    defaultValue: CardVersions.GetValueOrDefault(slug, Latest),
                    options: options.ToArray(),
                    labels: new()
                    {
                        { "zhs", cardLabel }
                    },
                    descriptions: new()
                    {
                        { "en", $"Use {Latest} (current) or {versionList} (old) version of {card.DisplayName}" },
                        { "zhs", $"使用 {Latest}（当前）或 {versionList}（旧版）版本的 {cardLabel}" }
                    },
                    onChanged: (value) =>
                    {
                        var version = (string)value;
                        CardVersions[slug] = version;
                        SaveConfig();
                        if (card.OldVersions.Contains(version))
                        {
                            ClearCanonicalCache(slug);
                        }
                    }));
            }

            var entriesArray = Array.CreateInstance(entryType, entries.Count);
            for (int i = 0; i < entries.Count; i++)
            {
                entriesArray.SetValue(entries[i], i);
            }

            var registerMethod = apiType.GetMethod("Register",
                new[] { typeof(string), typeof(string), entryType.MakeArrayType() });
            registerMethod!.Invoke(null, new object[] { "RevertAnthony", "RevertAnthony", entriesArray });

            Log.Info($"RevertAnthony: Registered {entries.Count} entries with ModConfig");
        }
        catch (Exception ex)
        {
            Log.Error($"RevertAnthony: Failed to register with ModConfig: {ex.Message}");
        }
    }

    static string GetConfigFilePath()
    {
        string executablePath = OS.GetExecutablePath();
        string directoryName = Path.GetDirectoryName(executablePath);
        string modsPath = Path.Combine(directoryName, "mods");

        if (Directory.Exists(modsPath))
        {
            try
            {
                string[] dllFiles = Directory.GetFiles(modsPath, "RevertAnthony.dll", SearchOption.AllDirectories);
                if (dllFiles.Length > 0)
                {
                    string dllDirectory = Path.GetDirectoryName(dllFiles[0]);
                    return Path.Combine(dllDirectory, "RevertAnthonyConfig.json");
                }
            }
            catch { }
        }

        return modsConfigPath;
    }

    static void SaveConfig()
    {
        try
        {
            string configPath = _configFilePath;
            var configData = new RevertAnthonyConfig
            {
                SchemaVersion = 1,
                CardVersions = new Dictionary<string, string>(CardVersions)
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            };

            string json = JsonSerializer.Serialize(configData, options);
            File.WriteAllText(configPath, json);
            Log.Info($"RevertAnthony: Configuration saved to {configPath}");
        }
        catch (Exception ex)
        {
            Log.Error($"RevertAnthony: Failed to save configuration: {ex.Message}");
        }
    }

    static void LoadConfig()
    {
        try
        {
            string configPath = GetConfigFilePath();
            _configFilePath = configPath;

            if (!File.Exists(configPath))
            {
                Log.Info($"RevertAnthony: Config file not found at {configPath}, using defaults");
                return;
            }

            string json = File.ReadAllText(configPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var configData = JsonSerializer.Deserialize<RevertAnthonyConfig>(json, options);
            if (configData?.CardVersions != null)
            {
                CardVersions = configData.CardVersions;
                Log.Info($"RevertAnthony: Loaded {CardVersions.Count} card version overrides from {configPath}");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"RevertAnthony: Failed to load config file: {ex.Message}, using defaults");
        }
    }
}
