using System;

namespace SessionTracker.Web.Dtos {

    public class SpeakerDto {

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GravatarHash { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}