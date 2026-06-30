namespace AcraUtils.Services
{
    public interface IAcraService
    {
        bool IsStarted { get; }

        void Start();
        void Stop();
        void Wait();
        void Wait(int to = 0);
    }
}
