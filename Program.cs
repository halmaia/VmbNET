namespace VmbNET
{
    internal static class Program
    {
        static void Main()
        {
            CameraManager.IsAPIUpAndRunning(out _);
            var x = CameraManager.IsVmbCAvailable;
            VmbVersionInfo vmbVersionInfo = new VmbVersionInfo();
            var s=vmbVersionInfo.ToString();
        }
    }
}