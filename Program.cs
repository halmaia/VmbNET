using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VmbNET
{
    internal static class Program
    {
        static unsafe void Main()
        {
            CameraManager.Startup();

            (nuint handle, VmbFrame*[] frames) =
                CameraManager.StartAsyncRecordingOnFirstCamera(16, 20, &FrameArrived);

            CameraManager.FeatureInvalidationRegister(handle, "DeviceTemperature"u8, &TemperatureInvalidated, (void*)1);

            _ = Console.ReadKey();

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
              //  Console.WriteLine(frame->Timestamp.ToString());
            }
            CameraManager.CaptureFrameQueue(cameraHandle, frame, &FrameArrived);
        }

        [UnmanagedCallersOnly]
        private static unsafe void TemperatureInvalidated(nuint cameraHandle,
                                                          byte* name,
                                                          void* userContext) 
        {
            Console.WriteLine(CameraManager.FeatureFloatGet(cameraHandle, name));
        }
    }
}