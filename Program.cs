namespace VmbNET
{
    internal static class Program
    {
        static unsafe void Main()
        {
            CameraManager.Startup();

            var h = CameraManager.OpenFirstCamera();
            CameraManager.CameraClose(h);
            CameraManager.Shutdown();
        }
    }
}