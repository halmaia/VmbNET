using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using VmbHandle = nuint;

namespace VmbNET
{
    public static class CameraManager
    {
        #region Private Constants
        private const string dllName = @"C:\Program Files\Allied Vision\Vimba X\api\bin\VmbC.dll";
        #endregion End – Private Constants

        #region Error Handling
        // TODO: Complete missing cases:
        [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
        private static void DetectError(ErrorType errorType)
        {
            if (errorType is not ErrorType.VmbErrorSuccess)
                ThrowAPIError(errorType);

            [DoesNotReturn]
            static void ThrowAPIError(ErrorType errorType) =>
            throw new Exception(errorType switch
            {
                ErrorType.VmbErrorInternalFault => "Internal fault.",
                ErrorType.VmbErrorSuccess => "No error.",
                _ => "Unknown error type."
            });
        }
        #endregion

        #region Startup functions
        /// <summary>
        /// Initializes the underlying VmbC API.
        /// </summary>
        /// <param name="pathConfiguration">
        /// A char pointer pointing to a semicolon (Windows) or colon (other os) separated list of paths.
        /// The paths contain directories to search for .cti files, paths to .cti files and optionally the path to a configuration xml file.
        /// If null is passed the parameter is the cti files found in the paths the GENICAM_GENTL{32|64}_PATH environment variable are considered.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Startup([AllowNull] char* pathConfiguration)
        {
            DetectError(VmbStartup(pathConfiguration!));

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
            EntryPoint = nameof(VmbStartup), ExactSpelling = true, PreserveSig = true, SetLastError = false)]
            static unsafe extern ErrorType VmbStartup(char* pathConfiguration);
        }

        /// <summary>
        /// Initializes the underlying VmbC API.
        /// </summary>
        /// <param name="pathConfiguration">
        /// A ReadOnlySpan<char> containing a semicolon (Windows) or colon (other os) separated list of paths.
        /// The paths contain directories to search for .cti files, paths to .cti files and optionally the path to a configuration xml file.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
        public static void Startup([DisallowNull] ReadOnlySpan<char> pathConfiguration)
        {
            ArgumentOutOfRangeException.ThrowIfZero(pathConfiguration!.Length, nameof(pathConfiguration));

            unsafe
            {
                fixed (char* pPathConfiguration = pathConfiguration!)
                    Startup(pPathConfiguration);
            }
        }

        /// <summary>
        /// Initializes the underlying VmbC API.
        /// </summary>
        /// <param name="pathConfiguration">
        /// A string containing a semicolon (Windows) or colon (other os) separated list of paths.
        /// The paths contain directories to search for .cti files, paths to .cti files and optionally the path to a configuration xml file.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Startup([DisallowNull] string pathConfiguration)
        {
            ArgumentException.ThrowIfNullOrEmpty(pathConfiguration, nameof(pathConfiguration));
            Startup((ReadOnlySpan<char>)pathConfiguration!);
        }

        /// <summary>
        /// Initializes the underlying VmbC API.
        /// </summary>
        /// <param name="pathConfiguration">
        /// The DirectoryInfo contains directory to search for .cti files, paths to .cti files and optionally the path to a configuration xml file.
        /// </param>
        [SkipLocalsInit]
        public static void Startup([DisallowNull] DirectoryInfo directoryInfo)
        {
            ArgumentNullException.ThrowIfNull(directoryInfo, nameof(directoryInfo));

            if (!directoryInfo!.Exists) // Early exit.
                throw new DirectoryNotFoundException();

            Startup((ReadOnlySpan<char>)directoryInfo!.FullName);
        }

        /// <summary>
        /// Initializes the underlying VmbC API.
        /// </summary>
        ///<remarks> The cti files found in the paths the GENICAM_GENTL{32|64}_PATH environment variable are considered.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Startup() => Startup((char*)null);

        /// <summary>
        /// Initializes the underlying VmbC API.
        /// </summary>
        /// <param name="pathConfiguration">
        /// A string containing a semicolon (Windows) or colon (other os) separated list of paths.
        /// The paths contain directories to search for .cti files, paths to .cti files and optionally the path to a configuration xml file.
        /// </param>
        public static void Startup([DisallowNull] IReadOnlyList<string> paths)
        {
            ArgumentNullException.ThrowIfNull(paths, nameof(paths));
            ArgumentOutOfRangeException.ThrowIfZero(paths!.Count, nameof(paths));
            Startup((ReadOnlySpan<char>)string.Join(',', paths!));
        }

        /// <summary>
        /// Initializes the underlying VmbC API.
        /// </summary>
        /// <param name="pathConfiguration">
        /// A string containing a semicolon (Windows) or colon (other os) separated list of paths.
        /// The paths contain directories to search for .cti files, paths to .cti files and optionally the path to a configuration xml file.
        /// </param>
        /// <param name="separator">
        /// Semicolon for Windows or colon for other os.
        /// </param>
        [SkipLocalsInit]
        public static void Startup([DisallowNull] string[] paths, [ConstantExpected] char separator = ';')
        {
            ArgumentNullException.ThrowIfNull(paths, nameof(paths));
            ArgumentOutOfRangeException.ThrowIfZero(paths!.Length, nameof(paths));

            if (separator is not ',' or ';')
                throw new ArgumentOutOfRangeException(nameof(separator), "Use semicolon for Windows or colon for other os!");

            Startup((ReadOnlySpan<char>)string.Join(separator, paths!));
        }
        #endregion

        #region Shutdown functions
        /// <summary>
        /// Perform a shutdown of the API. This frees some resources and deallocates all physical resources if applicable.
        /// </summary>
        /// <remarks>The call is silently ignored, if executed from a callback.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shutdown()
        {
            DetectError(VmbShutdown());

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
             EntryPoint = nameof(VmbShutdown), ExactSpelling = true, PreserveSig = true, SetLastError = false)]
            static extern ErrorType VmbShutdown();
        }
        #endregion

        #region API Test
        public static bool IsAPIUpAndRunning([NotNullWhen(false)] out string? errorMessage)
        {
            try
            {
                Startup();
                Shutdown();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }

            errorMessage = null;
            return true;
        }
        public static bool IsVmbCAvailable => File.Exists(dllName);
        #endregion End – API Test

        #region Version Query
        /// <summary>
        /// Retrieve the version number of VmbC.
        /// </summary>
        /// <remarks>This function can be called at anytime, even before the API is initialized.</remarks>
        /// <param name="versionInfo">Pointer to the struct where version information resides</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void VersionQuery([NotNull, DisallowNull] VmbVersionInfo* versionInfo)
        {
            ArgumentNullException.ThrowIfNull(versionInfo, nameof(versionInfo));
            DetectError(VmbVersionQuery(versionInfo!));

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
            EntryPoint = nameof(VmbVersionQuery), ExactSpelling = true, PreserveSig = true, SetLastError = false)]
            static extern ErrorType VmbVersionQuery(VmbVersionInfo* versionInfo,
            [ConstantExpected(Max = VmbVersionInfo.Size, Min = VmbVersionInfo.Size)]
            uint sizeofVersionInfo = VmbVersionInfo.Size);
        }

        /// <summary>
        /// Retrieve the version number of VmbC.
        /// </summary>
        /// <remarks>This function can be called at anytime, even before the API is initialized.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
        public static VmbVersionInfo VersionQuery()
        {
            VmbVersionInfo versionInfo;
            unsafe { VersionQuery(&versionInfo); }
            return versionInfo;
        }
        #endregion End – Version Query

        #region Close Camera
        public static void CameraClose([NotNull, DisallowNull] VmbHandle cameraHandle)
        {
            unsafe { ArgumentNullException.ThrowIfNull((void*)cameraHandle, nameof(cameraHandle)); }

            DetectError(VmbCameraClose(cameraHandle!));

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
             EntryPoint = nameof(VmbCameraClose), ExactSpelling = true, PreserveSig = true, SetLastError = false)]
            static extern ErrorType VmbCameraClose(VmbHandle cameraHandle);
        }
        #endregion End – Close Camera

        #region List Cameras
        public static unsafe void CamerasList(VmbCameraInfo* pCameraInfo, uint listLength,
                                              uint* pNumFound,
                                              uint sizeofCameraInfo)
        {
            ArgumentNullException.ThrowIfNull(pNumFound, nameof(pNumFound));

            DetectError(VmbCamerasList(pCameraInfo, listLength, pNumFound!, sizeofCameraInfo));

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
             EntryPoint = nameof(VmbCamerasList), ExactSpelling = true, SetLastError = false)]
            static unsafe extern ErrorType VmbCamerasList(VmbCameraInfo* pCameraInfo, uint listLength,
                                                          uint* pNumFound,
                                                          uint sizeofCameraInfo);
        }

        [return: MaybeNull]
        public static unsafe VmbCameraInfo[]? CamerasList(bool firstOnly = true)
        {
            uint NumFound;
            uint* pNumFound = &NumFound;
            CamerasList(null, 0u, pNumFound, 0u);

            if (NumFound == 0) return null;
            if (firstOnly) NumFound = 1u;

            VmbCameraInfo[] cameraInfo = GC.AllocateUninitializedArray<VmbCameraInfo>((int)NumFound, false);

            fixed (VmbCameraInfo* pCameraInfo = cameraInfo)
                CamerasList(pCameraInfo, NumFound, pNumFound, VmbCameraInfo.Size * NumFound);

            return cameraInfo;
        }

        [return: MaybeNull]
        public static VmbCameraInfo? GetFirstCamera() => CamerasList(true)?[0]; // Null propagation.
        #endregion End – List Cameras

        #region Open Cameras
        [SkipLocalsInit]
        public static unsafe VmbHandle CameraOpen([NotNull, DisallowNull] byte* idString,
                                                  VmbAccessMode accessMode = VmbAccessMode.VmbAccessModeExclusive)
        {
            ArgumentNullException.ThrowIfNull(idString, nameof(idString));

            VmbHandle cameraHandle;
            DetectError(VmbCameraOpen(idString!, accessMode, &cameraHandle));
            return cameraHandle;

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
             EntryPoint = nameof(VmbCameraOpen), ExactSpelling = true, SetLastError = false)]
            unsafe static extern ErrorType VmbCameraOpen(byte* idString, VmbAccessMode accessMode, VmbHandle* pCameraHandle);
        }

        [SkipLocalsInit]
        public static unsafe VmbHandle CameraOpen([DisallowNull] VmbCameraInfo camera,
                                                  VmbAccessMode accessMode = VmbAccessMode.VmbAccessModeExclusive) =>
            CameraOpen(camera.CameraIdExtended, accessMode);

        [SkipLocalsInit]
        public static VmbHandle OpenFirstCamera(VmbAccessMode accessMode = VmbAccessMode.VmbAccessModeExclusive)
        {
            VmbCameraInfo? firstCamera = GetFirstCamera();
            return firstCamera is null ?
                throw new NullReferenceException("No camera found!") :
                CameraOpen(firstCamera!.Value, accessMode);
        }
        #endregion End – Open Cameras

        #region Frame Announce
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void FrameAnnounce([NotNull, DisallowNull] VmbHandle cameraHandle,
                                                [NotNull, DisallowNull] VmbFrame* pFrame)
        {
            ArgumentNullException.ThrowIfNull((void*)cameraHandle, nameof(cameraHandle));
            ArgumentNullException.ThrowIfNull(pFrame, nameof(pFrame));

            DetectError(VmbFrameAnnounce(cameraHandle!, pFrame!));

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
             EntryPoint = nameof(VmbFrameAnnounce), ExactSpelling = true, SetLastError = false)]
            static unsafe extern ErrorType VmbFrameAnnounce(
                                      VmbHandle cameraHandle,
                                      VmbFrame* pFrame,
                                      [ConstantExpected(Max = VmbFrame.Size, Min = VmbFrame.Size)]
                                      uint sizeofFrame = VmbFrame.Size);
        }

        public static unsafe VmbFrame CreateFrameAndAnnounce([NotNull, DisallowNull] VmbHandle cameraHandle,
                                                             uint payloadSize)
        {
            VmbFrame frame = new(payloadSize);
            FrameAnnounce(cameraHandle, &frame);
            return frame;
        }

        #endregion End – Frame Announce

        #region Get Payload Size
        public static unsafe uint PayloadSizeGet([NotNull, DisallowNull] VmbHandle handle)
        {
            ArgumentNullException.ThrowIfNull((void*)handle, nameof(handle));

            uint payloadSize;
            DetectError(VmbPayloadSizeGet(handle!, &payloadSize));
            return payloadSize;

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
            EntryPoint = nameof(VmbPayloadSizeGet), ExactSpelling = true, SetLastError = false)]
            unsafe static extern ErrorType VmbPayloadSizeGet(VmbHandle handle, uint* payloadSize);
        }
        #endregion End – Get Payload Size

        #region Revoke frames
        /// <summary>
        /// In case of an failure some of the frames may have been revoked. To prevent this it is recommended to call
        /// CaptureQueueFlush for the same handle before invoking this function.
        /// </summary>
        /// <param name="handle">Handle for a stream or camera.</param>
        public static void FrameRevokeAll([NotNull, DisallowNull] VmbHandle handle)
        {
            unsafe { ArgumentNullException.ThrowIfNull((void*)handle, nameof(handle)); }

            DetectError(VmbFrameRevokeAll(handle!));

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
            EntryPoint = nameof(VmbFrameRevokeAll), ExactSpelling = true, SetLastError = false)]
            static extern ErrorType VmbFrameRevokeAll(VmbHandle handle);
        }

        public static unsafe void FrameRevoke([NotNull, DisallowNull] VmbHandle handle, [NotNull, DisallowNull] VmbFrame* frame)
        {
            ArgumentNullException.ThrowIfNull((void*)handle, nameof(handle));
            ArgumentNullException.ThrowIfNull(frame, nameof(frame));

            DetectError(VmbFrameRevoke(handle!, frame!));

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
            EntryPoint = nameof(VmbFrameRevoke), ExactSpelling = true, SetLastError = false)]
            static extern unsafe ErrorType VmbFrameRevoke(VmbHandle handle, VmbFrame* frame);
        }
        #endregion End – Revoke frames

        #region Capture Frame Queue
        /// <summary>
        /// Queue frames that may be filled during frame capturing.
        /// The given frame is put into a queue that will be filled sequentially.
        /// The order in which the frames are filled is determined by the order in which they are queued.
        /// If the frame was announced with FrameAnnounce() before, the application
        /// has to ensure that the frame is also revoked by calling FrameRevoke() or
        /// FrameRevokeAll() when cleaning up.
        /// </summary>
        /// <param name="handle">Handle of a camera or stream.</param>
        /// <param name="frame">Pointer to an already announced frame.</param>
        /// <param name="callback">Callback to be run when the frame is complete. Null is OK.</param>
        public static unsafe void CaptureFrameQueue([NotNull, DisallowNull] VmbHandle handle,
                                                    [NotNull, DisallowNull] VmbFrame* frame,
                                                    delegate* unmanaged<VmbHandle, VmbHandle, VmbFrame, void> callback)
        {
            ArgumentNullException.ThrowIfNull((void*)handle, nameof(handle));
            ArgumentNullException.ThrowIfNull(frame, nameof(frame));

            DetectError(VmbCaptureFrameQueue(handle!, frame!, callback!));

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
            EntryPoint = nameof(VmbCaptureFrameQueue), ExactSpelling = true, SetLastError = false)]
            static extern unsafe ErrorType VmbCaptureFrameQueue(VmbHandle handle,
                                                                VmbFrame* frame,
                                                                delegate* unmanaged<VmbHandle, VmbHandle, VmbFrame, void> callback);
        }
        #endregion End – Capture Frame Queue

        #region Capture Queue Flush
        public static void CaptureQueueFlush([NotNull, DisallowNull] VmbHandle handle)
        {
            unsafe { ArgumentNullException.ThrowIfNull((void*)handle, nameof(handle)); }

            DetectError(VmbCaptureQueueFlush(handle!));

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
            EntryPoint = nameof(VmbCaptureQueueFlush), ExactSpelling = true, SetLastError = false)]
            static extern ErrorType VmbCaptureQueueFlush(VmbHandle handle);
        }
        #endregion End – Capture Queue Flush

        #region Capture Start
        /// <summary>
        /// Prepare the API for incoming frames.
        /// </summary>
        /// <param name="handle">Handle for a camera or a stream.</param>
        public static void CaptureStart([NotNull, DisallowNull] VmbHandle handle)
        {
            unsafe { ArgumentNullException.ThrowIfNull((void*)handle, nameof(handle)); }

            DetectError(VmbCaptureStart(handle!));

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
            EntryPoint = nameof(VmbCaptureStart), ExactSpelling = true, SetLastError = false)]
            static extern ErrorType VmbCaptureStart(VmbHandle handle);
        }
        #endregion End – Capture End

        #region Capture Start
        /// <summary>
        /// Stop the API from being able to receive frames. The frame callback will not be called anymore.
        /// </summary>
        /// <param name="handle">Handle for a camera or a stream.</param>
        /// <remarks>
        /// This function waits for the completion of the last callback for the current capture.
        /// If the callback does not return in finite time, this function may not return in finite time either.
        /// </remarks>
        public static void CaptureEnd([NotNull, DisallowNull] VmbHandle handle)
        {
            unsafe { ArgumentNullException.ThrowIfNull((void*)handle, nameof(handle)); }

            DetectError(VmbCaptureEnd(handle!));

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
            EntryPoint = nameof(VmbCaptureEnd), ExactSpelling = true, SetLastError = false)]
            static extern ErrorType VmbCaptureEnd(VmbHandle handle);
        }
        #endregion End – Capture Start

        #region Command Run
        public static unsafe void FeatureCommandRun([NotNull, DisallowNull] VmbHandle handle,
                                                    [NotNull, DisallowNull] byte* name)
        {
            CheckFeatureArgs(handle, name);

            DetectError(VmbFeatureCommandRun(handle!, name!));

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
            EntryPoint = nameof(VmbFeatureCommandRun), ExactSpelling = true, SetLastError = false)]
            static extern unsafe ErrorType VmbFeatureCommandRun(VmbHandle handle, byte* name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
        public static unsafe void FeatureCommandRun([NotNull, DisallowNull] VmbHandle handle,
                                                    [NotNull, DisallowNull] ReadOnlySpan<byte> name)
        {
            fixed (byte* pName = name)
                FeatureCommandRun(handle, pName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AcquisitionStop([NotNull, DisallowNull] VmbHandle handle) =>
            FeatureCommandRun(handle, "AcquisitionStop"u8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AcquisitionStart([NotNull, DisallowNull] VmbHandle handle) =>
            FeatureCommandRun(handle, "AcquisitionStart"u8);

        #endregion End – Command Run

        #region Stop Async Recording
        public static void StopAsyncRecording([NotNull, DisallowNull] VmbHandle handle)
        {
            // 1. Stop image acquisition:
            AcquisitionStop(handle);

            // 2. Stop the capture engine:
            CaptureEnd(handle);

            // 3. Flush the capture queue:
            CaptureQueueFlush(handle);

            // 4. Revoke all frames:
            FrameRevokeAll(handle);
        }
        #endregion End – Stop Async Recording

        #region Feature Sets…
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void CheckFeatureArgs([NotNull, DisallowNull] VmbHandle handle,
                                                    [NotNull, DisallowNull] byte* name)
        {
            ArgumentNullException.ThrowIfNull((void*)handle, nameof(handle));
            ArgumentNullException.ThrowIfNull(name, nameof(name));
        }
        public static unsafe void FeatureBoolSet([NotNull, DisallowNull] VmbHandle handle,
                                                 [NotNull, DisallowNull] byte* name,
                                                 bool value) 
        {
            CheckFeatureArgs(handle, name);

            DetectError(VmbFeatureBoolSet(handle!, name!, value));

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
            EntryPoint = nameof(VmbFeatureBoolSet), ExactSpelling = true, SetLastError = false)]
            static extern unsafe ErrorType VmbFeatureBoolSet(VmbHandle handle, byte* name, bool value);
        }

        public static unsafe void FeatureBoolSet([NotNull, DisallowNull] VmbHandle handle,
                                         [NotNull, DisallowNull] ReadOnlySpan<byte> name,
                                         bool value)
        {
            fixed(byte* pName = name)
                FeatureBoolSet(handle, name, value);
        }

        public static unsafe void FeatureIntSet([NotNull, DisallowNull] VmbHandle handle,
                                         [NotNull, DisallowNull] byte* name,
                                         long value)
        {
            CheckFeatureArgs(handle, name);

            DetectError(VmbFeatureIntSet(handle!, name!, value));

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
            EntryPoint = nameof(VmbFeatureIntSet), ExactSpelling = true, SetLastError = false)]
            static extern unsafe ErrorType VmbFeatureIntSet(VmbHandle handle, byte* name, long value);
        }

        public static unsafe void FeatureIntSet([NotNull, DisallowNull] VmbHandle handle,
                                         [NotNull, DisallowNull] ReadOnlySpan<byte> name,
                                         long value)
        {
            fixed (byte* pName = name)
                FeatureIntSet(handle, name, value);
        }

        public static unsafe void FeatureFloatSet([NotNull, DisallowNull] VmbHandle handle,
                                         [NotNull, DisallowNull] byte* name,
                                         double value)
        {
            CheckFeatureArgs(handle, name);

            DetectError(VmbFeatureFloatSet(handle!, name!, value));

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
            EntryPoint = nameof(VmbFeatureFloatSet), ExactSpelling = true, SetLastError = false)]
            static extern unsafe ErrorType VmbFeatureFloatSet(VmbHandle handle, byte* name, double value);
        }

        public static unsafe void FeatureFloatSet([NotNull, DisallowNull] VmbHandle handle,
                                         [NotNull, DisallowNull] ReadOnlySpan<byte> name,
                                         double value)
        {
            fixed (byte* pName = name)
                FeatureFloatSet(handle, name, value);
        }

        #endregion End – Feature Sets…

        #region Convinience Sets
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetAcquisitionFrameRate(VmbHandle handle, double frameRate) => 
            FeatureFloatSet(handle, "AcquisitionFrameRate"u8, frameRate);
        #endregion End – Convinience Sets
    }
}