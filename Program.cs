namespace VmbNET
{
    internal static class Program
    {
        static unsafe void Main()
        {
            var si = sizeof(VmbCameraInfo);
            var ver =CameraManager.VersionQuery();
            CameraManager.CameraClose((nuint)((void*)null));
        }
    }
}