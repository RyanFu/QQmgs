﻿using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Twitter.App.Models.BindingModels
{
    public class CreateActivityBindingModel
    {
        [Required]
        [MinLength(1)]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Required]
        [JsonProperty("description")]
        public string Description { get; set; }

        [Required]
        [JsonProperty("place")]
        public string Place { get; set; }

        [Required]
        [JsonProperty("classfication")]
        public string Classfication { get; set; }

        [Required]
        [JsonProperty("start_time")]
        public string StartTime { get; set; }

        [Required]
        [JsonProperty("end_time")]
        public string EndTime { get; set; }

        [JsonProperty("organizer")]
        public string Organizer { get; set; }

        [JsonProperty("is_display")]
        public bool IsDisplay { get; set; }
    }
}