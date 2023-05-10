using System;
using System.Diagnostics;

namespace SentryVision.Endpoint.Modules
{
    class Rtsp
    {
        public static void InitiateRtsp(string CameraId, int VideoWidth, int VideoHeight, int FrameRate, string RtspServer, int RtspPort, string RtspPath)
        {
            using (Process proc = new Process())
            {
                try
                {
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.FileName = "ffmpeg";
                    proc.StartInfo.Arguments = $"-f v4l2 -framerate ${FrameRate} -video_size {VideoWidth}x{VideoHeight} -i {CameraId} -f rtsp -rtsp_transport tcp rtsp://{RtspServer}:{RtspPort}/{RtspPath}";
                    proc.Start();
                    proc.WaitForExitAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}