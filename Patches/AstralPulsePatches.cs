using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// AstralPulse v0.99.1/v0.103.2 vs current
// v0.99.1/v0.103.2: Damage 14, no hit count multiplier, upgrade +4
// v0.107.1: Damage 6, hit twice, upgrade +2

[HarmonyPatch(typeof(AstralPulse), "CanonicalVars", MethodType.Getter)]
static class AstralPulse_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("astral-pulse", "v0.99.1", "v0.103.2"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(14m, ValueProp.Move),
            };
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(AstralPulse), "OnPlay")]
static class AstralPulse_OnPlay_Patch
{
    static bool Prefix(PlayerChoiceContext choiceContext, CardPlay cardPlay, AstralPulse __instance, ref Task __result)
    {
        if (!RevertAnthony.IsVersion("astral-pulse", "v0.99.1", "v0.103.2"))
            return true;

        __result = OldOnPlay(choiceContext, cardPlay, __instance);
        return false;
    }

    static async Task OldOnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay, AstralPulse instance)
    {
        await DamageCmd.Attack(instance.DynamicVars.Damage.BaseValue).FromCard(instance).TargetingAllOpponents(instance.CombatState)
            .WithHitFx("vfx/vfx_starry_impact")
            .SpawningHitVfxOnEachCreature()
            .Execute(choiceContext);
    }
}

[HarmonyPatch(typeof(AstralPulse), "OnUpgrade")]
static class AstralPulse_OnUpgrade_Patch
{
    static bool Prefix(AstralPulse __instance)
    {
        if (RevertAnthony.IsVersion("astral-pulse", "v0.99.1", "v0.103.2"))
        {
            // v0.99.1/v0.103.2: +4 damage (current: +2)
            __instance.DynamicVars.Damage.UpgradeValueBy(4m);
            return false;
        }
        return true;
    }
}
