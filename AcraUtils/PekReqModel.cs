
namespace AcraUtils
{
    public class PekReqModel
    {
        public PEK_ServiceReference.Response responseModel { get; set; }
        public string requestModel { get; set; }
        public bool isTinModel { get; set; }
        public long userActivityId { get; set; }
        public int SourceID { get; set; }
    }
}
