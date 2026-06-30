using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckUpService.Models
{
    public class Response: IResponse
    {

        private string responseIDField;

        private long responseTimeField;

        private int errorCodeField;

        private string errorDescField;

        private dynamic responseMessageField;

        /// <remarks/>
        public string ResponseID
        {
            get
            {
                return this.responseIDField;
            }
            set
            {
                this.responseIDField = value;
            }
        }

        /// <remarks/>
        public long ResponseTime
        {
            get
            {
                return this.responseTimeField;
            }
            set
            {
                this.responseTimeField = value;
            }
        }

        /// <remarks/>
        public dynamic ResponseMessage
        {
            get
            {
                return this.responseMessageField;
            }
            set
            {
                this.responseMessageField = value;
            }
        }

        /// <remarks/>
        public int ErrorCode
        {
            get
            {
                return this.errorCodeField;
            }
            set
            {
                this.errorCodeField = value;
            }
        }

        /// <remarks/>
        public string ErrorDesc
        {
            get
            {
                return this.errorDescField;
            }
            set
            {
                this.errorDescField = value;
            }
        }
    }
}
