using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Relics;

namespace MinionLib.RightClick.Patches;

[HarmonyPatch(typeof(NRelic), nameof(NRelic._Ready))]
public static class RelicRightClickPatch
{
    private const string Module = "RelicRightClickPatch";

    [HarmonyPostfix]
    private static void Postfix(NRelic __instance)
    {
        __instance.Connect(Control.SignalName.GuiInput,
            Callable.From<InputEvent>(inputEvent => OnRelicGuiInput(__instance, inputEvent)));
    }

    private static void OnRelicGuiInput(NRelic relicNode, InputEvent inputEvent)
    {
        if (relicNode.GetViewport().IsInputHandled())
            return;

        var triggeredByMouse =
            inputEvent is InputEventMouseButton { ButtonIndex: MouseButton.Right } mouseButton &&
            mouseButton.IsReleased();

        var triggeredByController =
            inputEvent is InputEventAction { Action: var action } actionEvent &&
            action == MegaInput.cancel &&
            actionEvent.IsPressed() &&
            relicNode.HasFocus();

        if (!triggeredByMouse && !triggeredByController) return;
        if (NTargetManager.Instance.IsInSelection) return;

        var relic = relicNode.Model;

        var me = LocalContext.GetMe(relic.Owner.RunState);
        if (me == null) return;

        var context = new RightClickContext(me, relic, new RightClickContext.Payload(triggeredByController));

        if (RightClickDispatcher.TryDispatch(context)) relicNode.GetViewport().SetInputAsHandled();
    }
}
