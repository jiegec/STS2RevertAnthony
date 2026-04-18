using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RevertAnthony;

// Dominate v0.99.1 vs current — significant rework
// v0.99.1: Exhaust keyword, Vars = StrengthPerVulnerable 1, no Vulnerable apply
//          ShouldGlow: check if any enemy has Vulnerable. OnUpgrade → remove Exhaust
//          OnPlay: gain Strength equal to target's Vulnerable stacks
// Current:  Exhaust keyword, Vars = Vulnerable 1 + StrengthPerVulnerable 1
//          ShouldGlow: same check. OnUpgrade → +1 Vulnerable. OnPlay: apply 1 Vulnerable, then gain Strength

[HarmonyPatch(typeof(CardModel), "ShouldGlowGoldInternal", MethodType.Getter)]
static class Dominate_ShouldGlowGoldInternal_Patch
{
    static void Postfix(CardModel __instance, ref bool __result)
    {
        if (__instance is Dominate && RevertAnthony.IsVersion("dominate", "v0.99.1"))
        {
            // v0.99.1: glow if any hittable enemy has Vulnerable
            __result = __instance.CombatState?.HittableEnemies.Any((Creature e) => e.HasPower<VulnerablePower>()) ?? false;
        }
    }
}

[HarmonyPatch(typeof(Dominate), "get_CanonicalVars")]
static class Dominate_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("dominate", "v0.99.1"))
        {
            // v0.99.1: StrengthPerVulnerable only (current: also applies 1 Vulnerable)
            __result = new DynamicVar[]
            {
                new DynamicVar("StrengthPerVulnerable", 1m),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Dominate), "OnPlay")]
static class Dominate_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, Dominate __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("dominate", "v0.99.1"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    // v0.99.1: gain Strength equal to target's existing Vulnerable (no new Vulnerable applied)
    // Current:  apply 1 Vulnerable, then gain Strength equal to target's total Vulnerable
    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, Dominate instance)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        int strengthToApply = cardPlay.Target.GetPower<VulnerablePower>()?.Amount ?? 0;
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<StrengthPower>(instance.Owner.Creature, strengthToApply, instance.Owner.Creature, instance);
    }
}

[HarmonyPatch(typeof(Dominate), "OnUpgrade")]
static class Dominate_OnUpgrade_Patch
{
    static bool Prefix(Dominate __instance)
    {
        if (RevertAnthony.IsVersion("dominate", "v0.99.1"))
        {
            // v0.99.1: remove Exhaust (current: +1 Vulnerable)
            __instance.RemoveKeyword(CardKeyword.Exhaust);
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class Dominate_Description_Patch
{
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is Dominate && RevertAnthony.IsVersion("dominate", "v0.99.1"))
            __result = new LocString("cards", "DOMINATE_V0991.description");
    }
}
