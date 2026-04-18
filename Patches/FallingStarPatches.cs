using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// FallingStar v0.99.1 vs current
// v0.99.1: Damage 7, Vulnerable 1, Weak 1
// Current:  Damage 8, Vulnerable 1, Weak 1

[HarmonyPatch(typeof(FallingStar), "get_CanonicalVars")]
static class FallingStar_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("falling-star", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(7m, ValueProp.Move),
                new PowerVar<VulnerablePower>(1m),
                new PowerVar<WeakPower>(1m),
            };
            return false;
        }
        return true;
    }
}
