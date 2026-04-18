using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// Debilitate v0.99.1 vs current
// v0.99.1:  Damage 7, DebilitatePower 3
// Current: Damage 10, DebilitatePower 3

[HarmonyPatch(typeof(Debilitate), "get_CanonicalVars")]
static class Debilitate_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("debilitate", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(7m, ValueProp.Move),
                new PowerVar<DebilitatePower>(3m),
            };
            return false;
        }
        return true;
    }
}
