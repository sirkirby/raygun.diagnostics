using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raygun.Diagnostics.Models
{
    public class MessageGroup : IMessageGroup
    {
        public string GroupKey { get; set; }
        public MessageGroup()
        {
            GroupKey = string.Empty;
        }
    }
}
