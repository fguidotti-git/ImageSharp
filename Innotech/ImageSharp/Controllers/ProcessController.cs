using ImageSharp.Models;
using ImageSharp.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace ImageSharp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ILogger _logger;


        public ProcessController(IImageService imageService, ILogger<SaveController> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }


        [HttpPost]
        public async Task<ProcessResponse> Index([FromBody] ProcessRequest model)
        {
            if (model == null || model.FileNames.Count == 0)
            {
                throw new Exception("Bad Request");
            }

            ProcessResponse response = new ProcessResponse();
            response.Images = new List<ImageFile>();

            try
            {

                Parallel.ForEach(model.FileNames, async (fileName, loopState) =>
                {

                    var base64 = await _imageService.Process(fileName);
                    if (string.IsNullOrEmpty(base64))
                    {
                        fileName = $"fail_{fileName}";
                    }

                    response.Images.Add(new ImageFile() { base64 = base64, FileName = fileName });

                });

            }
            catch (Exception e)
            {
                _logger.LogError("SaveController.Index: ", e.Message);
            }

            return response;
        }


    }
}
