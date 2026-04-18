using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// Begone v0.99.1 vs current
// v0.99.1: 1-cost Attack, AnyEnemy, Damage 4(+1), ExtraHoverTips → MinionDiveBomb, deal dmg then transform to MinionDiveBomb
// Current:  1-cost Skill, Self, no vars, ExtraHoverTips → MinionStrike, transform to MinionStrike

[HarmonyPatch(typeof(Begone), "get_ExtraHoverTips")]
static class Begone_ExtraHoverTips_Patch
{
    static bool Prefix(Begone __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (RevertAnthony.IsVersion("begone", "v0.99.1"))
        {
            // v0.99.1: MinionDiveBomb hover tip (current: MinionStrike)
            __result = new IHoverTip[]
            {
                HoverTipFactory.FromCard<MinionDiveBomb>(__instance.IsUpgraded),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Begone), "OnPlay")]
static class Begone_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, Begone __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("begone", "v0.99.1"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    // v0.99.1: Deal damage, then select 1 card from hand and transform to MinionDiveBomb
    // Current: No damage, transform to MinionStrike
    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, Begone instance)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(instance.DynamicVars.Damage.BaseValue)
            .FromCard(instance)
            .Targeting(cardPlay.Target!)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        CardModel cardModel = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1),
            context: choiceContext,
            player: instance.Owner,
            filter: null,
            source: instance
        )).FirstOrDefault();

        if (cardModel != null)
        {
            CardModel cardModel2 = instance.CombatState.CreateCard<MinionDiveBomb>(instance.Owner);
            if (instance.IsUpgraded)
            {
                CardCmd.Upgrade(cardModel2);
            }
            await CardCmd.Transform(cardModel, cardModel2);
        }
    }
}

[HarmonyPatch(typeof(CardModel), "TargetType", MethodType.Getter)]
static class Begone_TargetType_Patch
{
    static void Postfix(CardModel __instance, ref TargetType __result)
    {
        if (__instance is Begone && RevertAnthony.IsVersion("begone", "v0.99.1"))
            __result = TargetType.AnyEnemy;
    }
}

[HarmonyPatch(typeof(CardModel), "Type", MethodType.Getter)]
static class Begone_CardType_Patch
{
    static void Postfix(CardModel __instance, ref CardType __result)
    {
        if (__instance is Begone && RevertAnthony.IsVersion("begone", "v0.99.1"))
            __result = CardType.Attack;
    }
}

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class Begone_Description_Patch
{
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is Begone && RevertAnthony.IsVersion("begone", "v0.99.1"))
            __result = new LocString("cards", "BEGONE_V0991.description");
    }
}

[HarmonyPatch(typeof(CardModel), "CanonicalVars", MethodType.Getter)]
static class Begone_CanonicalVars_Patch
{
    static void Postfix(CardModel __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (__instance is Begone && RevertAnthony.IsVersion("begone", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(4m, ValueProp.Move)
            };
        }
    }
}

[HarmonyPatch(typeof(CardModel), "OnUpgrade")]
static class Begone_OnUpgrade_Patch
{
    static void Postfix(CardModel __instance)
    {
        if (__instance is Begone && RevertAnthony.IsVersion("begone", "v0.99.1"))
        {
            // v0.99.1: +1 damage (current: no upgrade effect)
            __instance.DynamicVars.Damage.UpgradeValueBy(1m);
        }
    }
}
