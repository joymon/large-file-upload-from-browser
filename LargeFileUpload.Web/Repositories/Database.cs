using PrDCOldApp.Web.Controllers;
using PrDCOldApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrDCOldApp.Web
{
    public class Database
    {
        internal  void SaveUploadEntryInDatabase(UploadEntry entry)
        {
            var imageEntry = new ImageEntry()
            {
                Path = entry.UploadUrl,
                Email = entry.EmailAddress,
                Published = entry.DatePublished,
                Location = entry.Location,
                Likes = 1
            };

            //Code for saving the file to database.
        }
    }
}