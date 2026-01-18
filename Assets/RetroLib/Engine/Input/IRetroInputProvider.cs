namespace RetroLib.Engine.Input
{
    public interface IRetroInputProvider
    {
        RetroInputState State { get; }

        void Poll();
    }
}
