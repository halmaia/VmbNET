using System.Runtime.InteropServices;

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
            var frm = CameraManager.CreateFrameAndAnnounce(h, siz);
            CameraManager.CameraClose(h);
            CameraManager.Shutdown();
            NativeMemory.Free(frm.Buffer);
        }
    }
}