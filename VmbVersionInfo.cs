using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VmbNET
{
    [StructLayout(LayoutKind.Sequential, Size = Size)]
    [DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
    public readonly struct VmbVersionInfo
    {
        public const int Size = 3 * sizeof(uint);
        public readonly uint Major { get; }
        public readonly uint Minor { get; }
        public readonly uint Patch { get; }
        public override readonly string ToString() => $"{Major}.{Minor}.{Patch}";
        public static explicit operator Version(VmbVersionInfo vmbVersion) => new((int)vmbVersion.Major, (int)vmbVersion.Minor, (int)vmbVersion.Patch);
    }
}