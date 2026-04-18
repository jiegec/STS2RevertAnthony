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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// Seance v0.99.1 vs current
// v0.99.1: Cost 0, ExtraHoverTips → Soul(IsUpgraded), OnPlay: transform + upgrade if upgraded, OnUpgrade: no-op
// Current:  Cost 1, ExtraHoverTips → Soul(), OnPlay: transform without upgrade, OnUpgrade: -1 energy cost

[HarmonyPatch(typeof(Seance), "get_ExtraHoverTips")]
static class Seance_ExtraHoverTips_Patch
{
    static bool Prefix(Seance __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (RevertAnthony.IsVersion("seance", "v0.99.1"))
        {
            // v0.99.1: Soul tooltip with IsUpgraded flag
            __result = new IHoverTip[]
            {
                HoverTipFactory.FromCard<Soul>(__instance.IsUpgraded),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Seance), "OnPlay")]
static class Seance_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, Seance __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("seance", "v0.99.1"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, Seance instance)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        List<CardModel> cardsIn = (from c in PileType.Draw.GetPile(instance.Owner).Cards
                                   orderby c.Rarity, c.Id
                                   select c).ToList();
        List<CardModel> list = (await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, instance.Owner, new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, instance.DynamicVars.Cards.IntValue))).ToList();
        foreach (CardModel item in list)
        {
            CardPileAddResult? cardPileAddResult = await CardCmd.TransformTo<Soul>(item);
            if (instance.IsUpgraded && cardPileAddResult.HasValue)
            {
                CardCmd.Upgrade(cardPileAddResult.Value.cardAdded);
            }
        }
    }
}

[HarmonyPatch(typeof(Seance), "OnUpgrade")]
static class Seance_OnUpgrade_Patch
{
    static bool Prefix()
    {
        if (RevertAnthony.IsVersion("seance", "v0.99.1"))
        {
            // v0.99.1: no upgrade effect (current: -1 energy cost)
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
static class Seance_EnergyCost_Patch
{
    static void Postfix(CardModel __instance, ref int __result)
    {
        if (__instance is Seance && RevertAnthony.IsVersion("seance", "v0.99.1"))
            __result = 0;
    }
}

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class Seance_Description_Patch
{
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is Seance && RevertAnthony.IsVersion("seance", "v0.99.1"))
            __result = new LocString("cards", "SEANCE_V0991.description");
    }
}
