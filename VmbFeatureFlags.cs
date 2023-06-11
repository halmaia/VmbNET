namespace VmbNET
{
    [Flags]
    public enum VmbFeatureFlags:uint
    {
        VmbFeatureFlagsNone = 0,        //!< No additional information is provided
        VmbFeatureFlagsRead = 1,        //!< Static info about read access. Current status depends on access mode, check with ::VmbFeatureAccessQuery()
        VmbFeatureFlagsWrite = 2,        //!< Static info about write access. Current status depends on access mode, check with ::VmbFeatureAccessQuery()
        VmbFeatureFlagsVolatile = 8,        //!< Value may change at any time
        VmbFeatureFlagsModifyWrite = 16,       //!< Value may change after a write
    }

}