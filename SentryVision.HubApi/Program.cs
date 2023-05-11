using System;
using SentryVision.HubApi.Modules;

namespace SentryVision.HubApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();
            
            app.MapGet("/", () => "Hello SentryVision!");
            app.MapGet("/rtspPaths", Rtsp.GetRtspStreams);
            
            app.Run();
        }
    }
}