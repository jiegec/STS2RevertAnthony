using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace RevertAnthony;

// Bulwark v0.99.1/v0.103.2 vs current
// v0.99.1/v0.103.2: Block 13, Forge 10
// v0.107.1: Block 12, Forge 10

[HarmonyPatch(typeof(Bulwark), "CanonicalVars", MethodType.Getter)]
static class Bulwark_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("bulwark", "v0.99.1", "v0.103.2"))
        {
            __result = new DynamicVar[]
            {
                new BlockVar(13m, ValueProp.Move),
                new ForgeVar(10),
            };
            return false;
        }
        return true;
    }
}
