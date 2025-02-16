﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace VmbNET
{
    internal static class Program
    {
        static unsafe void Main()
        {
            CameraManager.Startup();
            double frameRate = 1;
            double exposureTime = 5000.8d;

            (nuint handle, VmbFrame*[] frames) =
                CameraManager.StartAsyncRecordingOnFirstCamera(16, ref exposureTime, ref frameRate, &FrameArrived,  true);
            CameraManager.RegisterDeviceTemperatureCallback(handle, &TemperatureInvalidated);

            _ = Console.ReadKey();

            CameraManager.UnRegisterDeviceTemperatureCallback(handle, &TemperatureInvalidated);
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

        [UnmanagedCallersOnly]
        private static unsafe void TemperatureInvalidated(nuint cameraHandle,
                                                          byte* name,
                                                          void* userContext) 
        {
            Console.WriteLine(CameraManager.FeatureFloatGet(cameraHandle, name).ToString()+"°C");
        }
    }
}