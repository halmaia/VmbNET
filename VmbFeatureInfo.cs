using System.Runtime.InteropServices;
using static VmbNET.VmbUtils;

namespace VmbNET;

[StructLayout(LayoutKind.Sequential, Size = Size)]
public readonly unsafe struct VmbFeatureInfo
{
    public const int Size = 88;

    public readonly byte* Name { get; }
    public readonly byte* Category { get; }
    public readonly byte* DisplayName { get; }
    public readonly byte* Tooltip { get; }
    public readonly byte* Description { get; }
    public readonly byte* SFNCNamespace { get; }    
    public readonly byte* Unit { get; }
    public readonly byte* Representation { get; }
    public readonly VmbFeatureData FeatureDataType { get; }
    public readonly VmbFeatureFlags FeatureFlags { get; }
    public readonly uint PollingTime { get; }
    public readonly VmbFeatureVisibility Visibility { get; }
    public readonly bool IsStreamable { get; }
    public readonly bool HasSelectedFeatures { get; } 

    public readonly string NameStr => PtrToStr(Name);
    public readonly string CategoryStr => PtrToStr(Category);
    public readonly string DisplayNameStr => PtrToStr(DisplayName);
    public readonly string TooltipStr => PtrToStr(Tooltip);
    public readonly string DescriptionStr => PtrToStr(Description);
    public readonly string SFNCNamespaceStr => PtrToStr(SFNCNamespace);
    public readonly string UnitStr => PtrToStr(Unit);
    public readonly string RepresentationStr => PtrToStr(Representation);
}