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

            int responseID = 0;

            ProcessResponse response = new ProcessResponse();
            response.Images = new List<ImageFile>();

            try
            {

                Parallel.ForEach(model.FileNames, async (fileName, loopState) =>
                {

                    var file = await _imageService.Process(fileName);

                    if(!file.FileName.Contains("404") && !file.FileName.Contains("Fail"))
                    {
                        var arrName = file.FileName.Split('_');
                        file.FileName = arrName[1];
                        responseID = Convert.ToInt32(arrName[0]);
                    }

                    response.Images.Add(file);

                });

            }
            catch (Exception e)
            {
                _logger.LogError("ProcessController.Index: ", e.Message);
            }

            response.RequestId = responseID;
            return response;
        }


    }
}
