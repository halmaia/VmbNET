namespace VmbNET;

public enum VmbPayloadType : uint
{
    VmbPayloadTypeUnknown = 0,        //!< Unknown payload type
    VmbPayloadTypeImage = 1,        //!< image data
    VmbPayloadTypeRaw = 2,        //!< raw data
    VmbPayloadTypeFile = 3,        //!< file data
    VmbPayloadTypeJPEG = 5,        //!< JPEG data as described in the GigEVision 2.0 specification
    VmbPayloadTypJPEG2000 = 6,        //!< JPEG 2000 data as described in the GigEVision 2.0 specification
    VmbPayloadTypeH264 = 7,        //!< H.264 data as described in the GigEVision 2.0 specification
    VmbPayloadTypeChunkOnly = 8,        //!< Chunk data exclusively
    VmbPayloadTypeDeviceSpecific = 9,        //!< Device specific data format
    VmbPayloadTypeGenDC = 11,       //!< GenDC data
}