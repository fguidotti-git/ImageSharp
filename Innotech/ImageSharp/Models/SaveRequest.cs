namespace ImageSharp.Models
{

    public class SaveRequest
    {
        public int RequestId { get; set; }
        public List<ImageFile> Images { get; set; }
    }

    public class ImageFile
    {
        public string FileName { get; set; }
        public string base64 { get; set; }
    }


}
