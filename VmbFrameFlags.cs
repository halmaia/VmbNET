namespace VmbNET;

[Flags]
public enum VmbFrameFlags : uint
{
    VmbFrameFlagsNone = 0,        //!< No additional information is provided
    VmbFrameFlagsDimension = 1,        //!< VmbFrame_t::width and VmbFrame_t::height are provided
    VmbFrameFlagsOffset = 2,        //!< VmbFrame_t::offsetX and VmbFrame_t::offsetY are provided (ROI)
    VmbFrameFlagsFrameID = 4,        //!< VmbFrame_t::frameID is provided
    VmbFrameFlagsTimestamp = 8,        //!< VmbFrame_t::timestamp is provided
    VmbFrameFlagsImageData = 16,       //!< VmbFrame_t::imageData is provided
    VmbFrameFlagsPayloadType = 32,       //!< VmbFrame_t::payloadType is provided
    VmbFrameFlagsChunkDataPresent = 64,       //!< VmbFrame_t::chunkDataPresent is set based on info provided by the transport layer
}