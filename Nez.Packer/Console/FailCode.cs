namespace Nez.Tools.Packing
{
    public enum FailCode
    {
        FailedParsingArguments = 1,
        FailedParsingConfig,
        NoImages,
        ImageNameCollision,
        FailedToLoadImage,
        FailedToPackImage,
        FailedToSaveImage,
        ImageSizeMismatch
    }
}