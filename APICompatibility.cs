using System.Reflection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;

namespace RevertAnthony;

static class Compat
{
    static PropertyInfo _currentSideProp;

    public static CombatSide GetCurrentSide(this CombatHistoryEntry entry)
    {
        if (_currentSideProp == null)
        {
            _currentSideProp = typeof(CombatHistoryEntry).GetProperty("CurrentSide",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
        return (CombatSide)_currentSideProp?.GetValue(entry);
    }
}
