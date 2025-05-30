using System.Runtime.CompilerServices;
using Moongate.Uo.Data.Types;

namespace Moongate.Uo.Data.Context;

public static class UoContext
{
    public static ClientVersion ServerVersion { get; set; }

    public static Expansion Expansion { get; set; }

    public static ExpansionInfo ExpansionInfo { get; set; }


    public static int SlotLimit { get; set; } = 5;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasProtocolChanges(ProtocolChanges changes) => (ServerVersion.ProtocolChanges & changes) != 0;

    public static bool NewSpellbook => HasProtocolChanges(ProtocolChanges.NewSpellbook);
    public static bool DamagePacket => HasProtocolChanges(ProtocolChanges.DamagePacket);
    public static bool BuffIcon => HasProtocolChanges(ProtocolChanges.BuffIcon);
    public static bool NewHaven => HasProtocolChanges(ProtocolChanges.NewHaven);
    public static bool ContainerGridLines => HasProtocolChanges(ProtocolChanges.ContainerGridLines);
    public static bool ExtendedSupportedFeatures => HasProtocolChanges(ProtocolChanges.ExtendedSupportedFeatures);
    public static bool StygianAbyss => HasProtocolChanges(ProtocolChanges.StygianAbyss);
    public static bool HighSeas => HasProtocolChanges(ProtocolChanges.HighSeas);
    public static bool NewCharacterList => HasProtocolChanges(ProtocolChanges.NewCharacterList);
    public static bool NewCharacterCreation => HasProtocolChanges(ProtocolChanges.NewCharacterCreation);
    public static bool ExtendedStatus => HasProtocolChanges(ProtocolChanges.ExtendedStatus);
    public static bool NewMobileIncoming => HasProtocolChanges(ProtocolChanges.NewMobileIncoming);
    public static bool NewSecureTrading => HasProtocolChanges(ProtocolChanges.NewSecureTrading);

    //public static bool IsUOTDClient => HasFlag(ClientFlags.UOTD) || ClientVersion?.Type == ClientType.UOTD;
    public static bool IsKRClient => ServerVersion?.Type == ClientType.KR;
    public static bool IsSAClient => ServerVersion?.Type == ClientType.SA;
    public static bool IsEnhancedClient => ServerVersion?.Type is ClientType.KR or ClientType.SA;
}
