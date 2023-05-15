namespace VmbNET
{
    internal static class Program
    {
        static unsafe void Main()
        {
            CameraManager.Startup();
            var r = CameraManager.CamerasList();

            CameraManager.Shutdown();
        }
    }
}