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
}
