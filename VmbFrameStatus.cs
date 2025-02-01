namespace VmbNET;
public enum VmbFrameStatus : int
{
    VmbFrameStatusComplete = 0,       //!< Frame has been completed without errors
    VmbFrameStatusIncomplete = -1,       //!< Frame could not be filled to the end
    VmbFrameStatusTooSmall = -2,       //!< Frame buffer was too small
    VmbFrameStatusInvalid = -3,       //!< Frame buffer was invalid
}