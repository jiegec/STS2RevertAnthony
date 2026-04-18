using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// FightMe v0.99.1 vs current
// v0.99.1: Damage 5, Repeat 2, Strength 2, EnemyStrength 1
// Current:  Damage 5, Repeat 2, Strength 3, EnemyStrength 1

[HarmonyPatch(typeof(FightMe), "get_CanonicalVars")]
static class FightMe_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("fight-me", "v0.99.1"))
        {
            // v0.99.1: 2 strength to self (current: 3)
            __result = new DynamicVar[]
            {
                new DamageVar(5m, ValueProp.Move),
                new RepeatVar(2),
                new PowerVar<StrengthPower>(2m),
                new DynamicVar("EnemyStrength", 1m),
            };
            return false;
        }
        return true;
    }
}
