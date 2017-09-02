using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using YsrisCoreLibrary.Extensions;
using YsrisCoreLibrary.Helpers;
using System.Reflection.Metadata.Ecma335;
using ImageSharp;
using ImageSharp.Formats;
using ImageSharp.Processing;
using Microsoft.AspNetCore.Cors.Infrastructure;
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
                    + fullPath;

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

            fullPath = fullPath.TrimStart('/');

            try
            {
                var storageAccount = CloudStorageAccount.Parse(ConfigurationHelper.BlobStorageConnectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("inoutdata");
                var blockBlob = container.GetBlockBlobReference(fullPath);

                var fileStream = new MemoryStream();
                await blockBlob.DownloadToStreamAsync(fileStream);

                return fileStream;
            }
            catch (StorageException)
            {
                return null;
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
