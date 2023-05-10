using System;
using SentryVision.Endpoint.Modules;

namespace SentryVision.Endpoint
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("**** SentryVision Camera Endpoint ****");
            Console.WriteLine("Initiating RTSP connection with server");
            Rtsp.InitiateRtsp("/dev/video0", 640, 480, 15, "192.168.1.4", 8554, "svendpoint01");
        }
    }
}