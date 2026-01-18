using RetroLib.Engine.Core;
using RetroLib.LibretroHost.Core;
using RetroLib.LibretroHost.State;

namespace RetroLib.Engine.Core
{
    public class RetroSession
    {
        public IRetroCore Core { get; }

        private readonly LibretroCoreState state;

        public RetroSession(IRetroCore core)
        {
            Core = core;

            // 🔑 Estado nasce junto com a sessão
            state = new LibretroCoreState();

            // 🔑 Core SEMPRE recebe o state antes de qualquer coisa
            Core.SetState(state);
        }

        public bool IsRunning => Core.IsRunning;

        public void LoadCore(string corePath)
        {
            Core.LoadCore(corePath);
        }

        public void LoadRom(string romPath)
        {
            Core.LoadRom(romPath);
        }

        public void Start()
        {
            Core.StartEmulation();
        }

        public void Stop()
        {
            Core.StopEmulation();
        }

        public void RunFrame()
        {
            Core.RunFrame();
        }
    }
}
