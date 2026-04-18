using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace RevertAnthony;

// CorrosiveWave v0.99.1 vs current
// v0.99.1: CorrosiveWave power 3 (current: CorrosiveWave power 2)

[HarmonyPatch(typeof(CorrosiveWave), "get_CanonicalVars")]
static class CorrosiveWave_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("corrosive-wave", "v0.99.1"))
        {
            __result = new DynamicVar[]
            {
                new DynamicVar("CorrosiveWave", 3m),
            };
            return false;
        }
        return true;
    }
}
