namespace Algo.VolumeProfileBuilder.Internals
{
    internal interface IVolumeProfileWriter
    {
        void Write(VolumeProfile volumeProfile, string outputFile = null);
    }
}