using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using YsrisCoreLibrary.Extensions;
using YsrisCoreLibrary.Helpers;
using ImageSharp;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using ysriscorelibrary.Interfaces;
using System;
using System.Linq;

namespace YsrisCoreLibrary.Services
{
    public class LocalFileSystemStorageService : IStorageService
    {
        private ILogger<MailHelperService> MyLogger;
        private IHostingEnvironment _env;

        private string basePath => _env.ContentRootPath + "/uploads/";

        public LocalFileSystemStorageService(SessionHelperService sessionHelper, ILogger<MailHelperService> logger, IHostingEnvironment env)
        {
            MyLogger = logger;
            _env = env;
        }


        public void SaveCroppedPictureTo(IFormFile postedFile, string fullPath, int width)
        {
            var outputStream = new MemoryStream();
            var inputstream = new MemoryStream();
            postedFile.CopyToAsync(inputstream);
            inputstream.Seek(0, SeekOrigin.Begin);

            //using (var inputStream = fileInfo.CreateReadStream())
            using (var image = Image.Load(inputstream))
            {
                var height = image.Height * (width / image.Width);
                if (width != null && height != null)
                    image
                        .Resize((int)width, (int)height)
                        .Crop((int)width, (int)width)
                        .SaveAsJpeg(outputStream);
                else
                    image
                        .SaveAsJpeg(outputStream);
            }
            outputStream.Seek(0, SeekOrigin.Begin);


            SaveFileTo(outputStream, fullPath.TrimStart('/'));
        }

        public void SavePictureTo(IFormFile postedFile, string fullPath, int? width)
        {
            var outputStream = new MemoryStream();
            var inputstream = new MemoryStream();
            postedFile.CopyToAsync(inputstream);
            inputstream.Seek(0, SeekOrigin.Begin);

            //using (var inputStream = fileInfo.CreateReadStream())
            using (var image = Image.Load(inputstream))
            {
                var height = image.Height * (width / image.Width);
                if (width != null && height != null)
                    image                        
                        .Resize((int)width, (int)height)
                        .SaveAsJpeg(outputStream);
                else
                    image
                        .SaveAsJpeg(outputStream);
            }
            outputStream.Seek(0, SeekOrigin.Begin);


            SaveFileTo(outputStream, fullPath.TrimStart('/'));

        }
        public void SaveFileTo(IFormFile postedFile, string fullPath)
        {
            var postedFile2 = new MemoryStream();
            postedFile.CopyTo(postedFile2);

            SaveFileTo(postedFile2, fullPath.TrimStart('/'));
        }

        public void SaveFileTo(MemoryStream postedFile2, string fullPath)
        {
            fullPath = basePath + fullPath.TrimStart('/');

            MyLogger.LogInformation($"fullPath=" + fullPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            FileStream file = null;

            using (postedFile2)
            using (file = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                postedFile2.WriteTo(file);
            }
        }

        public async Task<MemoryStream> GetFileContent(string fullPath)
        {
            if (fullPath == null)
                return null;


            fullPath = basePath + fullPath.TrimStart('/');

            using (MemoryStream ms = new MemoryStream())
            using (FileStream file = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {

                file.CopyTo(ms);
                return ms;
            }

        }

        public IEnumerable<string> ListFiles(string directory)
        {
            directory = basePath + directory.TrimStart('/');
            var set =
                Directory.Exists(directory)
                    ? Directory.GetFiles(directory, "*.*"/*, SearchOption.AllDirectories*/).Select(a => a.Replace(basePath, string.Empty).PadLeft('/').Trim())
                    .Concat(Directory.GetDirectories(directory, "*.*").Select(a => a.Replace(basePath, string.Empty).PadLeft('/').Trim()))
                    : new List<string>();
            return set;
        }

        public void MoveFile(string from, string to)
        {
            if (!Directory.Exists(basePath + Path.GetDirectoryName(to.TrimStart('/'))))
                Directory.CreateDirectory(basePath + Path.GetDirectoryName(to.TrimStart('/')));
            File.Copy(basePath + from, basePath + to.TrimStart('/'));
            File.Delete(basePath + from);
        }

        public string GetFullPath(string cur)
        {
            var filename = basePath + cur;
            return filename;
        }
    }
}
