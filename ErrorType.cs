namespace VmbNET;

public enum ErrorType : int
{
    VmbErrorSuccess = 0,           //!< No error
    VmbErrorInternalFault = -1,           //!< Unexpected fault in VmbC or driver
    VmbErrorApiNotStarted = -2,           //!< ::VmbStartup() was not called before the current command
    VmbErrorNotFound = -3,           //!< The designated instance (camera, feature etc.) cannot be found
    VmbErrorBadHandle = -4,           //!< The given handle is not valid
    VmbErrorDeviceNotOpen = -5,           //!< Device was not opened for usage
    VmbErrorInvalidAccess = -6,           //!< Operation is invalid with the current access mode
    VmbErrorBadParameter = -7,           //!< One of the parameters is invalid (usually an illegal pointer)
    VmbErrorStructSize = -8,           //!< The given struct size is not valid for this version of the API
    VmbErrorMoreData = -9,           //!< More data available in a string/list than space is provided
    VmbErrorWrongType = -10,          //!< Wrong feature type for this access function
    VmbErrorInvalidValue = -11,          //!< The value is not valid; either out of bounds or not an increment of the minimum
    VmbErrorTimeout = -12,          //!< Timeout during wait
    VmbErrorOther = -13,          //!< Other error
    VmbErrorResources = -14,          //!< Resources not available (e.g. memory)
    VmbErrorInvalidCall = -15,          //!< Call is invalid in the current context (e.g. callback)
    VmbErrorNoTL = -16,          //!< No transport layers are found
    VmbErrorNotImplemented = -17,          //!< API feature is not implemented
    VmbErrorNotSupported = -18,          //!< API feature is not supported
    VmbErrorIncomplete = -19,          //!< The current operation was not completed (e.g. a multiple registers read or write)
    VmbErrorIO = -20,          //!< Low level IO error in transport layer
    VmbErrorValidValueSetNotPresent = -21,          //!< The valid value set could not be retrieved, since the feature does not provide this property
    VmbErrorGenTLUnspecified = -22,          //!< Unspecified GenTL runtime error
    VmbErrorUnspecified = -23,          //!< Unspecified runtime error
    VmbErrorBusy = -24,          //!< The responsible module/entity is busy executing actions
    VmbErrorNoData = -25,          //!< The function has no data to work on
    VmbErrorParsingChunkData = -26,          //!< An error occurred parsing a buffer containing chunk data
    VmbErrorInUse = -27,          //!< Something is already in use
    VmbErrorUnknown = -28,          //!< Error condition unknown
    VmbErrorXml = -29,          //!< Error parsing XML
    VmbErrorNotAvailable = -30,          //!< Something is not available
    VmbErrorNotInitialized = -31,          //!< Something is not initialized
    VmbErrorInvalidAddress = -32,          //!< The given address is out of range or invalid for internal reasons
    VmbErrorAlready = -33,          //!< Something has already been done
    VmbErrorNoChunkData = -34,          //!< A frame expected to contain chunk data does not contain chunk data
    VmbErrorUserCallbackException = -35,          //!< A callback provided by the user threw an exception
    VmbErrorFeaturesUnavailable = -36,          //!< The XML for the module is currently not loaded; the module could be in the wrong state or the XML could not be retrieved or could not be parsed properly
    VmbErrorTLNotFound = -37,          //!< A required transport layer could not be found or loaded
    VmbErrorAmbiguous = -39,          //!< An entity cannot be uniquely identified based on the information provided
    VmbErrorRetriesExceeded = -40,          //!< Something could not be accomplished with a given number of retries
    VmbErrorInsufficientBufferCount = -41,          //!< The operation requires more buffers
    VmbErrorCustom = 1,            //!< The minimum error code to use for user defined error codes to avoid conflict with existing error codes
}
