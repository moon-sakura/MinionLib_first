using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MinionLib.Component.Interfaces;

namespace MinionLib.Component.Patches;

public static class CardComponentDyanamicVarsUpdatePatch
{
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.UpdateDynamicVarPreview))]
    [HarmonyPostfix]
    private static void UpdateDynamicVarPreviewPostfix(CardModel __instance, object previewMode,
        Creature? target, object dynamicVarSet)
    {
        if (__instance is not IComponentsCardModel componentsCard)
            return;

        var runGlobalHooks = __instance.CombatState != null;

        foreach (var component in componentsCard.Components)
        foreach (var dynVar in component.DynamicVars.Values)
            dynVar.UpdateCardPreview(__instance, (dynamic)previewMode, target, runGlobalHooks);
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.FinalizeUpgradeInternal))]
    [HarmonyPostfix]
    private static void FinalizeUpgradeInternalPostfix(CardModel __instance)
    {
        if (__instance is not IComponentsCardModel componentsCard)
            return;

        foreach (var varSet in componentsCard.Components.Select(c => c.DynamicVars)) varSet.FinalizeUpgrade();
    }
}
