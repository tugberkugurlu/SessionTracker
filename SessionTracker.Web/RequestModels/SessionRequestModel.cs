using System.ComponentModel.DataAnnotations;

namespace SessionTracker.Web.RequestModels {

    public class SessionRequestModel {

        public int SpeakerId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Range(30, 75)]
        public byte DurationInMinutes { get; set; }

        [MinLength(1)]
        [MaxLength(6)]
        public string[] Tags { get; set; }
    }
}