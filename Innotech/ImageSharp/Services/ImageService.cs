using ImageSharp.Models;
using ImageSharp.Services.IServices;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Buffers.Text;
using System.Globalization;
using System.Net;

namespace ImageSharp.Services
{
    public class ImageService : IImageService
    {
        private readonly ILogger _logger;
        public ImageService(ILogger<ImageService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> Save(ImageFile imageFile, int requestId)
        {
            try
            {
                var bytes = Convert.FromBase64String(imageFile.base64);
                using (var image = new FileStream($"Images\\{requestId}_{imageFile.FileName}", FileMode.Create))
                {
                    image.Write(bytes, 0, bytes.Length);
                    image.Flush();
                }
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return false;
            }

        }


        public async Task<ImageFile> Process(string imageName)
        {
            ImageFile imageFile = new ImageFile();
            string base64 = String.Empty;

            try
            {
                
                FontCollection collection = new();
                FontFamily family = collection.Add("Fonts\\Roboto-Regular.ttf");
                Font font = family.CreateFont(12, FontStyle.Bold);

                DirectoryInfo directoryInfo = new DirectoryInfo("Images");
                FileInfo file = directoryInfo.GetFiles($"*{imageName}").FirstOrDefault();

                if (file == null)
                {
                    imageFile.FileName = $"404_{imageName}";
                }
                else
                {
                    using (Image image = Image.Load(file.FullName))
                    {
                        image.Mutate(x => x.DrawText(file.Name.Replace("_", " "), font, Color.YellowGreen, new PointF(10, 10)));
                        base64 = image.ToBase64String(JpegFormat.Instance);
                    }
                    imageFile.FileName = file.Name;
                    imageFile.base64 = base64;
                }
                
                return imageFile;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                imageFile.FileName = $"Error_{imageName}";
                return imageFile;
            }

        }

    }
}
