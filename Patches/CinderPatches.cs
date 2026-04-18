using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace RevertAnthony;

// Cinder v0.99.1 vs current
// v0.99.1: Damage 17(22), exhaust 1 card from top of draw pile (shuffle first)
//          OnUpgrade → +5 damage
// Current:  Damage 18(24), exhaust 1 random card from hand
//          OnUpgrade → +6 damage

[HarmonyPatch(typeof(Cinder), "get_CanonicalVars")]
static class Cinder_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("cinder", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(17m, ValueProp.Move),
                new DynamicVar("CardsToExhaust", 1m),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Cinder), "OnPlay")]
static class Cinder_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, Cinder __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("cinder", "v0.99.1"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    // v0.99.1: Exhaust from top of draw pile (shuffle if needed)
    // Current: Exhaust random card from hand
    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, Cinder instance)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await DamageCmd.Attack(instance.DynamicVars.Damage.BaseValue).FromCard(instance).Targeting(cardPlay.Target)
            .WithHitVfxNode((Creature t) => NFireBurstVfx.Create(t, 0.75f))
            .Execute(choiceContext);
        CardPile drawPile = PileType.Draw.GetPile(instance.Owner);
        for (int i = 0; i < instance.DynamicVars["CardsToExhaust"].IntValue; i++)
        {
            await CardPileCmd.ShuffleIfNecessary(choiceContext, instance.Owner);
            CardModel cardModel = drawPile.Cards.FirstOrDefault();
            if (cardModel != null)
            {
                await CardCmd.Exhaust(choiceContext, cardModel);
            }
        }
    }
}

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class Cinder_Description_Patch
{
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is Cinder && RevertAnthony.IsVersion("cinder", "v0.99.1"))
            __result = new LocString("cards", "CINDER_V0991.description");
    }
}

[HarmonyPatch(typeof(Cinder), "OnUpgrade")]
static class Cinder_OnUpgrade_Patch
{
    static bool Prefix(Cinder __instance)
    {
        if (RevertAnthony.IsVersion("cinder", "v0.99.1"))
        {
            // v0.99.1: +5 damage (current: +6)
            __instance.DynamicVars.Damage.UpgradeValueBy(5m);
            return false;
        }
        return true;
    }
}
