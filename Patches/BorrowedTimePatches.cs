using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RevertAnthony;

// BorrowedTime v0.99.1 vs current — complete rework
// v0.99.1: Cost 0, Gain 1(2) Energy, Apply 3 Doom
//          ExtraHoverTips → DoomPower + Energy, OnUpgrade → +1 Energy
// Current:  Cost 1, Gain 4(6) Energy, Apply BorrowedTimePower (+1 cost)
//          ExtraHoverTips → Energy only, OnUpgrade → +2 Energy
// (Energy cost patch is in BaseCardModelPatches.cs)
// (Description patch is in BaseCardModelPatches.cs)

[HarmonyPatch(typeof(BorrowedTime), "get_CanonicalVars")]
static class BorrowedTime_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("borrowed-time", "v0.99.1"))
        {
            // v0.99.1: Doom 3 + Energy 1 (current: Energy 4 + ExtraCost 1)
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
            // v0.99.1: DoomPower + Energy tips (current: Energy tip only)
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

    // v0.99.1: apply Doom, then gain Energy
    // Current:  gain Energy, then apply BorrowedTimePower
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
            // v0.99.1: +1 Energy (current: +2 Energy)
            __instance.DynamicVars.Energy.UpgradeValueBy(1m);
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(CardModel), "CanonicalEnergyCost", MethodType.Getter)]
static class BorrowedTime_EnergyCost_Patch
{
    static void Postfix(CardModel __instance, ref int __result)
    {
        if (__instance is BorrowedTime && RevertAnthony.IsVersion("borrowed-time", "v0.99.1"))
            __result = 0;
    }
}

[HarmonyPatch(typeof(CardModel), "Description", MethodType.Getter)]
static class BorrowedTime_Description_Patch
{
    static void Postfix(CardModel __instance, ref LocString __result)
    {
        if (__instance is BorrowedTime && RevertAnthony.IsVersion("borrowed-time", "v0.99.1"))
            __result = new LocString("cards", "BORROWED_TIME_V0991.description");
    }
}
