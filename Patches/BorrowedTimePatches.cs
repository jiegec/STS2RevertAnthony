using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RevertAnthony;

// ============================================
// Borrowed Time - v0.99.1 vs v0.103.2
// Old: Cost 0, Gain 1(2) Energy, Apply 3 Doom
// New: Cost 1, Gain 4(6) Energy, Apply BorrowedTimePower (+1 cost)
// ============================================

[HarmonyPatch(typeof(BorrowedTime), "get_CanonicalVars")]
static class BorrowedTime_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("borrowed-time", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new PowerVar<DoomPower>(3m),
                new EnergyVar(1)
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(BorrowedTime), "get_ExtraHoverTips")]
static class BorrowedTime_ExtraHoverTips_Patch
{
    static bool Prefix(BorrowedTime __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (RevertAnthony.IsVersion("borrowed-time", "v0.99.1"))
        {
            __result = new IHoverTip[]
            {
                HoverTipFactory.FromPower<DoomPower>(),
                HoverTipFactory.ForEnergy(__instance)
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(BorrowedTime), "OnPlay")]
static class BorrowedTime_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, BorrowedTime __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("borrowed-time", "v0.99.1"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, BorrowedTime instance)
    {
        await CreatureCmd.TriggerAnim(instance.Owner.Creature, "Cast", instance.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<DoomPower>(instance.Owner.Creature, instance.DynamicVars.Doom.BaseValue, instance.Owner.Creature, instance);
        await PlayerCmd.GainEnergy(instance.DynamicVars.Energy.BaseValue, instance.Owner);
    }
}

[HarmonyPatch(typeof(BorrowedTime), "OnUpgrade")]
static class BorrowedTime_OnUpgrade_Patch
{
    static bool Prefix(BorrowedTime __instance)
    {
        if (RevertAnthony.IsVersion("borrowed-time", "v0.99.1"))
        {
            __instance.DynamicVars.Energy.UpgradeValueBy(1m);
            return false;
        }
        return true;
    }
}
