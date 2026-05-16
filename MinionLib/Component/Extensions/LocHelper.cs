using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MinionLib.Component.Extensions;

public static class LocHelper
{
    public static void AddMany(this LocString loc, IEnumerable<DynamicVar> value)
    {
        foreach (var dynamicVar in value)
            loc.Add(dynamicVar);
    }

    public static void AddMany(this LocString loc, DynamicVarSet value)
    {
        value.AddTo(loc);
    }

    public static void AddMany(this LocString loc, IReadOnlyDictionary<string, object> value)
    {
        foreach (var (name, variable) in value)
            loc.AddObj(name, variable);
    }
}
