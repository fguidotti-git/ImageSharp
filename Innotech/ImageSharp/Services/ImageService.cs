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
                using (var image = new FileStream($"{requestId}_{imageFile.FileName}", FileMode.Create))
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


        public async Task<string> Process(string imageName)
        {
            try
            {
                String base64;

                FontCollection collection = new();
                FontFamily family = collection.Add("path/to/font.ttf");
                Font font = family.CreateFont(12, FontStyle.Italic);

                DirectoryInfo directoryInfo = new DirectoryInfo("");
                FileInfo file = directoryInfo.GetFiles($"*{imageName}").FirstOrDefault();

                if (file == null)
                    throw new Exception("File not found");

                using (Image image = Image.Load(file.FullName))
                {
                    image.Mutate(x => x.DrawText(file.Name, font, Color.Black, new PointF(10, 10)));
                    base64 = image.ToBase64String(JpegFormat.Instance);
                }
                return base64;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return "";
            }

        }

    }
}
