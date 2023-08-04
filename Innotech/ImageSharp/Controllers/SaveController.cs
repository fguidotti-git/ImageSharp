using ImageSharp.Models;
using ImageSharp.Services;
using ImageSharp.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Web;

namespace ImageSharp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SaveController : ControllerBase
    {
            private readonly IImageService _imageService;
            private readonly ILogger _logger;


            public SaveController(IImageService imageService, ILogger<SaveController> logger)
            {
                _imageService = imageService;
                _logger = logger;
            }

        /// <summary>
        /// Receive a list of base64 images, convert them into byte images and save them to disk
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
            public async Task<SaveResponse> Index([FromBody] SaveRequest model)
            {
                string status = "Sucess";
                var exceptions = new ConcurrentQueue<Exception>();

                if (model == null || model.Images.Count == 0)
                {
                    status = "BadRequest";
                    return new SaveResponse() { RequestId = 0, Status = status };
                }

                var success = true;
                try
                {

                    Parallel.ForEach(model.Images, async (image, loopState) =>
                    {
                        success = await _imageService.Save(image, model.RequestId);
                        if (!success)
                        {
                            loopState.Break();
                            status = $"Fail on {image.FileName}";
                        }

                    });

                }
                catch (Exception e)
                {
                    _logger.LogError("SaveController.Index: ", e.Message);
                    status = "Fail";
                }

                return new SaveResponse() { RequestId = model.RequestId, Status = status };
            }



    }
}
