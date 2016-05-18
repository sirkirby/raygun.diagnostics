using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindscape.Raygun4Net;
using Mindscape.Raygun4Net.Messages;

namespace Raygun.Diagnostics.Tests.Models
{
    class MockRaygunClient : RaygunClient
    {

        public override void Send(RaygunMessage raygunMessage)
        {
            //Fire the event to tell the library to propagate the event down the subscription change
            //OnCustomGroupingKey(new Exception(raygunMessage.ToString()), raygunMessage);            
        }

        public override void Send(Exception exception)
        {
            //do nothing here so we don't actually send the message
        }

    }
}
