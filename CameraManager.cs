
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using VmbHandle = nuint;

namespace VmbNET;

public static class CameraManager
{
    #region Private Constants
    [NotNull]
    private const string dllName = @"C:\Program Files\Allied Vision\Vimba X\api\bin\VmbC.dll";
    #endregion End – Private Constants

    #region Error Handling
    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    private static void DetectError(ErrorType errorType)
    {
        if (errorType is not ErrorType.VmbErrorSuccess)
            ThrowAPIError(errorType);

        [DoesNotReturn]
        static void ThrowAPIError(ErrorType errorType)
        {
            Shutdown();

            throw new Exception(errorType switch
            {
                ErrorType.VmbErrorSuccess => "No error",
                ErrorType.VmbErrorInternalFault => "Unexpected fault in VmbC or driver",
                ErrorType.VmbErrorApiNotStarted => "VmbStartup() was not called before the current command",
                ErrorType.VmbErrorNotFound => "The designated instance (camera, feature etc.) cannot be found",
                ErrorType.VmbErrorBadHandle => "The given handle is not valid",
                ErrorType.VmbErrorDeviceNotOpen => "Device was not opened for usage",
                ErrorType.VmbErrorInvalidAccess => "Operation is invalid with the current access mode",
                ErrorType.VmbErrorBadParameter => "One of the parameters is invalid (usually an illegal pointer)",
                ErrorType.VmbErrorStructSize => "The given struct size is not valid for this version of the API",
                ErrorType.VmbErrorMoreData => "More data available in a string/list than space is provided",
                ErrorType.VmbErrorWrongType => "Wrong feature type for this access function",
                ErrorType.VmbErrorInvalidValue => "The value is not valid; either out of bounds or not an increment of the minimum",
                ErrorType.VmbErrorTimeout => "Timeout during wait",
                ErrorType.VmbErrorOther => "Other error",
                ErrorType.VmbErrorResources => "Resources not available (e.g. memory)",
                ErrorType.VmbErrorInvalidCall => "Call is invalid in the current context (e.g. callback)",
                ErrorType.VmbErrorNoTL => "No transport layers are found",
                ErrorType.VmbErrorNotImplemented => "API feature is not implemented",
                ErrorType.VmbErrorNotSupported => "API feature is not supported",
                ErrorType.VmbErrorIncomplete => "The current operation was not completed (e.g. a multiple registers read or write)",
                ErrorType.VmbErrorIO => "Low level IO error in transport layer",
                ErrorType.VmbErrorValidValueSetNotPresent => "The valid value set could not be retrieved, since the feature does not provide this property",
                ErrorType.VmbErrorGenTLUnspecified => "Unspecified GenTL runtime error",
                ErrorType.VmbErrorUnspecified => "Unspecified runtime error",
                ErrorType.VmbErrorBusy => "The responsible module/entity is busy executing actions",
                ErrorType.VmbErrorNoData => "The function has no data to work on",
                ErrorType.VmbErrorParsingChunkData => "An error occurred parsing a buffer containing chunk data",
                ErrorType.VmbErrorInUse => "Something is already in use",
                ErrorType.VmbErrorUnknown => "Error condition unknown",
                ErrorType.VmbErrorXml => "Error parsing XML",
                ErrorType.VmbErrorNotAvailable => "Something is not available",
                ErrorType.VmbErrorNotInitialized => "Something is not initialized",
                ErrorType.VmbErrorInvalidAddress => "The given address is out of range or invalid for internal reasons",
                ErrorType.VmbErrorAlready => "Something has already been done",
                ErrorType.VmbErrorNoChunkData => "A frame expected to contain chunk data does not contain chunk data",
                ErrorType.VmbErrorUserCallbackException => "A callback provided by the user threw an exception",
                ErrorType.VmbErrorFeaturesUnavailable => "The XML for the module is currently not loaded; the module could be in the wrong state or the XML could not be retrieved or could not be parsed properly",
                ErrorType.VmbErrorTLNotFound => "A required transport layer could not be found or loaded",
                ErrorType.VmbErrorAmbiguous => "An entity cannot be uniquely identified based on the information provided",
                ErrorType.VmbErrorRetriesExceeded => "Something could not be accomplished with a given number of retries",
                ErrorType.VmbErrorInsufficientBufferCount => "The operation requires more buffers",
                ErrorType.VmbErrorCustom => "User defined error (1).",
                _ => "Unknown error.",
            });
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    private static void CheckProcessType()
    {
        if (!Environment.Is64BitProcess)
            throw new PlatformNotSupportedException("Only 64-bit platforms are supported!");
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
    public static unsafe void Startup([AllowNull] char* pathConfiguration = null)
    {
        CheckProcessType();
        DetectError(VmbStartup(pathConfiguration));

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
        EntryPoint = nameof(VmbStartup), ExactSpelling = true, PreserveSig = true, SetLastError = false)]
        static unsafe extern ErrorType VmbStartup([AllowNull] char* pathConfiguration);
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
        Startup(pathConfiguration!);
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

        Startup(directoryInfo!.FullName!);
    }

    /// <summary>
    /// Initializes the underlying VmbC API.
    /// </summary>
    /// <param name="pathConfiguration">
    /// A string containing a semicolon (Windows) or colon (other os) separated list of paths.
    /// The paths contain directories to search for .cti files, paths to .cti files and optionally the path to a configuration xml file.
    /// </param>
    public static void Startup(ReadOnlySpan<string> paths, [ConstantExpected] char separator = ';')
    {
        ThrowOnSeparatorError(separator);
        ArgumentOutOfRangeException.ThrowIfZero(paths.Length, nameof(paths));

        Startup(string.Join(separator, paths));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ThrowOnSeparatorError([ConstantExpected] char separator)
    {
        if (separator is not ';' or ':')
        {
            ThrowSeparatorError();

            [DoesNotReturn]
            static void ThrowSeparatorError() =>
            throw new ArgumentOutOfRangeException(nameof(separator), "Use semicolon for Windows or colon for other os!");
        }
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
    public static void Startup([DisallowNull] string[] paths,
                               [ConstantExpected]
                               char separator = ';')
    {
        ArgumentNullException.ThrowIfNull(paths, nameof(paths));
        ArgumentOutOfRangeException.ThrowIfZero(paths!.Length, nameof(paths));
        ThrowOnSeparatorError(separator);

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
        VmbShutdown();

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
         EntryPoint = nameof(VmbShutdown), ExactSpelling = true, PreserveSig = true, SetLastError = false)]
        static extern void VmbShutdown();
    }
    #endregion

    #region API Test
    [SkipLocalsInit]
    public static bool IsAPIUpAndRunning([NotNullWhen(false)] out string? errorMessage)
    {
        if (!IsVmbCDLLAvailable)
            throw new FileNotFoundException("Required dll not found.", dllName);

        try
        {
            unsafe
            {
                Startup();
            }
            Shutdown();
        }
        catch (Exception ex)
        {
            errorMessage = ex.ToString();
            return false;
        }

        errorMessage = null;
        return true;
    }

    public static bool IsVmbCDLLAvailable => File.Exists(dllName);
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
        CheckProcessType();
        ArgumentNullException.ThrowIfNull(versionInfo, nameof(versionInfo));

        DetectError(VmbVersionQuery(versionInfo!));

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
        EntryPoint = nameof(VmbVersionQuery), ExactSpelling = true, PreserveSig = true, SetLastError = false)]
        static extern ErrorType VmbVersionQuery(
            [NotNull, DisallowNull] VmbVersionInfo* versionInfo,
            [ConstantExpected(Max = VmbVersionInfo.Size, Min = VmbVersionInfo.Size)]
            uint sizeofVersionInfo = VmbVersionInfo.Size);
    }

    /// <summary>
    /// Retrieve the version number of VmbC.
    /// </summary>
    /// <remarks>This function can be called at anytime, even before the API is initialized.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe VmbVersionInfo VersionQuery()
    {
        Unsafe.SkipInit(out VmbVersionInfo versionInfo);
        VersionQuery(&versionInfo);
        return versionInfo;
    }
    #endregion End – Version Query

    #region Close Camera
    public static void CameraClose([NotNull, DisallowNull] VmbHandle cameraHandle)
    {
        if (cameraHandle is 0)
            throw new ArgumentNullException(nameof(cameraHandle));

        DetectError(VmbCameraClose(cameraHandle!));

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
         EntryPoint = nameof(VmbCameraClose), ExactSpelling = true, PreserveSig = true, SetLastError = false)]
        static extern ErrorType VmbCameraClose(VmbHandle cameraHandle);
    }
    #endregion End – Close Camera

    #region List Cameras
    public static unsafe void CamerasList(VmbCameraInfo* pCameraInfo, uint listLength,
                                          uint* pNumFound,
                                          [ConstantExpected] uint sizeofCameraInfo = VmbCameraInfo.Size)
    {
        ArgumentNullException.ThrowIfNull(pNumFound, nameof(pNumFound));

        DetectError(VmbCamerasList(pCameraInfo, listLength, pNumFound!, sizeofCameraInfo));

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
         EntryPoint = nameof(VmbCamerasList), ExactSpelling = true, SetLastError = false)]
        static unsafe extern ErrorType VmbCamerasList(VmbCameraInfo* pCameraInfo, uint listLength,
                                                      uint* pNumFound,
                                                      [ConstantExpected] uint sizeofCameraInfo = VmbCameraInfo.Size);
    }

    [SkipLocalsInit]
    [return: MaybeNull]
    public static unsafe VmbCameraInfo[]? CamerasList(bool firstOnly = true)
    {
        uint NumFound;
        CamerasList(null, 0u, &NumFound);

        if (NumFound is 0u) return null;
        if (firstOnly) NumFound = 1u;

        VmbCameraInfo[] cameraInfo = new VmbCameraInfo[NumFound];

        fixed (VmbCameraInfo* pCameraInfo = cameraInfo)
            CamerasList(pCameraInfo, NumFound, &NumFound);

        return cameraInfo;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static VmbCameraInfo GetFirstCamera() =>
        (CamerasList(true) ?? throw new NullReferenceException("No camera found!"))[0];
    #endregion End – List Cameras

    #region Open Cameras
    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe VmbHandle CameraOpen([DisallowNull] in VmbCameraInfo camera,
                                              VmbAccessMode accessMode = VmbAccessMode.VmbAccessModeExclusive) =>
        CameraOpen(camera.CameraIdExtended, accessMode);

    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe VmbHandle OpenFirstCamera(VmbAccessMode accessMode = VmbAccessMode.VmbAccessModeExclusive) =>
        CameraOpen(GetFirstCamera().CameraIdExtended, accessMode);
    #endregion End – Open Cameras

    #region Frame Announce
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void FrameAnnounce([NotNull, DisallowNull] VmbHandle cameraHandle,
                                            [NotNull, DisallowNull] VmbFrame* pFrame)
    {
        ArgumentNullException.ThrowIfNull(cameraHandle.ToPointer(), nameof(cameraHandle));
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

    [SkipLocalsInit]
    public static unsafe VmbFrame* CreateFrameAndAnnounce([NotNull, DisallowNull] VmbHandle cameraHandle,
                                                          uint payloadSize)
    {
        ArgumentOutOfRangeException.ThrowIfZero(payloadSize);

        VmbFrame* pFrame = (VmbFrame*)NativeMemory.AlignedAlloc(VmbFrame.Size, 64u);
        Unsafe.InitBlock(pFrame, 0, VmbFrame.Size);
        *(uint*)(((byte*)pFrame) + 8) = payloadSize;
        FrameAnnounce(cameraHandle, pFrame);
        return pFrame;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe void FreeAllocatedFrames(VmbFrame*[] frames)
    {
        if (frames is not null)
        {
            int i = 0, len = frames.Length;
            while (i != len)
            {
                NativeMemory.AlignedFree(frames[i]);
                frames[i++] = null;
            }
        }
    }

    [SkipLocalsInit, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe VmbFrame*[] CreateFramesAndAnnounce([NotNull, DisallowNull] VmbHandle cameraHandle,
                                                             uint payloadSize,
                                                             uint numberOfBufferFrames)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(numberOfBufferFrames, 3u, nameof(numberOfBufferFrames));

        VmbFrame*[] frames = new VmbFrame*[numberOfBufferFrames];

        uint i = 0u;
        do
            frames[i++] = CreateFrameAndAnnounce(cameraHandle, payloadSize);
        while (i != numberOfBufferFrames);

        return frames;
    }
    #endregion End – Frame Announce

    #region Get Payload Size
    [SkipLocalsInit]
    public static unsafe uint PayloadSizeGet([NotNull, DisallowNull] VmbHandle handle)
    {
        ArgumentNullException.ThrowIfNull(handle.ToPointer(), nameof(handle));

        uint payloadSize;
        DetectError(VmbPayloadSizeGet(handle!, &payloadSize));
        return payloadSize;

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
        EntryPoint = nameof(VmbPayloadSizeGet), ExactSpelling = true, SetLastError = false)]
        unsafe static extern ErrorType VmbPayloadSizeGet([NotNull] VmbHandle handle, [NotNull] uint* payloadSize);
    }
    #endregion End – Get Payload Size

    #region Revoke frames
    /// <summary>
    /// In case of a failure some of the frames may have been revoked. To prevent this it is recommended to call
    /// CaptureQueueFlush for the same handle before invoking this function.
    /// </summary>
    /// <param name="handle">Handle for a stream or camera.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    /// <see cref="FrameRevoke(VmbHandle, VmbFrame*)"/> when cleaning up.
    /// </summary>
    /// <param name="handle">Handle of a camera or stream.</param>
    /// <param name="frame">Pointer to an already announced frame.</param>
    /// <param name="callback">Callback to be run when the frame is complete. Null is OK.</param>
    public static unsafe void CaptureFrameQueue([NotNull, DisallowNull] VmbHandle handle,
                                                [NotNull, DisallowNull] VmbFrame* frame,
                                                delegate* unmanaged<VmbHandle, VmbHandle, VmbFrame*, void> callback)
    {
        ArgumentNullException.ThrowIfNull((void*)handle, nameof(handle));
        ArgumentNullException.ThrowIfNull(frame, nameof(frame));
        // Do not test callback for null! See <param name="callback"> above.

        DetectError(VmbCaptureFrameQueue(handle!, frame!, callback));

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
        EntryPoint = nameof(VmbCaptureFrameQueue), ExactSpelling = true, SetLastError = false)]
        static extern unsafe ErrorType VmbCaptureFrameQueue(VmbHandle handle,
                                                            VmbFrame* frame,
                                                            delegate* unmanaged<VmbHandle, VmbHandle, VmbFrame*, void> callback);
    }

    [SkipLocalsInit]
    public static unsafe void QueueFrames([NotNull, DisallowNull] VmbHandle handle,
                                          [NotNull, DisallowNull] VmbFrame*[] frames,
                                          delegate* unmanaged<VmbHandle, VmbHandle, VmbFrame*, void> callback)
    {
        ArgumentNullException.ThrowIfNull(frames, nameof(frames));
        int len = frames!.Length;
        ArgumentOutOfRangeException.ThrowIfZero(len, nameof(frames));

        do
            CaptureFrameQueue(handle, frames[--len], callback);
        while (len is not 0);
    }
    #endregion End – Capture Frame Queue

    #region Capture Queue Flush
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        ArgumentNullException.ThrowIfNull(handle.ToPointer(), nameof(handle));
        ArgumentNullException.ThrowIfNull(name, nameof(name));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe void FeatureBoolSet([NotNull, DisallowNull] VmbHandle handle,
                                     [NotNull, DisallowNull] ReadOnlySpan<byte> name,
                                     bool value)
    {
        fixed (byte* pName = name)
            FeatureBoolSet(handle, pName, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe void FeatureIntSet([NotNull, DisallowNull] VmbHandle handle,
                                     [NotNull, DisallowNull] ReadOnlySpan<byte> name,
                                     long value)
    {
        fixed (byte* pName = name)
            FeatureIntSet(handle, pName, value);
    }

    public static unsafe bool TryFeatureFloatSet([NotNull, DisallowNull] VmbHandle handle,
                                                 [NotNull, DisallowNull] ReadOnlySpan<byte> name,
                                                 ref double value)
    {

        fixed (byte* pName = name)
        {
            bool isReadable, isWriteable;
            FeatureAccessQuery(handle, pName, &isReadable, &isWriteable);
            if (isWriteable && isReadable)
            {
                double min, max;
                FeatureFloatRangeQuery(handle, pName, &min, &max);
                value = double.Clamp(value, min, max);
                FeatureFloatSet(handle, pName, value);
                value = FeatureFloatGet(handle, pName);
                return true;
            }
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe void FeatureFloatSet([NotNull, DisallowNull] VmbHandle handle,
                                              [NotNull, DisallowNull] ReadOnlySpan<byte> name,
                                              double value)
    {
        fixed (byte* pName = name)
            FeatureFloatSet(handle, pName, value);
    }

    public static unsafe void WriteUserDataFileAtZero([NotNull, DisallowNull] VmbHandle handle,
                                                [NotNull, DisallowNull] byte* buffer, uint bufferSize)
    {
        FeatureEnumSet(handle, "FileSelector"u8, "UserData"u8);
        FeatureEnumSet(handle, "FileOperationSelector"u8, "Delete"u8);
        FeatureCommandRun(handle, "FileOperationExecute"u8);
        FeatureEnumSet(handle, "FileOperationSelector"u8, "Open"u8);
        FeatureEnumSet(handle, "FileOpenMode"u8, "Write"u8);
        FeatureCommandRun(handle, "FileOperationExecute"u8);
        FeatureEnumSet(handle, "FileOperationSelector"u8, "Write"u8);
        FeatureRawSet(handle, "FileAccessBuffer"u8, buffer, bufferSize);
        FeatureCommandRun(handle, "FileOperationExecute"u8);
        FeatureEnumSet(handle, "FileOperationSelector"u8, "Close"u8);
        FeatureCommandRun(handle, "FileOperationExecute"u8);
    }

    public static unsafe void ReadUserDataAtZero([NotNull, DisallowNull] VmbHandle handle,
                                                [NotNull, DisallowNull] byte* buffer, uint bufferSize, uint* sizeFilled)
    {
        FeatureEnumSet(handle, "FileSelector"u8, "UserData"u8);
        FeatureEnumSet(handle, "FileOperationSelector"u8, "Open"u8);
        FeatureEnumSet(handle, "FileOpenMode"u8, "Read"u8);
        FeatureCommandRun(handle, "FileOperationExecute"u8);
        FeatureEnumSet(handle, "FileOperationSelector"u8, "Read"u8);
        FeatureCommandRun(handle, "FileOperationExecute"u8);
        FeatureRawGet(handle, "FileAccessBuffer"u8, buffer, 4, sizeFilled);
        FeatureEnumSet(handle, "FileOperationSelector"u8, "Close"u8);
        FeatureCommandRun(handle, "FileOperationExecute"u8);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void FeatureRawSet([NotNull, DisallowNull] VmbHandle handle,
                                        ReadOnlySpan<byte> name,
                                        [NotNull, DisallowNull] byte* buffer,
                                        uint bufferSize)
    {
        fixed (byte* pName = name)
            FeatureRawSet(handle, pName, buffer, bufferSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void FeatureRawSet([NotNull, DisallowNull] VmbHandle handle,
                                            [NotNull, DisallowNull] byte* name,
                                            [NotNull, DisallowNull] byte* buffer,
                                            uint bufferSize)
    {
        CheckFeatureArgs(handle, name);
        ArgumentNullException.ThrowIfNull(buffer);

        DetectError(VmbFeatureRawSet(handle!, name!, buffer, bufferSize));

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
        EntryPoint = nameof(VmbFeatureRawSet), ExactSpelling = true, SetLastError = false)]
        static extern unsafe ErrorType VmbFeatureRawSet(VmbHandle handle, byte* name, byte* buffer,
                                                        uint bufferSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void FeatureRawGet([NotNull, DisallowNull] VmbHandle handle,
                                    ReadOnlySpan<byte> name,
                                    [NotNull, DisallowNull] byte* buffer,
                                    uint bufferSize,
                                    [NotNull, DisallowNull] uint* sizeFilled)
    {
        fixed (byte* pName = name)
            FeatureRawGet(handle, pName, buffer, bufferSize, sizeFilled);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void FeatureRawGet([NotNull, DisallowNull] VmbHandle handle,
                                            [NotNull, DisallowNull] byte* name,
                                            [NotNull, DisallowNull] byte* buffer,
                                            uint bufferSize,
                                            [NotNull, DisallowNull] uint* sizeFilled)
    {
        CheckFeatureArgs(handle, name);
        ArgumentNullException.ThrowIfNull(buffer);

        DetectError(VmbFeatureRawGet(handle!, name!, buffer, bufferSize, sizeFilled));

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
        EntryPoint = nameof(VmbFeatureRawGet), ExactSpelling = true, SetLastError = false)]
        static extern unsafe ErrorType VmbFeatureRawGet(VmbHandle handle, byte* name, byte* buffer,
                                                        uint bufferSize,
                                                        uint* sizeFilled);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void FeatureEnumSetUnsafe([NotNull, DisallowNull] VmbHandle handle,
                                                   [NotNull, DisallowNull] byte* name,
                                                   [NotNull, DisallowNull] byte* value)
    {
        DetectError(VmbFeatureEnumSet(handle!, name!, value!));

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
        EntryPoint = nameof(VmbFeatureEnumSet), ExactSpelling = true, SetLastError = false),
        SuppressGCTransition, SuppressUnmanagedCodeSecurity]
        static extern unsafe ErrorType VmbFeatureEnumSet(VmbHandle handle, byte* name, byte* value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void FeatureEnumSet([NotNull, DisallowNull] VmbHandle handle,
                                             [NotNull, DisallowNull] byte* name,
                                             [NotNull, DisallowNull] byte* value)
    {
        CheckFeatureArgs(handle, name);
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        DetectError(VmbFeatureEnumSet(handle!, name!, value!));

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
        EntryPoint = nameof(VmbFeatureEnumSet), ExactSpelling = true, SetLastError = false)]
        static extern unsafe ErrorType VmbFeatureEnumSet(VmbHandle handle, byte* name, byte* value);
    }

    [SkipLocalsInit]
    public static unsafe void FeatureEnumSet([NotNull, DisallowNull] VmbHandle handle,
                                             [NotNull, DisallowNull] ReadOnlySpan<byte> name,
                                             [NotNull, DisallowNull] ReadOnlySpan<byte> value)
    {
        fixed (byte* pValue = value)
        fixed (byte* pName = name)
            FeatureEnumSet(handle, pName, pValue);
    }
    #endregion End – Feature Sets…

    #region Convinience Sets
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetAcquisitionFrameRate(VmbHandle handle, double frameRate)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(frameRate, double.Epsilon);
        FeatureFloatSet(handle, "AcquisitionFrameRate"u8, frameRate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetDeviceUserID(VmbHandle handle) =>
        FeatureStringGet(handle, "DeviceUserID"u8);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetExposureAutoToOff(VmbHandle handle) =>
      FeatureEnumSet(handle, "ExposureAuto"u8, "Off"u8);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetExposureAutoToOn(VmbHandle handle) =>
      FeatureEnumSet(handle, "ExposureAuto"u8, "On"u8);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetExposureTime(VmbHandle handle, double exposureTime) =>
       FeatureFloatSet(handle, "ExposureTime"u8, exposureTime);
    public static bool TrySetExposureTime(VmbHandle handle, ref double exposureTime) =>
        TryFeatureFloatSet(handle, "ExposureTime"u8, ref exposureTime);
    public static bool TrySetAcquisitionFrameRate(VmbHandle handle, ref double frameRate) =>
        TryFeatureFloatSet(handle, "AcquisitionFrameRate"u8, ref frameRate);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetDeviceLinkThroughputLimitModeToOff(VmbHandle handle) =>
        FeatureEnumSet(handle, "DeviceLinkThroughputLimitMode"u8, "Off"u8);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetDeviceLinkThroughputLimitModeToOn(VmbHandle handle) =>
        FeatureEnumSet(handle, "DeviceLinkThroughputLimitMode"u8, "On"u8);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetMaxDriverBuffersCount(VmbHandle handle, long newMaxValue) =>
        FeatureIntSet(handle, "MaxDriverBuffersCount"u8, newMaxValue);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetAcquisitionModeToContinuous(VmbHandle handle) =>
        FeatureEnumSet(handle, "AcquisitionMode"u8, "Continuous"u8);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetAcquisitionFrameRateEnableToTrue(VmbHandle handle) =>
        FeatureBoolSet(handle, "AcquisitionFrameRateEnable"u8, true);

    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static long GetTimestampLatchValue(VmbHandle handle)
    {
        FeatureCommandRun(handle, "TimestampLatch"u8);
        return FeatureIntGet(handle, "TimestampLatchValue"u8);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static void ResetTimestamp(VmbHandle handle)
    {
        FeatureCommandRun(handle, "TimestampReset"u8);
    }


    #endregion End – Convinience Sets

    #region Start Async Recording
    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe (VmbHandle cameraHandle, VmbFrame*[] frames) StartAsyncRecordingOnFirstCamera(
                                         [ConstantExpected(Max = 64u, Min = 3u)]
                                         uint numberOfBufferFrames,
                                         ref double exposureTime,
                                         ref double frameRate,
                                         delegate* unmanaged<VmbHandle, VmbHandle, VmbFrame*, void> callback,
                                         [ConstantExpected]
                                         bool triggeringOnLine0 = false)
    {
        VmbHandle cameraHandle;
        return (cameraHandle = OpenFirstCamera(),
        StartAsyncRecording(cameraHandle, numberOfBufferFrames, ref exposureTime, ref frameRate, callback, triggeringOnLine0));
    }

    [SkipLocalsInit]
    public static unsafe VmbFrame*[] StartAsyncRecording([NotNull, DisallowNull] VmbHandle cameraHandle,
                                            [ConstantExpected(Max = 64u, Min = 3u)]
                                            uint numberOfBufferFrames,
                                            ref double exposureTime,
                                            ref double frameRate,
                                            delegate* unmanaged<VmbHandle, VmbHandle, VmbFrame*, void> callback,
                                            [ConstantExpected]
                                            bool triggeringOnLine0 = false)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(frameRate, nameof(frameRate));

        SetDeviceLinkThroughputLimitModeToOff(cameraHandle);
        SetAcquisitionModeToContinuous(cameraHandle);

        if (triggeringOnLine0)
        {
            ActivateExternalTriggeringOnLine1(cameraHandle);
        }
        else
        {
            SetAcquisitionFrameRateEnableToTrue(cameraHandle);
            if (!TrySetExposureTime(cameraHandle, ref exposureTime))
                throw new Exception("Unable to set exposure time.");
            if (!TrySetAcquisitionFrameRate(cameraHandle, ref frameRate))
                throw new Exception("Unable to set frame rate.");
        }

        VmbFrame*[] frames = CreateFramesAndAnnounce(cameraHandle, PayloadSizeGet(cameraHandle), numberOfBufferFrames);

        CaptureStart(cameraHandle);

        QueueFrames(cameraHandle, frames, callback);

        AcquisitionStart(cameraHandle);

        return frames;
    }
    #endregion End – Start Async Recording

    #region Feature Access Query
    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe void FeatureAccessQuery([NotNull, DisallowNull] VmbHandle handle,
                                                    [NotNull, DisallowNull] byte* name,
                                                    [NotNull, DisallowNull] bool* isReadable,
                                                    [NotNull, DisallowNull] bool* isWriteable)
    {
        CheckFeatureArgs(handle, name);
        ArgumentNullException.ThrowIfNull(isReadable, nameof(isReadable));
        ArgumentNullException.ThrowIfNull(isWriteable, nameof(isWriteable));

        DetectError(VmbFeatureAccessQuery(handle!, name!, isReadable!, isWriteable!));

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
         EntryPoint = nameof(VmbFeatureAccessQuery), ExactSpelling = true, SetLastError = false)]
        static unsafe extern ErrorType VmbFeatureAccessQuery(VmbHandle handle,
                                                             byte* name,
                                                             bool* isReadable,
                                                             bool* isWriteable);

    }
    public static unsafe (bool isWriteable, bool isReadable) FeatureAccessQuery([NotNull, DisallowNull] VmbHandle handle,
                                                                                   [NotNull, DisallowNull] byte* name)
    {
        (bool isWriteable, bool isReadable) featureAccess;
        FeatureAccessQuery(handle, name, &featureAccess.isReadable, &featureAccess.isWriteable);
        return featureAccess;
    }

    public static unsafe (bool isWriteable, bool isReadable) FeatureAccessQuery([NotNull, DisallowNull] VmbHandle handle,
                                                                                   [NotNull, DisallowNull] ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
            return FeatureAccessQuery(handle, pName);
    }

    #endregion End – Feature Info Query

    #region Feature Info Query
    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe void FeatureInfoQuery([NotNull, DisallowNull] VmbHandle handle,
                                                  [NotNull, DisallowNull] byte* name,
                                                  [NotNull, DisallowNull] VmbFeatureInfo* featureInfo)
    {
        CheckFeatureArgs(handle, name);
        ArgumentNullException.ThrowIfNull(featureInfo, nameof(featureInfo));

        DetectError(VmbFeatureInfoQuery(handle!, name!, featureInfo!));

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
         EntryPoint = nameof(VmbFeatureInfoQuery), ExactSpelling = true, SetLastError = false)]
        static unsafe extern ErrorType VmbFeatureInfoQuery(VmbHandle handle,
                                                           byte* name,
                                                           VmbFeatureInfo* featureInfo,
                                                           [ConstantExpected(Max = VmbFeatureInfo.Size, Min = VmbFeatureInfo.Size)]
                                                           uint sizeofFeatureInfo = VmbFeatureInfo.Size);

    }
    public static unsafe VmbFeatureInfo FeatureInfoQuery([NotNull, DisallowNull] VmbHandle handle,
                                                            [NotNull, DisallowNull] byte* name)
    {
        VmbFeatureInfo featureInfo;
        FeatureInfoQuery(handle, name, &featureInfo);
        return featureInfo;
    }

    public static unsafe VmbFeatureInfo FeatureInfoQuery([NotNull, DisallowNull] VmbHandle handle,
                                                            [NotNull, DisallowNull] ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
            return FeatureInfoQuery(handle, pName);
    }

    #endregion End – Feature Info Query

    #region Fearure Range Query
    public static unsafe void FeatureFloatRangeQuery([NotNull, DisallowNull] VmbHandle handle,
                                             [NotNull, DisallowNull] byte* name,
                                             [NotNull, DisallowNull] double* min,
                                             [NotNull, DisallowNull] double* max)
    {
        CheckFeatureArgs(handle, name);
        ArgumentNullException.ThrowIfNull(min, nameof(min));
        ArgumentNullException.ThrowIfNull(max, nameof(max));

        DetectError(VmbFeatureFloatRangeQuery(handle!, name!, min!, max!));

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
         EntryPoint = nameof(VmbFeatureFloatRangeQuery), ExactSpelling = true, SetLastError = false)]
        static unsafe extern ErrorType VmbFeatureFloatRangeQuery(VmbHandle handle,
                                                             byte* name,
                                                             double* min,
                                                             double* max);

    }

    public static unsafe (double min, double max) FeatureFloatRangeQuery([NotNull, DisallowNull] VmbHandle handle,
                                                                           [NotNull, DisallowNull] byte* name)
    {
        (double min, double max) featureRange;
        FeatureFloatRangeQuery(handle, name, &featureRange.min, &featureRange.max);
        return featureRange;
    }

    public static unsafe (double min, double max) FeatureFloatRangeQuery([NotNull, DisallowNull] VmbHandle handle,
                                                                                   [NotNull, DisallowNull] ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
            return FeatureFloatRangeQuery(handle, pName);
    }
    #endregion End – Fearure Range Query

    #region Feature Invalidation Register

    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe void FeatureInvalidationRegister([NotNull, DisallowNull] VmbHandle handle,
                                                          [NotNull, DisallowNull] ReadOnlySpan<byte> name,
                                                          [NotNull, DisallowNull] delegate* unmanaged<VmbHandle, byte*, void*, void> callback,
                                                          void* userContext)
    {
        fixed (byte* pName = name)
            FeatureInvalidationRegister(handle, pName, callback, userContext);
    }

    [SkipLocalsInit]
    public static unsafe void FeatureInvalidationRegister([NotNull, DisallowNull] VmbHandle handle,
                                                      [NotNull, DisallowNull] byte* name,
                                                      [NotNull, DisallowNull] delegate* unmanaged<VmbHandle, byte*, void*, void> callback,
                                                      void* userContext)
    {
        CheckFeatureArgs(handle, name);
        ArgumentNullException.ThrowIfNull(callback, nameof(callback));

        DetectError(VmbFeatureInvalidationRegister(handle, name, callback, userContext));

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
        EntryPoint = nameof(VmbFeatureInvalidationRegister), ExactSpelling = true, SetLastError = false)]
        static extern unsafe ErrorType VmbFeatureInvalidationRegister(VmbHandle handle,
                                                byte* name,
                                                delegate* unmanaged<VmbHandle, byte*, void*, void> callback,
                                                void* userContext);
    }
    #endregion End – Feature Invalidation Register

    #region Feature Invalidation UnRegister
    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe void FeatureInvalidationUnRegister([NotNull, DisallowNull] VmbHandle handle,
                                              [NotNull, DisallowNull] ReadOnlySpan<byte> name,
                                              [NotNull, DisallowNull] delegate* unmanaged<VmbHandle, byte*, void*, void> callback)
    {
        fixed (byte* pName = name)
            FeatureInvalidationUnRegister(handle, pName, callback);
    }

    [SkipLocalsInit]
    public static unsafe void FeatureInvalidationUnRegister([NotNull, DisallowNull] VmbHandle handle,
                                              [NotNull, DisallowNull] byte* name,
                                              [NotNull, DisallowNull] delegate* unmanaged<VmbHandle, byte*, void*, void> callback)
    {
        CheckFeatureArgs(handle, name);
        ArgumentNullException.ThrowIfNull(callback, nameof(callback));

        DetectError(VmbFeatureInvalidationUnregister(handle, name, callback));

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
        EntryPoint = nameof(VmbFeatureInvalidationUnregister), ExactSpelling = true, SetLastError = false)]
        static extern unsafe ErrorType VmbFeatureInvalidationUnregister(VmbHandle handle,
                                                                      byte* name,
                                                                      delegate* unmanaged<VmbHandle, byte*, void*, void> callback);
    }
    #endregion End – Feature Invalidation UnRegister

    #region Register/UnRegister Device Temperature Callback

    public static unsafe void RegisterDeviceTemperatureCallback([NotNull, DisallowNull] VmbHandle handle,
                                                                [NotNull, DisallowNull] delegate* unmanaged<VmbHandle, byte*, void*, void> callback,
                                                                void* userContext = null) =>
        FeatureInvalidationRegister(handle, "DeviceTemperature"u8, callback, userContext);

    public static unsafe void UnRegisterDeviceTemperatureCallback([NotNull, DisallowNull] VmbHandle handle,
                                                                  [NotNull, DisallowNull] delegate* unmanaged<VmbHandle, byte*, void*, void> callback) =>
        FeatureInvalidationUnRegister(handle, "DeviceTemperature"u8, callback);
    #endregion  End – Register/UnRegister Device Temperature Callback

    #region Feature Gets

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void FeatureStringGet([NotNull, DisallowNull] VmbHandle handle,
                                      [NotNull, DisallowNull] byte* name,
                                      [AllowNull] byte* buffer,
                                      uint bufferSize,
                                      [NotNull, DisallowNull] uint* sizeFilled)
    {
        CheckFeatureArgs(handle, name);

        DetectError(VmbFeatureStringGet(handle!, name!, buffer, bufferSize, sizeFilled));

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
        EntryPoint = nameof(VmbFeatureStringGet), ExactSpelling = true, SetLastError = false)]
        static extern unsafe ErrorType VmbFeatureStringGet([NotNull, DisallowNull] VmbHandle handle,
                                      [NotNull, DisallowNull] byte* name,
                                      [AllowNull] byte* buffer,
                                      uint bufferSize,
                                      [NotNull, DisallowNull] uint* sizeFilled);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe string FeatureStringGet([NotNull, DisallowNull] VmbHandle handle,
                                                [NotNull, DisallowNull] ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
            return FeatureStringGet(handle, pName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe string FeatureStringGet([NotNull, DisallowNull] VmbHandle handle,
                                                 [NotNull, DisallowNull] byte* name)
    {
        Unsafe.SkipInit(out uint sizeFilled);
        FeatureStringGet(handle, name, null, 0u, &sizeFilled);

        if (sizeFilled <= 2048u)
        {
            byte* buffer = stackalloc byte[(int)sizeFilled];
            FeatureStringGet(handle, name, buffer, sizeFilled, &sizeFilled);
            return new((sbyte*)buffer);
        }
        if (sizeFilled > Array.MaxLength)
            ThrowOversizedStrigBuffer();

        fixed (byte* buffer = GC.AllocateUninitializedArray<byte>((int)sizeFilled, false))
        {
            FeatureStringGet(handle, name, buffer, sizeFilled, &sizeFilled);
            return new((sbyte*)buffer);
        }

        [DoesNotReturn]
        static void ThrowOversizedStrigBuffer() =>
        throw new OutOfMemoryException("String length out of range. Impossible to allocate.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void FeatureFloatGet([NotNull, DisallowNull] VmbHandle handle,
                                              [NotNull, DisallowNull] byte* name,
                                              [NotNull, DisallowNull] double* value)
    {
        CheckFeatureArgs(handle, name);
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        DetectError(VmbFeatureFloatGet(handle!, name!, value!));

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
        EntryPoint = nameof(VmbFeatureFloatGet), ExactSpelling = true, SetLastError = false)]
        static extern unsafe ErrorType VmbFeatureFloatGet(VmbHandle handle, byte* name, double* value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe double FeatureFloatGet([NotNull, DisallowNull] VmbHandle handle,
                                                [NotNull, DisallowNull] ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
            return FeatureFloatGet(handle, pName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe double FeatureFloatGet([NotNull, DisallowNull] VmbHandle handle,
                                                [NotNull, DisallowNull] byte* name)
    {
        double value;
        FeatureFloatGet(handle, name, &value);
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void FeatureIntGet([NotNull, DisallowNull] VmbHandle handle,
                                      [NotNull, DisallowNull] byte* name,
                                      [NotNull, DisallowNull] long* value)
    {
        CheckFeatureArgs(handle, name);
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        DetectError(VmbFeatureIntGet(handle!, name!, value!));

        [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
        EntryPoint = nameof(VmbFeatureIntGet), ExactSpelling = true, SetLastError = false)]
        static extern unsafe ErrorType VmbFeatureIntGet(VmbHandle handle, byte* name, long* value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe long FeatureIntGet([NotNull, DisallowNull] VmbHandle handle,
                                                [NotNull, DisallowNull] ReadOnlySpan<byte> name)
    {
        fixed (byte* pName = name)
            return FeatureIntGet(handle, pName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
    public static unsafe long FeatureIntGet([NotNull, DisallowNull] VmbHandle handle,
                                            [NotNull, DisallowNull] byte* name)
    {
        long value;
        FeatureIntGet(handle, name, &value);
        return value;
    }
    #endregion  End – Feature Gets

    #region External triggering
    [SkipLocalsInit]
    public static unsafe void ActivateExternalTriggeringOnLine0(nuint handle)
    {
        FeatureEnumSet(handle, "TriggerSource"u8, "Line0"u8);
        FeatureEnumSet(handle!, "TriggerMode"u8, "On"u8); // After the first call it can't be null.
        FeatureEnumSet(handle!, "TriggerActivation"u8, "RisingEdge"u8);
        FeatureFloatSet(handle!, "TriggerDelay"u8, 0d);
    }
    [SkipLocalsInit]
    public static unsafe void ActivateExternalTriggeringOnLine1(nuint handle)
    {
        FeatureEnumSet(handle, "TriggerSource"u8, "Line1"u8);
        FeatureEnumSet(handle!, "TriggerMode"u8, "On"u8); // After the first call it can't be null.
        FeatureEnumSet(handle!, "TriggerActivation"u8, "RisingEdge"u8);
        FeatureFloatSet(handle!, "TriggerDelay"u8, 0d);
    }
    #endregion End – External triggering
}