using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RevertAnthony;

// Glow v0.99.1 vs current
// v0.99.1: Stars 1, Cards 2. OnPlay: gain stars + draw 2
// Current:  Stars 1, Cards 1. OnPlay: gain stars + draw 1 + apply DrawCardsNextTurnPower(1)

[HarmonyPatch(typeof(Glow), "get_CanonicalVars")]
static class Glow_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("glow", "v0.99.1"))
        {
            // v0.99.1: draw 2 cards (current: draw 1)
            __result = new DynamicVar[]
            {
                new StarsVar(1),
                new CardsVar(2),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Glow), "OnPlay")]
static class Glow_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, Glow __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("glow", "v0.99.1"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    // v0.99.1:  gain stars + draw cards only
    // Current: gain stars + draw cards + apply DrawCardsNextTurnPower
    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, Glow instance)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await PlayerCmd.GainStars(instance.DynamicVars.Stars.BaseValue, instance.Owner);
        await CardPileCmd.Draw(choiceContext, instance.DynamicVars.Cards.BaseValue, instance.Owner);
    }
}

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class Glow_Description_Patch
{
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is Glow && RevertAnthony.IsVersion("glow", "v0.99.1"))
            __result = new LocString("cards", "GLOW_V0991.description");
    }
}
