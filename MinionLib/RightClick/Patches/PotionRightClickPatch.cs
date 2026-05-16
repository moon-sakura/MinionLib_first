using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Potions;

namespace MinionLib.RightClick.Patches;

[HarmonyPatch(typeof(NPotion), nameof(NPotion._Ready))]
public static class PotionRightClickPatch
{
    private const string Module = "PotionRightClickPatch";

    [HarmonyPostfix]
    private static void Postfix(NPotion __instance)
    {
        __instance.Connect(Control.SignalName.GuiInput,
            Callable.From<InputEvent>(inputEvent => OnPotionGuiInput(__instance, inputEvent)));
    }

    private static void OnPotionGuiInput(NPotion potionNode, InputEvent inputEvent)
    {
        if (potionNode.GetViewport().IsInputHandled())
            return;

        var triggeredByMouse =
            inputEvent is InputEventMouseButton { ButtonIndex: MouseButton.Right } mouseButton &&
            mouseButton.IsReleased();

        var triggeredByController =
            inputEvent is InputEventAction { Action: var action } actionEvent &&
            action == MegaInput.cancel &&
            actionEvent.IsPressed() &&
            potionNode.HasFocus();

        if (!triggeredByMouse && !triggeredByController) return;
        if (NTargetManager.Instance.IsInSelection) return;

        var potion = potionNode.Model;

        var me = LocalContext.GetMe(potion.Owner.RunState);
        if (me == null) return;

        var context = new RightClickContext(me, potion, new RightClickContext.Payload(triggeredByController));

        if (RightClickDispatcher.TryDispatch(context)) potionNode.GetViewport().SetInputAsHandled();
    }
}
