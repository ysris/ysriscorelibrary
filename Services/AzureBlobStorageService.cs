﻿using System;
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
    public class AzureBlobStorageService : IStorageService
    {
        private SessionHelperService SessionHelperInstance;
        private ILogger<MailHelperService> MyLogger;
        private IHostingEnvironment Env;

        public AzureBlobStorageService(SessionHelperService sessionHelper, ILogger<MailHelperService> logger, IHostingEnvironment env)
        {
            SessionHelperInstance = sessionHelper;
            MyLogger = logger;
            Env = env;
        }

        public void SavePictureTo(IFormFile postedFile, string fullPath, int? width = null, int? height = null)
        {
            // Get the stream

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

        public async void SaveFileTo(MemoryStream postedFile2, string fullPath)
        {
            fullPath = fullPath.TrimStart('/');

            //    //Connect to Azure
            var storageAccount = CloudStorageAccount.Parse(ConfigurationHelper.BlobStorageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("ConfigurationHelper.StorageContainerName");
            var x = container.CreateIfNotExistsAsync().Result;

            //  ref to the file
            var blockBlob = container.GetBlockBlobReference(fullPath);


            postedFile2.Position = 0;
            using (postedFile2)
            {
                await blockBlob.UploadFromStreamAsync(postedFile2);
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

        public IEnumerable<string> ListFiles(string directory)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationHelper.BlobStorageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blockBlob = blobClient.GetContainerReference("inoutdata").GetDirectoryReference(directory);
            BlobContinuationToken continuationToken = null;
            List<IListBlobItem> results = new List<IListBlobItem>();
            do
            {
                var response = blockBlob.ListBlobsSegmentedAsync(continuationToken).Result;
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);
            return results.Select(a => a.Uri.AbsolutePath.ToString());
        }
        
        public void MoveFile(string from, string to)
        {
          throw new NotImplementedException();
        }

        public string GetFullPath(string cur)
        {
            throw new NotImplementedException();
        }
    }
}