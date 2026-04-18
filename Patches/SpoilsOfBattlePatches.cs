using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// SpoilsOfBattle v0.99.1 vs current — complete rework
// v0.99.1: Forge 10(15), OnPlay: forge only, OnUpgrade → +5 forge, ExtraHoverTips → Forge
// Current:  Forge 5(8), Cards 2. OnPlay: forge + draw 2, OnUpgrade → +3 forge

[HarmonyPatch(typeof(SpoilsOfBattle), "get_CanonicalVars")]
static class SpoilsOfBattle_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("spoils-of-battle", "v0.99.1"))
        {
            // v0.99.1: forge 10 only (current: forge 5 + draw 2)
            __result = new DynamicVar[]
            {
                new ForgeVar(10),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(SpoilsOfBattle), "OnPlay")]
static class SpoilsOfBattle_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, SpoilsOfBattle __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("spoils-of-battle", "v0.99.1"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    // v0.99.1: forge only (current: forge + draw 2 cards)
    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, SpoilsOfBattle instance)
    {
        await ForgeCmd.Forge(instance.DynamicVars.Forge.IntValue, instance.Owner, instance);
    }
}

[HarmonyPatch(typeof(SpoilsOfBattle), "OnUpgrade")]
static class SpoilsOfBattle_OnUpgrade_Patch
{
    static bool Prefix(SpoilsOfBattle __instance)
    {
        if (RevertAnthony.IsVersion("spoils-of-battle", "v0.99.1"))
        {
            // v0.99.1: +5 forge (current: +3 forge)
            __instance.DynamicVars.Forge.UpgradeValueBy(5m);
            return false;
        }
        return true;
    }
}
