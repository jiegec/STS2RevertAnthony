using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// Spite v0.99.1 vs current — complete rework
// v0.99.1: Damage 6(+3) Cards 1. ShouldGlow: took unblocked damage this turn (player side).
//          OnPlay: deal damage; if took damage, draw 1 card. OnUpgrade → +3 damage
// Current:  Damage 5, Repeat 2(+1). ShouldGlow: lost HP this turn (simplified check).
//          OnPlay: deal damage 1 or 2 times based on HP loss. OnUpgrade → +1 repeat

[HarmonyPatch(typeof(Spite), "get_ShouldGlowGoldInternal")]
static class Spite_ShouldGlowGoldInternal_Patch
{
    static bool Prefix(ref bool __result)
    {
        if (RevertAnthony.IsVersion("spite", "v0.99.1"))
        {
            // v0.99.1: glow if player took unblocked damage this turn (checks CurrentSide == Player)
            // Current: glow if owner creature lost HP this turn (no side check)
            __result = CombatManager.Instance.History.Entries.OfType<DamageReceivedEntry>().Any((DamageReceivedEntry e) => e.HappenedThisTurn(e.Receiver?.CombatState) && e.Receiver != null && e.Result.UnblockedDamage > 0 && e.CurrentSide == CombatSide.Player);
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Spite), "get_CanonicalVars")]
static class Spite_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("spite", "v0.99.1"))
        {
            // v0.99.1: damage + draw 1 card (current: damage + repeat 2)
            __result = new DynamicVar[]
            {
                new DamageVar(6m, ValueProp.Move),
                new CardsVar(1),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Spite), "OnPlay")]
static class Spite_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, Spite __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("spite", "v0.99.1"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    // v0.99.1: deal damage once; if took damage, draw 1 card
    // Current: deal damage 1 or 2 times based on HP loss this turn
    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, Spite instance)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(instance.DynamicVars.Damage.BaseValue).FromCard(instance).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        bool tookDamageThisTurn = CombatManager.Instance.History.Entries.OfType<DamageReceivedEntry>().Any((DamageReceivedEntry e) => e.HappenedThisTurn(instance.CombatState) && e.Receiver == instance.Owner.Creature && e.Result.UnblockedDamage > 0 && e.CurrentSide == CombatSide.Player);

        if (tookDamageThisTurn)
        {
            await CardPileCmd.Draw(choiceContext, instance.DynamicVars.Cards.IntValue, instance.Owner);
        }
    }
}


[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class Spite_Description_Patch
{
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is Spite && RevertAnthony.IsVersion("spite", "v0.99.1"))
            __result = new LocString("cards", "SPITE_V0991.description");
    }
}

[HarmonyPatch(typeof(Spite), "OnUpgrade")]
static class Spite_OnUpgrade_Patch
{
    static bool Prefix(Spite __instance)
    {
        if (RevertAnthony.IsVersion("spite", "v0.99.1"))
        {
            // v0.99.1: +3 damage (current: +1 repeat)
            __instance.DynamicVars.Damage.UpgradeValueBy(3m);
            return false;
        }
        return true;
    }
}
