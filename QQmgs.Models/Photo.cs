﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitter.Models
{
    public class Photo
    {
        public Photo()
        {
            this.UsersFavourite = new HashSet<User>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public DateTime DatePosted { get; set; }

        [Required]
        public string AuthorId { get; set; }

        public string Descrption { get; set; }

        public virtual User Author { get; set; }

        public PhotoType PhotoType { get; set; }

        public PhotoClasscification PhotoClasscification { get; set; }

        public bool IsSoftDelete { get; set; }

        public int OriginalWidth { get; set; }

        public int OriginalHeight { get; set; }

        public virtual ICollection<User> UsersFavourite { get; set; }
    }

    public enum PhotoType
    {
        Photo,
        AvatarImage,
        GroupImage
    }

    public enum PhotoClasscification
    {
        其他 = 0,
        风景 = 1,
        美食 = 2,
        人像 = 3,
        艺术 = 4
    }
}
