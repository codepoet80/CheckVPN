using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace CheckVPN
{
    class CheckVPN
    {
        static int Main(string[] args)
        {
            string workingDir = Path.Combine(Directory.GetCurrentDirectory());

            Console.WriteLine("Checking the gateway...");
            IPAddress theGateway = GetDefaultGateway();
            if (theGateway.ToString() != "192.168.1.1")
            {
                Console.WriteLine("VPN is up!");

                Process proc = new Process();
                proc.StartInfo.WorkingDirectory = workingDir;
                proc.StartInfo.FileName = "start-download.bat";
                proc.StartInfo.CreateNoWindow = false;
                proc.Start();
                proc.WaitForExit();
                return 0;
            }
            else
            {
                Console.WriteLine("VPN is down!");

                Process proc = new Process();
                proc.StartInfo.WorkingDirectory = workingDir;
                proc.StartInfo.FileName = "stop-download.bat";
                proc.StartInfo.CreateNoWindow = false;
                proc.Start();
                proc.WaitForExit();
                return 1;
            }
        }

        public static IPAddress GetDefaultGateway()
        {
            List<IPAddress> allHops = GetTraceRoute("www.google.com");
            return allHops[0];
        }

        public static List<IPAddress> GetTraceRoute(string hostname)
        {
            // following are similar to the defaults in the "traceroute" unix command.
            const int timeout = 8000;
            const int maxTTL = 30;
            const int bufferSize = 32;
            List<IPAddress> theHops = new List<IPAddress>();

            byte[] buffer = new byte[bufferSize];
            new Random().NextBytes(buffer);

            using (var pinger = new Ping())
            {
                for (int ttl = 1; ttl <= maxTTL; ttl++)
                {
                    PingOptions options = new PingOptions(ttl, true);
                    PingReply reply = pinger.Send(hostname, timeout, buffer, options);

                    // we've found a route at this ttl
                    if (reply.Status == IPStatus.Success || reply.Status == IPStatus.TtlExpired)
                    {
                        theHops.Add(reply.Address);
                    }
                    // if we reach a status other than expired or timed out, we're done searching or there has been an error
                    if (reply.Status != IPStatus.TtlExpired && reply.Status != IPStatus.TimedOut)
                        break;
                }
            }
            return theHops;
        }
    }
}
