using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Venom.Lib.Image
{
    public class ImageBase64
    {
        public string Url { get; private set; }
        private byte[] DataBytes { get; set; }
        public bool HasContent { get { return DataBytes != null && DataBytes.Length > 0; } }

        private ImageBase64() { }

        public static ImageBase64 FromString(string url)
        {
            var img = new ImageBase64();
            img.DataBytes = Convert.FromBase64String(Regex.Replace(url, "data:image/(png|jpg|gif|jpeg|pjpeg|x-png);base64,", ""));
            img.Url = url;
            return img;
        }

        public static ImageBase64 FromBytes(byte[] data, string extensionName)
        {
            var img = new ImageBase64();
            img.DataBytes = data; 
            img.Url = string.Format("data:image/{0};base64,{1}", extensionName, Convert.ToBase64String(data));
            return img;
        }

        public static ImageBase64 Open(string path)
        {
            if (path == null || !System.IO.File.Exists(path))
                throw new Exception("Invalid path file");
            var ext = new System.IO.FileInfo(path).Extension;
            return FromBytes(System.IO.File.ReadAllBytes(path), ext.Replace(".", string.Empty));
        }

        public bool Save(string path)
        {
            var fileInfo = new System.IO.FileInfo(path);
            if (!System.IO.Directory.Exists(fileInfo.DirectoryName))
                System.IO.Directory.CreateDirectory(fileInfo.DirectoryName);
            System.IO.File.WriteAllBytes(path, this.DataBytes);
            return true;
        }
    }
}
