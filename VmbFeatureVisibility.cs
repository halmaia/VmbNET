namespace VmbNET
{
    public enum VmbFeatureVisibility : uint
    {
        VmbFeatureVisibilityUnknown = 0,        //!< Feature visibility is not known
        VmbFeatureVisibilityBeginner = 1,        //!< Feature is visible in feature list (beginner level)
        VmbFeatureVisibilityExpert = 2,        //!< Feature is visible in feature list (expert level)
        VmbFeatureVisibilityGuru = 3,        //!< Feature is visible in feature list (guru level)
        VmbFeatureVisibilityInvisible = 4,        //!< Feature is visible in the feature list, but should be hidden in GUI applications
    }

}