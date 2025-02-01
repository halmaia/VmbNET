using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using VmbHandle = nuint;
using static VmbNET.VmbUtils;

namespace VmbNET;

[StructLayout(LayoutKind.Sequential, Size = Size)]
[SkipLocalsInit]
[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public readonly unsafe struct VmbCameraInfo
{
    public const int Size = 80;

    public readonly byte* CameraIdString { get; }
    public readonly byte* CameraIdExtended { get; }
    public readonly byte* CameraName { get; }
    public readonly byte* ModelName { get; }
    public readonly byte* SerialString { get; }

    public readonly VmbHandle TransportLayerHandle { get; }
    public readonly VmbHandle InterfaceHandle { get; }
    public readonly VmbHandle LocalDeviceHandle { get; }

    public readonly VmbHandle* StreamHandles { get; }

    public readonly uint StreamCount { get; }

    private readonly VmbAccessMode PermittedAccess { get; }

    public ReadOnlySpan<VmbHandle> Streams => new(StreamHandles, (int)StreamCount);


    public readonly string CameraIdStr => PtrToStr(CameraIdString);
    public readonly string CameraIdExtendedStr => PtrToStr(CameraIdExtended);
    public readonly string CameraNameStr => PtrToStr(CameraName);
    public readonly string ModelNameStr => PtrToStr(ModelName);
    public readonly string SerialStr => PtrToStr(SerialString);

    public readonly override string ToString() =>
        string.Concat(CameraIdStr, " (", CameraNameStr + ")");
}
