using System.Runtime.CompilerServices;

namespace VmbNET
{
    internal static class VmbUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe string PtrToStr(byte* ptr) => new((sbyte*)ptr);
    }
}
