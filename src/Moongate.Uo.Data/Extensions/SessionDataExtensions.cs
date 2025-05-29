using System.Runtime.CompilerServices;
using Moongate.Uo.Data.Entities;
using Moongate.Uo.Data.Types;
using Moongate.Uo.Network.Data.Sessions;

namespace Moongate.Uo.Data.Extensions;

public static class SessionDataExtensions
{
    public static void SetClientVersion(this SessionData session, ClientVersion clientVersion)
    {
        session.SetData(clientVersion);
    }

    public static ClientVersion GetClientVersion(this SessionData session)
    {
        return session.GetData<ClientVersion>();
    }

    public static ProtocolChanges GetProtocolChanges(this SessionData session)
    {
        var clientVersion = session.GetClientVersion();

        if (clientVersion == null)
        {
            throw new InvalidOperationException("Client version is not set in the session data.");
        }

        return clientVersion.ProtocolChanges;
    }

    public static ClientFlags? GetClientFlags(this SessionData session)
    {
        return session.GetData<ClientFlags>();
    }

    public static void SetClientFlags(this SessionData session, ClientFlags clientFlags)
    {
        session.SetData(clientFlags);
    }

    public static int GetSeed(this SessionData session)
    {
        return session.GetData<int?>("seed") ?? 0;
    }

    public static MobileEntity? GetMobile(this SessionData session)
    {
        return session.GetData<MobileEntity>();
    }

    public static void SetMobile(this SessionData session, MobileEntity mobile)
    {
        session.SetData(mobile);
    }

    public static void SetSeed(this SessionData session, int seed)
    {
        session.SetData(seed, "seed");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasProtocolChanges(this SessionData session, ProtocolChanges changes) =>
        (session.GetProtocolChanges() & changes) != 0;

    public static bool NewSpellbook(this SessionData session) => session.HasProtocolChanges(ProtocolChanges.NewSpellbook);

    public static bool ExtendedSupportedFeatures(this SessionData session) => session.HasProtocolChanges(ProtocolChanges.ExtendedSupportedFeatures);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasFlag(this SessionData session, ClientFlags flag) => (session.GetClientFlags() & flag) != 0;
}
