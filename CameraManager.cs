using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace VmbNET
{
    public static class CameraManager
    {
        #region Private Constants
        private const string dllName = @"C:\Program Files\Allied Vision\Vimba X\api\bin\VmbC.dll";
        #endregion

        #region Error Handling
        [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
        private static void DetectError(ErrorType errorType)
        {
            if (errorType is not ErrorType.VmbErrorSuccess)
                ThrowAPIError(errorType);

            [DoesNotReturn]
            static void ThrowAPIError(ErrorType errorType) => throw new Exception(errorType switch
            {
                ErrorType.VmbErrorInternalFault => "Internal fault.",
                ErrorType.VmbErrorSuccess => "No error."
            });
        }
        #endregion
    }
}
