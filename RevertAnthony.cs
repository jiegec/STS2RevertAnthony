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
    public string Character { get; set; }
    public List<string> OldVersions { get; set; }

    public SupportedCard(string slug, string character, params string[] oldVersions)
    {
        Slug = slug;
        Character = character;
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
        new SupportedCard("acrobatics", "SILENT", "v0.99.1"),
        new SupportedCard("alignment", "REGENT", "v0.99.1"),
        new SupportedCard("anticipate", "SILENT", "v0.99.1"),
        new SupportedCard("arsenal", "REGENT", "v0.99.1"),
        new SupportedCard("banshees-cry", "NECROBINDER", "v0.99.1"),
        new SupportedCard("begone", "REGENT", "v0.99.1"),
        new SupportedCard("believe-in-you", "COLORLESS", "v0.99.1"),
        new SupportedCard("blade-of-ink", "SILENT", "v0.99.1"),
        new SupportedCard("borrowed-time", "NECROBINDER", "v0.99.1"),
        new SupportedCard("break", "IRONCLAD", "v0.99.1"),
        new SupportedCard("bundle-of-joy", "REGENT", "v0.99.1"),
        new SupportedCard("celestial-might", "REGENT", "v0.99.1"),
        new SupportedCard("charge", "REGENT", "v0.99.1"),
        new SupportedCard("cinder", "IRONCLAD", "v0.99.1"),
        new SupportedCard("collision-course", "REGENT", "v0.99.1"),
        new SupportedCard("colossus", "IRONCLAD", "v0.99.1"),
        new SupportedCard("corrosive-wave", "SILENT", "v0.99.1"),
        new SupportedCard("danse-macabre", "NECROBINDER", "v0.99.1"),
        new SupportedCard("debilitate", "NECROBINDER", "v0.99.1"),
        new SupportedCard("defy", "NECROBINDER", "v0.99.1"),
        new SupportedCard("dirge", "NECROBINDER", "v0.99.1"),
        new SupportedCard("dominate", "IRONCLAD", "v0.99.1"),
        new SupportedCard("eternal-armor", "COLORLESS", "v0.99.1"),
        new SupportedCard("expect-a-fight", "IRONCLAD", "v0.99.1"),
        new SupportedCard("falling-star", "REGENT", "v0.99.1"),
        new SupportedCard("fight-me", "IRONCLAD", "v0.99.1"),
        new SupportedCard("flick-flack", "SILENT", "v0.99.1"),
        new SupportedCard("follow-through", "SILENT", "v0.99.1"),
        new SupportedCard("folly", "NECROBINDER", "v0.99.1"),
        new SupportedCard("forgotten-ritual", "IRONCLAD", "v0.99.1"),
        new SupportedCard("gather-light", "REGENT", "v0.99.1"),
        new SupportedCard("glitterstream", "REGENT", "v0.99.1"),
        new SupportedCard("glow", "REGENT", "v0.99.1"),
        new SupportedCard("grand-finale", "SILENT", "v0.99.1"),
        new SupportedCard("grave-warden", "NECROBINDER", "v0.99.1"),
        new SupportedCard("guiding-star", "REGENT", "v0.99.1"),
        new SupportedCard("heirloom-hammer", "REGENT", "v0.99.1"),
        new SupportedCard("hemokinesis", "IRONCLAD", "v0.99.1"),
        new SupportedCard("hidden-gem", "COLORLESS", "v0.99.1"),
        new SupportedCard("hotfix", "DEFECT", "v0.99.1"),
        new SupportedCard("huddle-up", "COLORLESS", "v0.99.1"),
        new SupportedCard("i-am-invincible", "REGENT", "v0.99.1"),
        new SupportedCard("kingly-kick", "REGENT", "v0.99.1"),
        new SupportedCard("kingly-punch", "REGENT", "v0.99.1"),
        new SupportedCard("leading-strike", "SILENT", "v0.99.1"),
        new SupportedCard("memento-mori", "SILENT", "v0.99.1"),
        new SupportedCard("minion-dive-bomb", "REGENT", "v0.99.1"),
        new SupportedCard("minion-strike", "REGENT", "v0.99.1"),
        new SupportedCard("neows-fury", "ANCIENT", "v0.99.1"),
        new SupportedCard("parry", "REGENT", "v0.99.1"),
        new SupportedCard("patter", "REGENT", "v0.99.1"),
        new SupportedCard("pinpoint", "SILENT", "v0.99.1"),
        new SupportedCard("production", "COLORLESS", "v0.99.1"),
        new SupportedCard("refine-blade", "REGENT", "v0.99.1"),
        new SupportedCard("rip-and-tear", "DEFECT", "v0.99.1"),
        new SupportedCard("sculpting-strike", "NECROBINDER", "v0.99.1"),
        new SupportedCard("seeker-strike", "COLORLESS", "v0.99.1"),
        new SupportedCard("seance", "NECROBINDER", "v0.99.1"),
        new SupportedCard("serpent-form", "SILENT", "v0.99.1"),
        new SupportedCard("skewer", "SILENT", "v0.99.1"),
        new SupportedCard("solar-strike", "REGENT", "v0.99.1"),
        new SupportedCard("speedster", "SILENT", "v0.99.1"),
        new SupportedCard("sword-sage", "REGENT", "v0.99.1"),
        new SupportedCard("spite", "IRONCLAD", "v0.99.1"),
        new SupportedCard("spoils-of-battle", "REGENT", "v0.99.1"),
        new SupportedCard("stoke", "IRONCLAD", "v0.99.1"),
        new SupportedCard("tremble", "IRONCLAD", "v0.99.1"),
        new SupportedCard("untouchable", "SILENT", "v0.99.1"),
        new SupportedCard("void-form", "REGENT", "v0.99.1"),
        new SupportedCard("voltaic", "DEFECT", "v0.99.1"),
        new SupportedCard("wrought-in-war", "REGENT", "v0.99.1"),
    };

    public static string SlugToLocKey(string slug) => slug.Replace("-", "_").ToUpperInvariant();

    public static bool IsVersion(string cardSlug, string version) =>
        CardVersions.TryGetValue(cardSlug, out var currentVersion) && currentVersion == version;

    private static bool _batchOperationInProgress;

    public static void ModLoaded()
    {
        Log.Info("RevertAnthony: Mod loading...");

        LoadConfig();
        _harmony = new Harmony("RevertAnthony");
        _harmony.PatchAll(typeof(RevertAnthony).Assembly);
        Log.Info("RevertAnthony: Harmony patches applied");

        DeferredLogPatches();
        DeferredRegisterModConfig();
    }

    static void ClearAllCanonicalCaches()
    {
        try
        {
            var dynField = typeof(CardModel).GetField("_dynamicVars", BindingFlags.NonPublic | BindingFlags.Instance);
            var energyField = typeof(CardModel).GetField("_energyCost", BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var card in ModelDb.AllCards)
            {
                dynField?.SetValue(card, null);
                energyField?.SetValue(card, null);
            }
            Log.Info("RevertAnthony: Cleared all canonical caches");
        }
        catch (Exception ex)
        {
            Log.Error($"RevertAnthony: Failed to clear all caches: {ex}");
        }
    }

    static void DeferredLogPatches()
    {
        var tree = (SceneTree)Engine.GetMainLoop();
        Action callback = null;
        callback = () =>
        {
            tree.ProcessFrame -= callback;
            try
            {
                var allMethods = Harmony.GetAllPatchedMethods();
                Log.Info("RevertAnthony: === All Harmony Patches in Game ===");

                var modPatchCounts = new Dictionary<string, int>();
                int totalPatchedMethods = 0;

                foreach (var method in allMethods)
                {
                    var patchInfo = Harmony.GetPatchInfo(method);
                    if (patchInfo == null) continue;

                    totalPatchedMethods++;
                    var owners = patchInfo.Owners.ToList();
                    string methodName = $"{method.DeclaringType?.Name}.{method.Name}";
                    string prefixes = patchInfo.Prefixes != null ? string.Join(", ", patchInfo.Prefixes.Select(p => $"{p.owner}({p.priority})")) : "none";
                    string postfixes = patchInfo.Postfixes != null ? string.Join(", ", patchInfo.Postfixes.Select(p => $"{p.owner}({p.priority})")) : "none";
                    string transpilers = patchInfo.Transpilers != null ? string.Join(", ", patchInfo.Transpilers.Select(p => $"{p.owner}({p.priority})")) : "none";

                    Log.Info($"  [PATCH] {methodName}");
                    Log.Info($"    Owners: {string.Join(", ", owners)}");
                    if (patchInfo.Prefixes != null && patchInfo.Prefixes.Count > 0)
                        Log.Info($"    Prefixes: {prefixes}");
                    if (patchInfo.Postfixes != null && patchInfo.Postfixes.Count > 0)
                        Log.Info($"    Postfixes: {postfixes}");
                    if (patchInfo.Transpilers != null && patchInfo.Transpilers.Count > 0)
                        Log.Info($"    Transpilers: {transpilers}");

                    foreach (var owner in owners.Distinct())
                    {
                        if (!modPatchCounts.ContainsKey(owner))
                            modPatchCounts[owner] = 0;
                        modPatchCounts[owner]++;
                    }
                }

                Log.Info("RevertAnthony: === Patch Summary ===");
                Log.Info($"  Total patched methods: {totalPatchedMethods}");
                foreach (var kvp in modPatchCounts.OrderByDescending(x => x.Value))
                {
                    Log.Info($"  {kvp.Key}: {kvp.Value} methods");
                }
                Log.Info("RevertAnthony: === End Patch Report ===");
            }
            catch (Exception ex)
            {
                Log.Warn($"RevertAnthony: Failed to enumerate patches: {ex}");
            }
        };
        tree.ProcessFrame += callback;
    }

    static void DeferredRegisterModConfig()
    {
        var tree = (SceneTree)Engine.GetMainLoop();
        Action callback = null;
        callback = () =>
        {
            tree.ProcessFrame -= callback;
            RegisterModConfigViaReflection();
            TrySubscribeLocaleChange();
        };
        tree.ProcessFrame += callback;
    }

    static void TrySubscribeLocaleChange()
    {
        try
        {
            var instance = LocManager.Instance;
            if (instance != null)
            {
                instance.SubscribeToLocaleChange(OnLocaleChanged);
                Log.Info("RevertAnthony: Subscribed to locale changes");
            }
            else
            {
                Log.Error("RevertAnthony: LocManager.Instance is null, cannot subscribe to locale changes");
            }
        }
        catch (Exception e)
        {
            Log.Error($"RevertAnthony: Failed to subscribe to locale change: {e}");
        }
    }

    static void OnLocaleChanged()
    {
        Log.Info("RevertAnthony: Locale changed, re-registering ModConfig");
        RegisterModConfigViaReflection();
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
                string description = null,
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
                if (description != null)
                    entryType.GetProperty("Description")?.SetValue(entry, description);
                if (descriptions != null)
                    entryType.GetProperty("Descriptions")?.SetValue(entry, descriptions);
                if (onChanged != null)
                    entryType.GetProperty("OnChanged")?.SetValue(entry, onChanged);
                return entry;
            }

            object GetConfigType(string name) => Enum.Parse(configType, name);

            // Collect all unique old versions for batch operations
            var allVersions = SupportedCards.SelectMany(c => c.OldVersions).Distinct().ToList();
            var batchOptions = new List<string> { "Unchanged", Latest };
            batchOptions.AddRange(allVersions);
            const string Unchanged = "Unchanged";

            // Global batch operation
            entries.Add(MakeEntry("", "Batch Operations", GetConfigType("Header"),
                labels: new()
                {
                    { "zhs", "批量设置" }
                }
            ));

            setValueMethod.Invoke(null, new object[] { "RevertAnthony", "batch_all_version", Unchanged });
            entries.Add(MakeEntry("batch_all_version", "Set all cards to",
                GetConfigType("Dropdown"),
                defaultValue: Unchanged,
                options: batchOptions.ToArray(),
                labels: new()
                {
                    { "zhs", "将所有卡牌设置为" }
                },
                description: "Apply the selected version to ALL cards at once (only cards that support it)",
                descriptions: new()
                {
                    { "zhs", "将所选版本批量应用到所有卡牌（仅支持该版本的卡牌）" }
                },
                onChanged: (value) =>
                {
                    var version = (string)value;
                    if (version == Unchanged) return;
                    _batchOperationInProgress = true;
                    try
                    {
                        foreach (var card in SupportedCards)
                        {
                            if (version != Latest && !card.OldVersions.Contains(version))
                                continue;
                            CardVersions[card.Slug] = version;
                            setValueMethod.Invoke(null, new object[] { "RevertAnthony", $"card_{card.Slug}_version", version });
                            ClearAllCanonicalCaches();
                        }
                        SaveConfig();
                    }
                    finally
                    {
                        _batchOperationInProgress = false;
                        var tree = (SceneTree)Engine.GetMainLoop();
                        Action resetCallback = null;
                        resetCallback = () =>
                        {
                            tree.ProcessFrame -= resetCallback;
                            setValueMethod.Invoke(null, new object[] { "RevertAnthony", "batch_all_version", Unchanged });
                        };
                        tree.ProcessFrame += resetCallback;
                    }
                }));
            entries.Add(MakeEntry("", "", GetConfigType("Separator")));

            // Group cards by character
            var cardsByCharacter = SupportedCards
                .GroupBy(c => c.Character)
                .OrderBy(g => g.Key);

            foreach (var group in cardsByCharacter)
            {
                var characterKey = group.Key;
                var charLoc = LocString.GetIfExists("characters", characterKey + ".title");
                var characterLabel = charLoc != null ? charLoc.GetFormattedText() : characterKey;
                var capturedGroup = group.ToList();

                // Add character header
                entries.Add(MakeEntry("", characterLabel, GetConfigType("Header")));

                // Per-character batch operation
                setValueMethod.Invoke(null, new object[] { "RevertAnthony", $"batch_{characterKey}_version", Unchanged });
                entries.Add(MakeEntry($"batch_{characterKey}_version", $"Set all {characterLabel} cards to",
                    GetConfigType("Dropdown"),
                    defaultValue: Unchanged,
                    options: batchOptions.ToArray(),
                    labels: new()
                    {
                        { "zhs", $"将所有{characterLabel}卡牌设置为" }
                    },
                    description: $"Apply the selected version to all {characterLabel} cards at once (only cards that support it)",
                    descriptions: new()
                    {
                        { "zhs", $"将所选版本批量应用到所有{characterLabel}卡牌（仅支持该版本的卡牌）" }
                    },
                    onChanged: (value) =>
                    {
                        var version = (string)value;
                        if (version == Unchanged) return;
                        _batchOperationInProgress = true;
                        try
                        {
                            foreach (var card in capturedGroup)
                            {
                                if (version != Latest && !card.OldVersions.Contains(version))
                                    continue;
                                CardVersions[card.Slug] = version;
                                setValueMethod.Invoke(null, new object[] { "RevertAnthony", $"card_{card.Slug}_version", version });
                                ClearAllCanonicalCaches();
                            }
                            SaveConfig();
                        }
                        finally
                        {
                            _batchOperationInProgress = false;
                            var capturedCharacterKey = characterKey;
                            var tree = (SceneTree)Engine.GetMainLoop();
                            Action resetCallback = null;
                            resetCallback = () =>
                            {
                                tree.ProcessFrame -= resetCallback;
                                setValueMethod.Invoke(null, new object[] { "RevertAnthony", $"batch_{capturedCharacterKey}_version", Unchanged });
                            };
                            tree.ProcessFrame += resetCallback;
                        }
                    }));

                foreach (var card in group)
                {
                    var slug = card.Slug;
                    var options = new List<string> { Latest };
                    options.AddRange(card.OldVersions);
                    var versionList = string.Join(", ", card.OldVersions);

                    // Use the game's built-in localization for card names
                    string locKey = SlugToLocKey(slug);
                    var titleLoc = new LocString("cards", locKey + ".title");
                    string cardLabel = titleLoc.GetFormattedText();

                    entries.Add(MakeEntry($"card_{slug}_version", cardLabel,
                        GetConfigType("Dropdown"),
                        defaultValue: CardVersions.GetValueOrDefault(slug, Latest),
                        options: options.ToArray(),
                        description: $"Use {Latest} (current) or {versionList} (old) version of {cardLabel}",
                        descriptions: new()
                        {
                            { "zhs", $"使用 {Latest}（当前）或 {versionList}（旧版）版本的 {cardLabel}" }
                        },
                        onChanged: (value) =>
                        {
                            if (_batchOperationInProgress) return;
                            var version = (string)value;
                            CardVersions[slug] = version;
                            SaveConfig();
                            ClearAllCanonicalCaches();
                        }));
                }
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
        string modsConfigPath = Path.Combine(modsPath, "RevertAnthonyConfig.json");

        if (File.Exists(modsConfigPath))
        {
            return modsConfigPath;
        }

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
