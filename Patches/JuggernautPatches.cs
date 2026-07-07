using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RevertAnthony;

// Juggernaut v0.99.1/v0.103.2 vs current
// v0.99.1/v0.103.2: JuggernautPower 5
// v0.107.1: JuggernautPower 6

[HarmonyPatch(typeof(Juggernaut), "CanonicalVars", MethodType.Getter)]
static class Juggernaut_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("juggernaut", "v0.99.1", "v0.103.2"))
        {
            __result = new DynamicVar[]
            {
                new PowerVar<JuggernautPower>(5m),
            };
            return false;
        }
        return true;
    }
}
