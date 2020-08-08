namespace Algo.VolumeProfileBuilder
{
    public interface IVolumeProfileReader
    {
        VolumeProfile Read(string csvFile = null);
    }
}