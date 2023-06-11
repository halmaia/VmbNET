namespace VmbNET
{
    public enum VmbFeatureData : uint
    {
        VmbFeatureDataUnknown = 0,        //!< Unknown feature type
        VmbFeatureDataInt = 1,        //!< 64-bit integer feature
        VmbFeatureDataFloat = 2,        //!< 64-bit floating point feature
        VmbFeatureDataEnum = 3,        //!< Enumeration feature
        VmbFeatureDataString = 4,        //!< String feature
        VmbFeatureDataBool = 5,        //!< Boolean feature
        VmbFeatureDataCommand = 6,        //!< Command feature
        VmbFeatureDataRaw = 7,        //!< Raw (direct register access) feature
        VmbFeatureDataNone = 8,        //!< Feature with no data
    }

}