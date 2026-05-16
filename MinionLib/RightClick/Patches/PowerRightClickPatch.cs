using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace MinionLib.RightClick.Patches;

[HarmonyPatch(typeof(NPower), nameof(NPower._Ready))]
public static class PowerRightClickPatch
{
    private const string Module = "PowerRightClickPatch";

    [HarmonyPostfix]
    private static void Postfix(NPower __instance)
    {
        __instance.Connect(Control.SignalName.GuiInput,
            Callable.From<InputEvent>(inputEvent => OnPowerGuiInput(__instance, inputEvent)));
    }

    private static void OnPowerGuiInput(NPower powerNode, InputEvent inputEvent)
    {
        if (powerNode.GetViewport().IsInputHandled())
            return;

        var triggeredByMouse =
            inputEvent is InputEventMouseButton { ButtonIndex: MouseButton.Right } mouseButton &&
            mouseButton.IsReleased();

        var triggeredByController =
            inputEvent is InputEventAction { Action: var action } actionEvent &&
            action == MegaInput.cancel &&
            actionEvent.IsPressed() &&
            powerNode.HasFocus();

        if (!triggeredByMouse && !triggeredByController) return;
        if (NTargetManager.Instance.IsInSelection) return;

        var power = powerNode.Model;

        var me = LocalContext.GetMe(power.Owner.CombatState);
        if (me == null) return;

        var context = new RightClickContext(me, power, new RightClickContext.Payload(triggeredByController));

        if (RightClickDispatcher.TryDispatch(context)) powerNode.GetViewport().SetInputAsHandled();
    }
}
