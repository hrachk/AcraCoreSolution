namespace AcraUtils.Configuration
{
    public class CBRegistrConverter
    {
        public int QueryCount { get; set; }
        public int QueueLength { get; set; }
        public int ThreadsCount { get; set; }
    }

    public class CBA001Receiver
    {
        public string CBA001Queue { get; set; }
    }
}
