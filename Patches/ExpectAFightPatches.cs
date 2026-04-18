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

// ExpectAFight v0.99.1 vs current
// v0.99.1:  OnPlay: gain energy based on attacks in hand only
// Current: OnPlay: gain energy based on attacks in hand + apply NoEnergyGainPower(1)

[HarmonyPatch(typeof(ExpectAFight), "OnPlay")]
static class ExpectAFight_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, ExpectAFight __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("expect-a-fight", "v0.99.1"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    // v0.99.1: gain energy only (current: gain energy + apply NoEnergyGainPower )
    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, ExpectAFight instance)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await PlayerCmd.GainEnergy(((CalculatedVar)instance.DynamicVars["CalculatedEnergy"]).Calculate(cardPlay.Target), instance.Owner);
    }
}

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class ExpectAFight_Description_Patch
{
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is ExpectAFight && RevertAnthony.IsVersion("expect-a-fight", "v0.99.1"))
            __result = new LocString("cards", "EXPECT_A_FIGHT_V0991.description");
    }
}
