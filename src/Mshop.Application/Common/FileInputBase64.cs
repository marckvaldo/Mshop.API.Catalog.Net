namespace Mshop.Application.Common
{
    public class FileInputBase64
    {
        public string FileStremBase64 { get; set; }
        public FileInputBase64(string fileStremBase64) 
        {
            FileStremBase64 = fileStremBase64;
            isValid();
        }

        public void isValid()
        {
            if (!string.IsNullOrEmpty(FileStremBase64) && string.IsNullOrEmpty(Helpers.GetExtensionBase64(FileStremBase64).Trim()))
                throw new Exception("not found data:image in the file");

            if (!string.IsNullOrEmpty(FileStremBase64) && !Helpers.IsBase64String(FileStremBase64))
                throw new Exception("File não é um arquivo Base64 válido");
        }

    }


    
}
