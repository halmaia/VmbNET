using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
            DetectError(VmbStartup(pathConfiguration));

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
        public static bool IsAPIUpAndRunning(out string errorMessage)
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

            errorMessage = string.Empty;
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
        public static unsafe void VersionQuery(VmbVersionInfo* versionInfo)
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
        public static void CameraClose(VmbHandle cameraHandle)
        {
            unsafe { ArgumentNullException.ThrowIfNull((void*)cameraHandle, nameof(cameraHandle)); }
            DetectError(VmbCameraClose(cameraHandle!));

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
             EntryPoint = nameof(VmbCameraClose), ExactSpelling = true, PreserveSig = true, SetLastError = false)]
            static extern ErrorType VmbCameraClose(VmbHandle cameraHandle);
        }
        #endregion End – Close Camera

        #region List Cameras
        [return: MaybeNull]
        public static unsafe VmbCameraInfo[]? CamerasList(bool firstOnly = true)
        {
            uint NumFound;
            uint* pNumFound = &NumFound;
            DetectError(VmbCamerasList(null, 0u, pNumFound, 0u));

            if (NumFound == 0) return null;
            if (firstOnly) NumFound = 1u;

            VmbCameraInfo[] cameraInfo = GC.AllocateUninitializedArray<VmbCameraInfo>((int)NumFound, false);

            fixed (VmbCameraInfo* pCameraInfo = cameraInfo)
                DetectError(VmbCamerasList(pCameraInfo, NumFound, pNumFound, VmbCameraInfo.Size * NumFound));

            return cameraInfo;

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
             EntryPoint = nameof(VmbCamerasList), ExactSpelling = true, SetLastError = false)]
            static unsafe extern ErrorType VmbCamerasList(VmbCameraInfo* pCameraInfo, uint listLength,
                                                          uint* pNumFound,
                                                          uint sizeofCameraInfo);
        }

        [return: MaybeNull]
        public static VmbCameraInfo? GetFirstCamera() => CamerasList(true)?[0]; // Null propagation.
        #endregion End – List Cameras

        #region Open Cameras
        [SkipLocalsInit]
        public static unsafe VmbHandle CameraOpen(byte* idString,
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
        public static unsafe void FrameAnnounce(VmbHandle cameraHandle,
                                                 VmbFrame* pFrame)
        {
            DetectError(VmbFrameAnnounce(cameraHandle, pFrame));

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
             EntryPoint = nameof(VmbFrameAnnounce), ExactSpelling = true, SetLastError = false)]
            static unsafe extern ErrorType VmbFrameAnnounce(
                                      VmbHandle cameraHandle,
                                      VmbFrame* pFrame,
                                      [ConstantExpected(Max = VmbFrame.Size, Min = VmbFrame.Size)]
                                       uint sizeofFrame = VmbFrame.Size);
        }

        public static VmbFrame CreateFrameAndAnnounce(VmbHandle cameraHandle, uint payloadSize)
        {
            VmbFrame frame = new()
            {
                bufferSize = payloadSize
            };
            unsafe { FrameAnnounce(cameraHandle, &frame); }
            return frame;
        }

        #endregion End – Frame Announce

        #region Get Payload Size
        public static unsafe uint PayloadSizeGet(VmbHandle handle)
        {
            ArgumentNullException.ThrowIfNull((void*)handle);
            
            uint payloadSize;
            DetectError(VmbPayloadSizeGet(handle!, &payloadSize));
            return payloadSize;

            [DllImport(dllName, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
            EntryPoint = nameof(VmbPayloadSizeGet), ExactSpelling = true, SetLastError = false)]
            unsafe static extern ErrorType VmbPayloadSizeGet(VmbHandle handle, uint* payloadSize);
        }
        #endregion End – Get Payload Size
    }
}