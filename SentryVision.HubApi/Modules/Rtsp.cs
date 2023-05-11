namespace SentryVision.HubApi.Modules
{
    public class Rtsp
    {
        private static readonly HttpClient Client = new()
        {
            BaseAddress = new Uri("http://127.0.0.1:9997/v1/")
        };

        public static async Task<string> GetRtspStreams()
        {
            Console.WriteLine("Making API call to get RTSP streams");
            try
            {
                var response = await Client.GetAsync("paths/list");
                return response.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e}");
                return "Error";
            }
        }
    }
}

