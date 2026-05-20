using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MinionLib.Utilities.BetterExtraArgs;

namespace MinionLib.Utilities.DescriptionPostProcess;

[HarmonyPatch]
public static class DescriptionPostProcessPatch
{
    [HarmonyTargetMethod]
    private static MethodBase TargetMethod()
    {
        var previewEnumType = AccessTools.Inner(typeof(CardModel), "DescriptionPreviewType");

        return AccessTools.Method(typeof(CardModel), "GetDescriptionForPile", [
            typeof(PileType),
            previewEnumType,
            typeof(Creature)
        ]);
    }

    [HarmonyPostfix]
    private static void Postfix(
        CardModel __instance,
        ref string __result,
        PileType pileType,
        int previewType,
        Creature? target = null)
    {
        if (__instance is IDescriptionPostProcessCard card)
            __result = card.PostProcessDescription(__result, pileType, (DescriptionPreviewType)previewType, target);
    }
}
