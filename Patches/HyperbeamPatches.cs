using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// Hyperbeam v0.99.1/v0.103.2 vs current
// v0.99.1/v0.103.2: Damage 26
// v0.107.1: Damage 28

[HarmonyPatch(typeof(Hyperbeam), "CanonicalVars", MethodType.Getter)]
static class Hyperbeam_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("hyperbeam", "v0.99.1", "v0.103.2"))
        {
            __result = new DynamicVar[]
            {
                new DamageVar(26m, ValueProp.Move),
                new PowerVar<FocusPower>(3m),
            };
            return false;
        }
        return true;
    }
}
