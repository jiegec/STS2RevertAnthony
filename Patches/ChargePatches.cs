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

// Charge v0.99.1 vs current
// v0.99.1: Transform selected cards into MinionStrike (upgrade minion if Charge upgraded)
//          ExtraHoverTips → MinionStrike
// Current:  Transform selected cards into MinionDiveBomb (upgrade minion if Charge upgraded)
//          ExtraHoverTips → MinionDiveBomb

[HarmonyPatch(typeof(Charge), "get_ExtraHoverTips")]
static class Charge_ExtraHoverTips_Patch
{
    static bool Prefix(Charge __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (RevertAnthony.IsVersion("charge", "v0.99.1"))
        {
            // v0.99.1: MinionStrike (current: MinionDiveBomb)
            __result = new IHoverTip[]
            {
                HoverTipFactory.FromCard<MinionStrike>(__instance.IsUpgraded),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Charge), "OnPlay")]
static class Charge_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, Charge __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("charge", "v0.99.1"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    // v0.99.1: Transform to MinionStrike (current: MinionDiveBomb)
    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, Charge instance)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        List<CardModel> cardsIn = (from c in PileType.Draw.GetPile(instance.Owner).Cards
                                   orderby c.Rarity, c.Id
                                   select c).ToList();
        List<CardModel> list = (await CardSelectCmd.FromSimpleGrid(choiceContext, cardsIn, instance.Owner, new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, instance.DynamicVars.Cards.IntValue))).ToList();
        foreach (CardModel item in list)
        {
            CardPileAddResult? cardPileAddResult = await CardCmd.TransformTo<MinionStrike>(item);
            if (instance.IsUpgraded && cardPileAddResult.HasValue)
            {
                CardCmd.Upgrade(cardPileAddResult.Value.cardAdded);
            }
        }
    }
}

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class Charge_Description_Patch
{
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is Charge && RevertAnthony.IsVersion("charge", "v0.99.1"))
            __result = new LocString("cards", "CHARGE_V0991.description");
    }
}
