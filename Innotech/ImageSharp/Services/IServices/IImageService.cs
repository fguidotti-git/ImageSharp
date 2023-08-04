using ImageSharp.Models;

namespace ImageSharp.Services.IServices
{
    public interface IImageService
    {
        public Task<bool> Save(ImageFile image, int requestId);

        public Task<string> Process(string imageName);
    }
}
