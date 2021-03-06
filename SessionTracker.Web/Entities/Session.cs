﻿using Raven.Imports.Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SessionTracker.Web.Entities
{
    public class Session
    {
        public string Id { get; set; }
        public string SpeakerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte DurationInMinutes { get; set; }
        public ICollection<string> Tags { get; set; }
        public DateTimeOffset? SessionDate { get; set; }
        public string Room { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset LastUpdatedOn { get; set; }

        [JsonIgnore]
        public bool IsScheduled
        {
            get { return SessionDate.HasValue; }
        }
    }
}