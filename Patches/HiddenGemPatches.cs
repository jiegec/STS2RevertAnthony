using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// HiddenGem v0.99.1 vs current
// v0.99.1: Can be generated in combat, OnPlay doesn't filter out cards with enchanted replay
// Current:  Cannot be generated in combat, OnPlay excludes cards with enchanted replay count >= 1

[HarmonyPatch(typeof(HiddenGem), "get_CanBeGeneratedInCombat")]
static class HiddenGem_CanBeGeneratedInCombat_Patch
{
    static bool Prefix(ref bool __result)
    {
        if (RevertAnthony.IsVersion("hidden-gem", "v0.99.1"))
        {
            // v0.99.1: Not overridden, uses base default (true)
            __result = true;
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(HiddenGem), "OnPlay")]
static class HiddenGem_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, HiddenGem __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("hidden-gem", "v0.99.1"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    // v0.99.1: Does not exclude cards with enchanted replay count >= 1
    // Current:  Excludes cards with enchanted replay count >= 1
    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, HiddenGem instance)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        List<CardModel> list = PileType.Draw.GetPile(instance.Owner).Cards.ToList();
        if (list.Count == 0)
        {
            return;
        }
        List<CardModel> list2 = list.Where(delegate(CardModel c)
        {
            bool flag = !c.Keywords.Contains(CardKeyword.Unplayable);
            bool flag2 = flag;
            if (flag2)
            {
                CardType type = c.Type;
                bool flag3 = (uint)(type - 5) <= 1u;
                flag2 = !flag3;
            }
            return flag2;
        }).ToList();
        List<CardModel> list3 = list2.Where(delegate(CardModel c)
        {
            CardType type = c.Type;
            return (uint)(type - 1) <= 2u;
        }).ToList();
        IEnumerable<CardModel> items = ((list3.Count == 0) ? list2 : list3);
        CardModel cardModel = instance.Owner.RunState.Rng.CombatCardSelection.NextItem(items);
        if (cardModel != null)
        {
            cardModel.BaseReplayCount += instance.DynamicVars["Replay"].IntValue;
            CardCmd.Preview(cardModel);
        }
    }
}
