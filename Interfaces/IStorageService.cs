using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ysriscorelibrary.Interfaces
{
    public interface IStorageService
    {
        void SavePictureTo(IFormFile postedFile, string fullPath, int? width = null, int? height = null);
        void SaveFileTo(IFormFile postedFile, string fullPath);
        void SaveFileTo(MemoryStream postedFile2, string fullPath);
        Task<MemoryStream> GetFileContent(string fullPath);
        IEnumerable<IListBlobItem> ListBlobs(CloudBlobDirectory container);

    }
}