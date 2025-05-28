using Moongate.Uo.Data.Entities;
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
}
