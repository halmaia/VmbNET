using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VmbNET;

[StructLayout(LayoutKind.Sequential, Size = Size)]
[DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
public readonly struct VmbVersionInfo : IEquatable<VmbVersionInfo>, IComparable<VmbVersionInfo>
{
    public const int Size = 3 * sizeof(uint);
    public readonly uint Major { get; }
    public readonly uint Minor { get; }
    public readonly uint Patch { get; }

    public override readonly bool Equals(object? obj) => obj is VmbVersionInfo info && Equals(info);

    public readonly bool Equals(VmbVersionInfo other) =>
        Major == other.Major &&
               Minor == other.Minor &&
               Patch == other.Patch;

    public override readonly string ToString() => $"{Major}.{Minor}.{Patch}";

    public static explicit operator Version(VmbVersionInfo vmbVersion) => new((int)vmbVersion.Major, (int)vmbVersion.Minor, (int)vmbVersion.Patch);

    public override readonly int GetHashCode()
    {
        // See https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/Version.cs,b7e7c7df371e4f00,references
        // for the original idea.
        uint accumulator = (Major & 0x0000000F) << 28;
        accumulator |= (Minor & 0x000000FF) << 20;
        accumulator |= (Patch & 0x000000FF) << 12;
        return (int)accumulator;
    }

    public readonly int CompareTo(VmbVersionInfo value) =>
        Major != value.Major ? (Major > value.Major ? 1 : -1) :
            Minor != value.Minor ? (Minor > value.Minor ? 1 : -1) :
            Patch != value.Patch ? (Patch > value.Patch ? 1 : -1) :
            0; // Everything is equal.


    public static bool operator ==(VmbVersionInfo left, VmbVersionInfo right) => left.Equals(right);

    public static bool operator !=(VmbVersionInfo left, VmbVersionInfo right) => !(left == right);

    public static bool operator <(VmbVersionInfo left, VmbVersionInfo right) => left.CompareTo(right) < 0;

    public static bool operator <=(VmbVersionInfo left, VmbVersionInfo right) => left.CompareTo(right) <= 0;

    public static bool operator >(VmbVersionInfo left, VmbVersionInfo right) => left.CompareTo(right) > 0;

    public static bool operator >=(VmbVersionInfo left, VmbVersionInfo right) => left.CompareTo(right) >= 0;
}