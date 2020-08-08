namespace Algo.VolumeProfileBuilder
{
    public interface IVolumeProfileBuilder
    {
        void Tick(decimal volume);
        VolumeProfile Build(bool persist = false);
    }
}