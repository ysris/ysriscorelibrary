using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using ImageSharp;
using ImageSharp.Formats;
using ImageSharp.Processing;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace YsrisCoreLibrary.Helpers
{
    public class FileSystemHelper
    {
        public static void SavePictureTo(IFormFile postedFile, string fullPath, int? width = null, int? height = null)
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

        public static void SaveFileTo(IFormFile postedFile, string fullPath)
        {
            var postedFile2 = new MemoryStream();
            postedFile.CopyTo(postedFile2);

            SaveFileTo(postedFile2, fullPath);         
        }

        public static async void SaveFileTo(MemoryStream postedFile2, string fullPath)
        {
            fullPath = fullPath.TrimStart('/');

            //    //Connect to Azure
            var storageAccount = CloudStorageAccount.Parse(ConfigurationHelper.BlobStorageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference("inoutdata");
            var x = container.CreateIfNotExistsAsync().Result;

            //  ref to the file
            var blockBlob = container.GetBlockBlobReference(fullPath);
            

            postedFile2.Position = 0;
            using (postedFile2)
            {
                await blockBlob.UploadFromStreamAsync(postedFile2);
            }
        }

        public static async Task<MemoryStream> GetFileContent(string fullPath)
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

        public static IEnumerable<IListBlobItem> ListBlobs(CloudBlobDirectory container)
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
