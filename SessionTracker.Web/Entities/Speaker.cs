using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SessionTracker.Web.Entities 
{    
    public class Speaker {

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastUpdatedOn { get; set; }
    }
}