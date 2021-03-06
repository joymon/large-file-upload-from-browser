﻿using System;
using System.Collections.Generic;

namespace PrDCOldApp.Web.Models
{
    public class ImageEntry
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Email { get; set; }
        public DateTime? Published { get; set; }
        public string Location { get; set; }
        public Tuple<double, double> Coordinates { get; set; }
        public int Likes { get; set; }
        public ICollection<Comment> Comments { get; set; }

        public ImageEntry()
        {
            Comments = new List<Comment>();
        }
    }
}