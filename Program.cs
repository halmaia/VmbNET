namespace VmbNET
{
    internal static class Program
    {
        static unsafe void Main()
        {
            var ver =CameraManager.VersionQuery();
            CameraManager.CameraClose((nuint)((void*)null));
        }
    }
}