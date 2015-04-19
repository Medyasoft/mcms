using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace Unigate.PhotoGallery.AdminPlugin.Manager
{
    public static class PhotoManager
    {
        /// <summary>
        /// Binary olarak verilen fotoğrafı kaydeder.
        /// </summary>
        /// <param name="imageBinaryArray">Fotoğrafın binary datası</param>
        /// <param name="imageid">Fotoğrafın Id'si</param>
        /// <param name="path">Fotoğrafın kaydediğiceği Full Path</param>
        /// <returns>İşlem durumu</returns>
        public static bool SavePhoto(byte[] imageBinaryArray, string path)
        {
            bool retval = false;

            if (imageBinaryArray != null && imageBinaryArray.Length > 0)
            {
                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                using (FileStream file = new FileStream(path, FileMode.Create, System.IO.FileAccess.Write))
                {
                    try
                    {
                        file.Write(imageBinaryArray, 0, imageBinaryArray.Length);

                        retval = true;
                    }
                    catch
                    {
                        retval = false;
                    }
                    finally
                    {
                        file.Close();
                    }
                }
            }

            return retval;
        }

        /// <summary>
        /// Binary olarak verilen fotoğrafı belirtilen boyutlara göre resize eder.
        /// </summary>
        /// <param name="imageBinaryArray">Fotoğrafın binary datası</param>
        /// <param name="width">Resize edilecek Genişlik</param>
        /// <param name="height">Resize edilecek Yükseklik</param>
        /// <returns>Resize edilmiş fotoğrafın binary datası</returns>
        public static byte[] ImageResize(byte[] imageBinaryArray, int newWidth, int newHeight, bool iscover)
        {
            byte[] imageout = null;

            using (Stream imageStream = new MemoryStream())
            {
                imageStream.Write(imageBinaryArray, 0, Convert.ToInt32(imageBinaryArray.Length));

                using (System.Drawing.Image imgPhoto = Bitmap.FromStream(imageStream))
                {
                    int sourceWidth = imgPhoto.Width;
                    int sourceHeight = imgPhoto.Height;

                    //Consider vertical pics
                    if (sourceWidth < sourceHeight)
                    {
                        int buff = newWidth;

                        newWidth = newHeight;
                        newHeight = buff;
                    }

                    int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
                    float nPercent = 0, nPercentW = 0, nPercentH = 0;

                    nPercentW = ((float)newWidth / (float)sourceWidth);
                    nPercentH = ((float)newHeight / (float)sourceHeight);
                    if (nPercentH < nPercentW)
                    {
                        nPercent = nPercentH;
                        destX = System.Convert.ToInt16((newWidth - (sourceWidth * nPercent)) / 2);
                    }
                    else
                    {
                        nPercent = nPercentW;
                        destY = System.Convert.ToInt16((newHeight - (sourceHeight * nPercent)) / 2);
                    }

                    int destWidth = (int)(sourceWidth * nPercent);
                    int destHeight = (int)(sourceHeight * nPercent);

                    using (Bitmap bmPhoto = new Bitmap((iscover ? newWidth : destWidth), (iscover ? newHeight : destHeight), PixelFormat.Format24bppRgb))
                    {
                        bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

                        using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
                        {
                            grPhoto.Clear(Color.White);
                            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                            grPhoto.DrawImage(imgPhoto, new Rectangle((iscover ? destX : sourceX), (iscover ? destY : sourceY), destWidth, destHeight), new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), GraphicsUnit.Pixel);
                        }

                        using (MemoryStream sout = new MemoryStream())
                        {
                            bmPhoto.Save(sout, imgPhoto.RawFormat);
                            imageout = sout.ToArray();
                        }

                    }
                }
            }

            return imageout;
        }

        public static Size GetImageSize(byte[] imageBinaryArray)
        {
            Size retval = new Size();
            using (Stream imageStream = new MemoryStream())
            {
                imageStream.Write(imageBinaryArray, 0, Convert.ToInt32(imageBinaryArray.Length));

                using (System.Drawing.Image imgPhoto = Bitmap.FromStream(imageStream))
                {
                    retval.Width = imgPhoto.Width;
                    retval.Height = imgPhoto.Height;
                }
            }
            return retval;
        }

        public static string GetUnigateFileName(string filewithextension)
        {
            string filename = Path.GetFileNameWithoutExtension(filewithextension)
                                  .ToLower()
                                  .Replace("ç", "c")
                                  .Replace("ğ", "g")
                                  .Replace("ı", "i")
                                  .Replace("ö", "o")
                                  .Replace("ş", "s")
                                  .Replace("ü", "u")
                                  .Replace(".", "-")
                                  .Replace(" ", "-");
            string extension = Path.GetExtension(filewithextension);
            return string.Format("{0}-{1}{2}", filename, Guid.NewGuid(), extension);
        }
    }
}