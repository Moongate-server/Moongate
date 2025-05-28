using Moongate.Core.Spans;
using Moongate.Uo.Data.Context;
using Moongate.Uo.Data.Extensions;
using Moongate.Uo.Data.Types;
using Moongate.Uo.Network.Data.Sessions;
using Moongate.Uo.Network.Interfaces.Messages;

namespace Moongate.Uo.Data.Network.Packets.Features;

public class SupportedFeaturesPacket : IUoNetworkPacket
{
    public byte OpCode { get; }
    public int Length => ExtendedSupportedFeatures ? 5 : 3;

    public bool ExtendedSupportedFeatures { get; set; }


    public SupportedFeaturesPacket(SessionData? sessionData = null)
    {

        if (sessionData != null)
        {
            ExtendedSupportedFeatures = sessionData.ExtendedSupportedFeatures();
        }
    }

    public bool Read(SpanReader reader)
    {
        return false;
    }

    public ReadOnlyMemory<byte> Write(SpanWriter writer)
    {
        var flags = UoContext.ExpansionInfo.SupportedFeatures;

        flags |= FeatureFlags.LiveAccount;
        flags |= FeatureFlags.SeventhCharacterSlot;

        writer.Write(OpCode);
        if (ExtendedSupportedFeatures)
        {
            writer.Write((uint)flags);
        }
        else
        {
            writer.Write((ushort)flags);
        }

        return writer.ToArray();

    }
}
