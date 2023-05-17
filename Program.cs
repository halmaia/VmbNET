namespace VmbNET
{
    internal static class Program
    {
        static unsafe void Main()
        {
            var s2 = sizeof(VmbFrame);
            CameraManager.Startup();

            var h = CameraManager.OpenFirstCamera();
            var siz = CameraManager.PayloadSizeGet(h);
            var mf = CameraManager.CreateFramesAndAnnounce(h, siz, 16);
            
            CameraManager.CameraClose(h);
            CameraManager.Shutdown();
            CameraManager.FreeAllocatedFrames(mf);
        }
    }
}