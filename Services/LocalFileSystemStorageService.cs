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

        public void SavePictureTo(IFormFile postedFile, string fullPath, int? width = null, int? height = null)
        {
            var postedFile2 = new MemoryStream();
            postedFile.CopyTo(postedFile2);

            var img = Image.Load(postedFile2.ToArray());
            if (width != null && height != null)
                img.Resize((int)width, (int)height);

            img.SaveAsJpeg(postedFile2);

            // Upload the picture to blob
            SaveFileTo(postedFile2, fullPath);
        }
        public void SaveFileTo(IFormFile postedFile, string fullPath)
        {
            var postedFile2 = new MemoryStream();
            postedFile.CopyTo(postedFile2);

            SaveFileTo(postedFile2, fullPath);
        }

        public void SaveFileTo(MemoryStream postedFile2, string fullPath)
        {
            fullPath =
                    Env.ContentRootPath
                    + ConfigurationHelper.StorageContainerName
                    + "/"
                    + fullPath.TrimStart('/');

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

            fullPath =
                    Env.ContentRootPath
                    + ConfigurationHelper.StorageContainerName
                    + "/"
                    + fullPath.TrimStart('/');


			using (MemoryStream ms = new MemoryStream())
			using (FileStream file = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
			{

                file.CopyTo(ms);
                return ms;
            }

        }

        public IEnumerable<IListBlobItem> ListBlobs(CloudBlobDirectory container)
        {
            BlobContinuationToken continuationToken = null;
            List<IListBlobItem> results = new List<IListBlobItem>();
            do
            {
                var response = container.ListBlobsSegmentedAsync(continuationToken).Result;
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);
            return results;
        }
    }
}
