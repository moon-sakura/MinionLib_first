using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace MinionLib.RightClick.Easy;

public class EasyRightClickCardAction : GameAction
{
    public Player Player { get; }
    public RightClickContext.Payload Extra { get; }
    public bool WasEnqueuedInCombat { get; }
    public override ulong OwnerId => Player.NetId;

    public override GameActionType ActionType =>
        WasEnqueuedInCombat ? GameActionType.CombatPlayPhaseOnly : GameActionType.NonCombat;

    public EasyRightClickableModelType Type { get; init; }

    public ModelId ModelId { get; }

    // Card
    public NetCombatCard NetCombatCard { get; init; }

    // Power
    public uint CreatureCombatId { get; init; }

    // Potion
    public uint PotionIndex { get; init; }


    public EasyRightClickCardAction(RightClickContext context, bool isCombatInProgress)
    {
        WasEnqueuedInCombat = isCombatInProgress;
        Player = context.Player;
        Extra = context.Extra;

        var model = context.Model;
        ModelId = model.Id;
        switch (model)
        {
            case CardModel card:
                Type = EasyRightClickableModelType.Card;
                NetCombatCard = NetCombatCard.FromModel(card);
                break;
            case RelicModel:
                Type = EasyRightClickableModelType.Relic;
                break;
            case PowerModel power:
                Type = EasyRightClickableModelType.Power;
                var combatId = power.Owner.CombatId;
                if (combatId == null) return;
                CreatureCombatId = combatId.Value;
                break;
            case PotionModel potion:
                Type = EasyRightClickableModelType.Potion;
                var index = potion.Owner.GetPotionSlotIndex(potion);
                if (index < 0)
                    throw new InvalidOperationException(
                        $"Potion {potion} has owner {Player}, but the owner's potion list does not contain it!");
                PotionIndex = (uint)index;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(context), model, null);
        }
    }

    public EasyRightClickCardAction(Player player, ModelId modelId, RightClickContext.Payload extra,
        bool isCombatInProgress)
    {
        Player = player;
        ModelId = modelId;
        Extra = extra;
        WasEnqueuedInCombat = isCombatInProgress;
    }


    protected override async Task ExecuteAction()
    {
        var combatState = Player.Creature.CombatState;
        if (WasEnqueuedInCombat && combatState is null) return;

        AbstractModel? model;
        switch (Type)
        {
            case EasyRightClickableModelType.Card:
                var card = NetCombatCard.ToCardModel();
                if (card.Pile?.Type != PileType.Hand) return;
                model = card;
                break;
            case EasyRightClickableModelType.Relic:
                model = Player.Relics.FirstOrDefault(r => r.Id == ModelId);
                break;
            case EasyRightClickableModelType.Power:
                model = combatState!.GetCreature(CreatureCombatId)?.Powers.FirstOrDefault(p => p.Id == ModelId);
                break;
            case EasyRightClickableModelType.Potion:
                model = Player.GetPotionAtSlotIndex((int)PotionIndex);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (model == null || model.Id != ModelId) return;
        if (model is not IEasyRightClickableModel rightClickable) return;

        var choiceContext = new GameActionPlayerChoiceContext(this);
        var clickContent = new RightClickContext(Player, model, Extra);
        await rightClickable.OnRightClick(choiceContext, clickContent);
        model.InvokeExecutionFinished();
    }

    public override INetAction ToNetAction()
    {
        return new NetEasyRightClickCardAction
        {
            Type = Type,
            ModelId = ModelId,
            NetCombatCard = NetCombatCard,
            CreatureCombatId = CreatureCombatId,
            PotionIndex = PotionIndex,
            Extra = Extra,
            WasEnqueuedInCombat = WasEnqueuedInCombat
        };
    }
}
