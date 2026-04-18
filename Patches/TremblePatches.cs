using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace RevertAnthony;

// Tremble v0.99.1 vs current
// v0.99.1: Vulnerable 2, no Exhaust keyword
// Current:  Vulnerable 3, Exhaust keyword

[HarmonyPatch(typeof(Tremble), "get_CanonicalKeywords")]
static class Tremble_CanonicalKeywords_Patch
{
    static bool Prefix(ref IEnumerable<CardKeyword> __result)
    {
        if (RevertAnthony.IsVersion("tremble", "v0.99.1"))
        {
            // v0.99.1: no Exhaust (current: has Exhaust)
            __result = Array.Empty<CardKeyword>();
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Tremble), "get_CanonicalVars")]
static class Tremble_CanonicalVars_Patch
{
    static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        if (RevertAnthony.IsVersion("tremble", "v0.99.1"))
        {
            // v0.99.1: Vuln 2 (current: Vuln 3)
            __result = new DynamicVar[]
            {
                new PowerVar<VulnerablePower>(2m),
            };
            return false;
        }
        return true;
    }
}
