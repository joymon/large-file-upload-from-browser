using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace PrDCOldApp.Web
{
    public class ThumbnailCreator
    {
        private void SaveThumbs(string path)
        {
            var original = System.Drawing.Image.FromFile(path);
            var thumbnail = ScaleImage(original, 400, 400);
            var thumbnailPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + "_t.png";
            thumbnail.Save(thumbnailPath, ImageFormat.Png);
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            var g = Graphics.FromImage(newImage);
            //.DrawImage(image, 0, 0, newWidth, newHeight);
            ColorMatrix colorMatrix = new ColorMatrix(
              new float[][]
              {
                 new float[] {.3f, .3f, .3f, 0, 0},
                 new float[] {.59f, .59f, .59f, 0, 0},
                 new float[] {.11f, .11f, .11f, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0, 0, 0, 0, 1}
              });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight),
               0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newImage;
        }
    }
}