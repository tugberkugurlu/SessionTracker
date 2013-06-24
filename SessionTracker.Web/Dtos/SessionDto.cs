using System.Collections.Generic;

namespace SessionTracker.Web.Dtos {
    
    public class SessionDto {

        public int Id { get; set; }
        public int SpeakerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte DurationInMinutes { get; set; }
        public ICollection<string> Tags { get; set; }

        public SpeakerDto Speaker { get; set; }
    }
}