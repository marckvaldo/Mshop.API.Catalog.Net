using System.Text.RegularExpressions;

namespace Mshop.Application.Common
{
    public static class Helpers
    {
        /*Files*/
        public static string GetExtensionBase64(string file)
        {
            return StringBetween(file, "/", ";");
        }

        public static string CleanExtensionBase64(string file)
        {
            var fileClean = new Regex(@"^data:image\/[a-z]+;base64,").Replace(file, "");
            return fileClean;
        }
        public static FileInput Base64ToStream(string file) 
        {
            var extension = GetExtensionBase64(file);
            var fileClean = new Regex(@"^data:image\/[a-z]+;base64,").Replace(file,"");
            byte[] imageByte = Convert.FromBase64String(fileClean);
            var stream = new MemoryStream(imageByte);

            return new FileInput(extension, stream);
        }

        public static string StreamToBase64(this Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            return Convert.ToBase64String(bytes);
           
        }

        public static bool IsBase64String(string base64)
        {
            base64 = CleanExtensionBase64(base64);
            return (base64.Length % 4 == 0) && Regex.IsMatch(base64, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }


        /*String*/
        public static string StringBetween(this string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.LastIndexOf(b);
            if (posA == -1 || posB == -1)
                return "";
  
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
                return "";

            //return value.Substring(adjustedPosA, posB - adjustedPosA);
            return value[adjustedPosA..(posB)];
        }

        public static string StringBefore(this string value, string a)
        {
            int posA = value.IndexOf(a);
            if (posA == -1)
            return "";

            //return value.Substring(0, posA);
            return value[..posA];
        }

        public static string StringAfter(this string value, string a)
        {
            int posA = value.LastIndexOf(a);
            if (posA == -1)
                return "";
            
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= value.Length)
                return "";

            //return value.Substring(adjustedPosA);
            return value[adjustedPosA..];
        }
    }
}
