namespace Mshop.Application.UseCases.Images.Common
{
    public class ImageModelOutPut
    {
        public ImageModelOutPut(string image)
        {
            Image = image;
        }

        public string Image { get; set; }
    }
}
