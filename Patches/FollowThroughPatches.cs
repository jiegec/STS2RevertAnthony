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
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// FollowThrough v0.99.1 vs current — complete rework
// v0.99.1: 1-cost Attack, Uncommon, AllEnemies, Damage 6(+2) Weak 1(+1)
//          ShouldGlow: last card played was a Skill → attack all enemies + apply Weak to all
//          ExtraHoverTips → WeakPower
// Current:  1-cost Attack, Common, AnyEnemy, Damage 7(+2), "CardCount" 5
//          ShouldGlow: hand size >= CardCount → hit target 2 times instead of 1

[HarmonyPatch(typeof(FollowThrough), "get_ShouldGlowGoldInternal")]
static class FollowThrough_ShouldGlowGoldInternal_Patch
{
    static bool Prefix(FollowThrough __instance, ref bool __result)
    {
        if (RevertAnthony.IsVersion("follow-through", "v0.99.1"))
        {
            // v0.99.1: glow if last card played was a Skill
            // Current: glow if hand size >= CardCount
            CardPlayStartedEntry cardPlayStartedEntry = CombatManager.Instance.History.CardPlaysStarted.LastOrDefault((CardPlayStartedEntry e) => e.CardPlay.Card.Owner == __instance.Owner && e.HappenedThisTurn(__instance.CombatState) && e.CardPlay.Card != __instance);
            if (cardPlayStartedEntry == null)
            {
                __result = false;
            }
            else
            {
                __result = cardPlayStartedEntry.CardPlay.Card.Type == CardType.Skill;
            }
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(FollowThrough), "get_CanonicalVars")]
static class FollowThrough_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("follow-through", "v0.99.1"))
        {
            // v0.99.1: damage 6 + weak 1 (current: damage 7 + "CardCount" 5)
            __result = new DynamicVar[]
            {
                new DamageVar(6m, ValueProp.Move),
                new PowerVar<WeakPower>(1m),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(FollowThrough), "get_ExtraHoverTips")]
static class FollowThrough_ExtraHoverTips_Patch
{
    static bool Prefix(ref IEnumerable<IHoverTip> __result)
    {
        if (RevertAnthony.IsVersion("follow-through", "v0.99.1"))
        {
            // v0.99.1: show WeakPower tooltip (current: no extra hover tips)
            __result = new IHoverTip[]
            {
                HoverTipFactory.FromPower<WeakPower>(),
            };
            return false;
        }
        return true;
    }
}

// TargetType and Rarity patches are in BaseCardModelPatches.cs

[HarmonyPatch(typeof(FollowThrough), "OnPlay")]
static class FollowThrough_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, FollowThrough __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("follow-through", "v0.99.1"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    // v0.99.1: attack all enemies; if last card was a Skill, apply Weak to all
    // Current: attack single enemy; if hand >= CardCount, hit 2 times instead of 1
    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, FollowThrough instance)
    {
        await DamageCmd.Attack(instance.DynamicVars.Damage.BaseValue).FromCard(instance).TargetingAllOpponents(instance.CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        bool wasLastCardSkill = false;
        CardPlayStartedEntry cardPlayStartedEntry = CombatManager.Instance.History.CardPlaysStarted.LastOrDefault((CardPlayStartedEntry e) => e.CardPlay.Card.Owner == instance.Owner && e.HappenedThisTurn(instance.CombatState) && e.CardPlay.Card != instance);
        if (cardPlayStartedEntry != null)
        {
            wasLastCardSkill = cardPlayStartedEntry.CardPlay.Card.Type == CardType.Skill;
        }

        if (wasLastCardSkill)
        {
            await PowerCmd.Apply<WeakPower>(instance.CombatState.HittableEnemies, instance.DynamicVars.Weak.BaseValue, instance.Owner.Creature, instance);
        }
    }
}

[HarmonyPatch(typeof(CardModel), "Rarity", MethodType.Getter)]
static class FollowThrough_Rarity_Patch
{
    static void Postfix(CardModel __instance, ref CardRarity __result)
    {
        if (__instance is FollowThrough && RevertAnthony.IsVersion("follow-through", "v0.99.1"))
            __result = CardRarity.Uncommon;
    }
}

[HarmonyPatch(typeof(CardModel), "TargetType", MethodType.Getter)]
static class FollowThrough_TargetType_Patch
{
    static void Postfix(CardModel __instance, ref TargetType __result)
    {
        if (__instance is FollowThrough && RevertAnthony.IsVersion("follow-through", "v0.99.1"))
            __result = TargetType.AllEnemies;
    }
}

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class FollowThrough_Description_Patch
{
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is FollowThrough && RevertAnthony.IsVersion("follow-through", "v0.99.1"))
            __result = new LocString("cards", "FOLLOW_THROUGH_V0991.description");
    }
}

[HarmonyPatch(typeof(FollowThrough), "OnUpgrade")]
static class FollowThrough_OnUpgrade_Patch
{
    static bool Prefix(FollowThrough __instance)
    {
        if (RevertAnthony.IsVersion("follow-through", "v0.99.1"))
        {
            // v0.99.1: +2 damage, +1 weak (current: +2 damage only)
            __instance.DynamicVars.Damage.UpgradeValueBy(2m);
            __instance.DynamicVars.Weak.UpgradeValueBy(1m);
            return false;
        }
        return true;
    }
}
