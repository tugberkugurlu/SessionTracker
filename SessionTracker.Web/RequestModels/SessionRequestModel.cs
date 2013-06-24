using System.Collections.Generic;

namespace SessionTracker.Web.RequestModels {

    public class SessionRequestModel {

        public int SpeakerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte DurationInMinutes { get; set; }
        public ICollection<string> Tags { get; set; }
    }
}