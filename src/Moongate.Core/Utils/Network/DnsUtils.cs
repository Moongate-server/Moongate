using System.Net;
using System.Net.Sockets;

namespace Moongate.Core.Utils.Network;

public static class DnsUtils
{
    /// <summary>
    /// Attempts to resolve a hostname from an IP address
    /// </summary>
    /// <param name="ipAddress">The IP address to resolve</param>
    /// <returns>True if hostname was successfully resolved, false otherwise</returns>
    public static async Task<(bool Resolved, string HostName)> TryResolveHostnameAsync(string ipAddress)
    {
        try
        {
            // Parse the IP address
            if (!IPAddress.TryParse(ipAddress, out IPAddress ip))
            {
                return (false, string.Empty);
            }

            // Try to get hostname from IP
            var hostEntry = await Dns.GetHostEntryAsync(ip);

            // Check if we got a valid hostname
            var resolved = !string.IsNullOrEmpty(hostEntry.HostName) &&
                           !hostEntry.HostName.Equals(ipAddress);

            return (resolved, hostEntry.HostName);
        }
        catch (SocketException)
        {
            return (false, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, string.Empty);
        }
    }
}
