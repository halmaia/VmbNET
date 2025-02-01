namespace VmbNET;

[Flags]
public enum VmbAccessMode : uint
{
    /// <summary>
    ///  No access.
    /// </summary>
    VmbAccessModeNone = 0,
    /// <summary>
    /// Read and write access.
    /// </summary>
    VmbAccessModeFull = 1,
    /// <summary>
    /// Read-only access.
    /// </summary>
    VmbAccessModeRead = 2,   
    /// <summary>
    /// Access type unknown.
    /// </summary>
    VmbAccessModeUnknown = 4,
    /// <summary>
    /// Read and write access without permitting access for other consumers.
    /// </summary>
    VmbAccessModeExclusive = 8
}