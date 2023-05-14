namespace VmbNET
{
    internal static class Program
    {
        static unsafe void Main()
        {
            var ver =CameraManager.VersionQuery();
             CameraManager.VersionQuery(null);
        }
    }
}