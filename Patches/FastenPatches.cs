using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// Fasten v0.99.1/v0.103.2 vs current
// v0.99.1/v0.103.2: ExtraBlock 5
// v0.107.1: ExtraBlock 4

[HarmonyPatch(typeof(Fasten), "CanonicalVars", MethodType.Getter)]
static class Fasten_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("fasten", "v0.99.1", "v0.103.2"))
        {
            __result = new DynamicVar[]
            {
                new DynamicVar("ExtraBlock", 5m),
            };
            return false;
        }
        return true;
    }
}
