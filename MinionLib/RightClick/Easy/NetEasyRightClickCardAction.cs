using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;

namespace MinionLib.RightClick.Easy;

public struct NetEasyRightClickCardAction : INetAction
{
    public EasyRightClickableModelType Type;
    public ModelId ModelId;

    // Card
    public NetCombatCard NetCombatCard;

    // Power
    public uint CreatureCombatId;

    // Potion
    public uint PotionIndex;

    public RightClickContext.Payload Extra;
    public bool WasEnqueuedInCombat;

    public void Serialize(PacketWriter writer)
    {
        writer.WriteEnum(Type);
        writer.WriteFullModelId(ModelId);
        switch (Type)
        {
            case EasyRightClickableModelType.Card:
                writer.Write(NetCombatCard);
                break;
            case EasyRightClickableModelType.Power:
                writer.WriteUInt(CreatureCombatId);
                break;
            case EasyRightClickableModelType.Potion:
                writer.WriteUInt(PotionIndex);
                break;
        }

        writer.Write(Extra);
        writer.WriteBool(WasEnqueuedInCombat);
    }

    public void Deserialize(PacketReader reader)
    {
        Type = reader.ReadEnum<EasyRightClickableModelType>();
        ModelId = reader.ReadFullModelId();
        switch (Type)
        {
            case EasyRightClickableModelType.Card:
                NetCombatCard = reader.Read<NetCombatCard>();
                break;
            case EasyRightClickableModelType.Power:
                CreatureCombatId = reader.ReadUInt();
                break;
            case EasyRightClickableModelType.Potion:
                PotionIndex = reader.ReadUInt();
                break;
        }
        Extra = reader.Read<RightClickContext.Payload>();
        WasEnqueuedInCombat = reader.ReadBool();
    }

    public GameAction ToGameAction(Player player)
    {
        return new EasyRightClickCardAction(player, ModelId, Extra, WasEnqueuedInCombat)
        {
            Type = Type,
            NetCombatCard = NetCombatCard,
            CreatureCombatId = CreatureCombatId,
            PotionIndex = PotionIndex
        };
    }
}
