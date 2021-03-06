﻿using System.ComponentModel.DataAnnotations;

namespace SessionTracker.Web.RequestModels
{
    public class SpeakerRequestModel
    {
        [Required]
        [StringLength(25)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(25)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(250)]
        public string Email { get; set; }
    }
}