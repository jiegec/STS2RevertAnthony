using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// Stoke v0.99.1 vs current — complete rework
// v0.99.1: Exhaust keyword. OnPlay: exhaust all cards in hand, draw that many.
//          OnUpgrade → -1 cost
// Current:  No keywords. OnPlay: exhaust all cards in hand, generate new random cards (upgrade if upgraded).
//          No OnUpgrade

[HarmonyPatch(typeof(Stoke), "get_CanonicalKeywords")]
static class Stoke_CanonicalKeywords_Patch
{
    static bool Prefix(ref IEnumerable<CardKeyword> __result)
    {
        if (RevertAnthony.IsVersion("stoke", "v0.99.1"))
        {
            // v0.99.1: has Exhaust (current: no keywords)
            __result = new CardKeyword[]
            {
                CardKeyword.Exhaust,
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Stoke), "OnPlay")]
static class Stoke_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, Stoke __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("stoke", "v0.99.1"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    // v0.99.1: exhaust hand, draw that many (current: exhaust hand, generate new random cards)
    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, Stoke instance)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        List<CardModel> list = PileType.Hand.GetPile(instance.Owner).Cards.ToList();
        int cardCount = list.Count;
        foreach (CardModel item in list)
        {
            await CardCmd.Exhaust(choiceContext, item);
        }
        await CardPileCmd.Draw(choiceContext, cardCount, instance.Owner);
    }
}

[HarmonyPatch(typeof(CardModel), "OnUpgrade")]
static class Stoke_OnUpgrade_Patch
{
    static void Postfix(CardModel __instance)
    {
        if (__instance is Stoke && RevertAnthony.IsVersion("stoke", "v0.99.1"))
        {
            // v0.99.1: -1 cost (current: no upgrade effect)
            __instance.EnergyCost.UpgradeBy(-1);
        }
    }
}

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class Stoke_Description_Patch
{
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is Stoke && RevertAnthony.IsVersion("stoke", "v0.99.1"))
            __result = new LocString("cards", "STOKE_V0991.description");
    }
}
