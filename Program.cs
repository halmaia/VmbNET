using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace VmbNET
{
    internal static class Program
    {
        static unsafe void Main()
        {

            CameraManager.Startup();
            nuint handle = CameraManager.OpenFirstCamera();
            VmbFrame*[] frames = CameraManager.StartAsyncRecording(handle, 16, 20, &FrameArrived);

            Console.ReadKey();

            CameraManager.StopAsyncRecording(handle);
            CameraManager.CameraClose(handle);
            CameraManager.Shutdown();
            CameraManager.FreeAllocatedFrames(frames);
        }

        [UnmanagedCallersOnly]
        private static unsafe void FrameArrived(nuint cameraHandle, nuint streamHandle, [NotNull, DisallowNull] VmbFrame* frame)
        {
            if (frame->ReceiveStatus is VmbFrameStatus.VmbFrameStatusComplete)
            {
                Console.WriteLine(frame->Timestamp.ToString());
            }
            CameraManager.CaptureFrameQueue(cameraHandle, frame, &FrameArrived);
        }
    }
}