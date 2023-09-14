using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueItemProcessor
{
    public class ProcessedMessage
    {

        public ProcessedMessage(string message, bool hasErrors = false)
        {
            TimeStamp = DateTime.Now;
            Message = message;
            HasErrors = hasErrors;

        }

        public DateTime TimeStamp { get; set; }
        public string Message { get; set; } 
        public bool HasErrors { get; set; } 

    }
}
