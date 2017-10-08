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
        private IHostingEnvironment Env;

        public LocalFileSystemStorageService(SessionHelperService sessionHelper, ILogger<MailHelperService> logger, IHostingEnvironment env)
        {
            MyLogger = logger;
            Env = env;
        }

        public void SavePictureTo(IFormFile postedFile, string fullPath, int? width, int? height)
        {
            var fileInfo = Env.WebRootFileProvider.GetFileInfo(new Uri(Env.ContentRootPath + "/uploads/" + fullPath.TrimStart('/')).LocalPath);

            var outputStream = new MemoryStream();
            var inputstream = new MemoryStream();
            postedFile.CopyToAsync(inputstream);
            inputstream.Seek(0, SeekOrigin.Begin);

            //using (var inputStream = fileInfo.CreateReadStream())
            using (var image = Image.Load(inputstream))
            {
                image
                    .Crop((int)width, (int)height)
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
            fullPath = Env.ContentRootPath + "/uploads/" + fullPath.TrimStart('/');

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

            
            fullPath = Env.ContentRootPath + "/uploads/" + fullPath.TrimStart('/');

			using (MemoryStream ms = new MemoryStream())
			using (FileStream file = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
			{

                file.CopyTo(ms);
                return ms;
            }

        }

        public IEnumerable<Uri> ListFiles(string directory)
        {
            directory = "/uploads/" + directory.TrimStart('/');
            return Directory.GetFiles(directory).Select(a => new Uri(a));
        }
    }
}
