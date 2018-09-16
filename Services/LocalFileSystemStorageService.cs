using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ysriscorelibrary.Interfaces;
using YsrisCoreLibrary.Models;

namespace YsrisCoreLibrary.Services
{
    public class LocalFileSystemStorageService : IStorageService
    {
        private ILogger<MailHelperService> MyLogger;
        private IHostingEnvironment _env;

        private string basePath => _env.ContentRootPath + "/uploads/";

        public LocalFileSystemStorageService(SessionHelperService<ICustomer> sessionHelper, ILogger<MailHelperService> logger, IHostingEnvironment env)
        {
            MyLogger = logger;
            _env = env;
        }

        public void SavePictureTo(IFormFile postedFile, string fullPath, int? width = null)
        {
            var outputStream = new MemoryStream();
            var inputstream = new MemoryStream();
            postedFile.CopyTo(inputstream);
            inputstream.Seek(0, SeekOrigin.Begin);

            //using (var inputStream = fileInfo.CreateReadStream())
            using (var image = Image.Load(inputstream))
            {
                if (width != null)
                {
                    Rectangle cropRectangle = new Rectangle();

                    if (image.Width > image.Height)
                    {
                        var cropX = Convert.ToInt32((image.Width - image.Height) / 2);
                        var cropY = 0;
                        cropRectangle = new Rectangle(cropX, cropY, image.Height, image.Height);
                    }
                    else
                    {
                        var cropX = 0;
                        var cropY = Convert.ToInt32((image.Height - image.Width) / 2);
                        cropRectangle = new Rectangle(cropX, cropY, image.Width, image.Width);
                    }

                    image.Mutate(x => x
                        .Crop(cropRectangle)
                        .Resize((int)width, (int)width));

                    image.Save(outputStream, new JpegEncoder { IgnoreMetadata = true });
                }
                else
                {
                    image.SaveAsJpeg(outputStream);
                }
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


        public void SaveContentTo(string fullPath, string content)
        {
            fullPath = basePath + fullPath.TrimStart('/');
            MyLogger.LogInformation($"fullPath=" + fullPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            File.WriteAllText(fullPath, content);
        }

        public FileInfo GetFileInfo(string filePath) => new FileInfo(GetFullPath(filePath));

        public bool FileExists(string xFilePath) => File.Exists(GetFullPath(xFilePath));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public async Task<MemoryStream> GetFileContent(string fullPath)
        {
            return await Task.Run(() =>
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
            });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="recursive"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public IEnumerable<string> ListFiles(IEnumerable<string> directory, bool recursive = true, string searchPattern = "*.*")
        {
            return directory.SelectMany(a => ListFiles(a, recursive, searchPattern) ?? Enumerable.Empty<string>());
        }

        public IEnumerable<string> ListFiles(string directory, bool recursive = true, string searchPattern = "*.*")
        {
            directory =
                 basePath
                + directory.TrimStart('/');

            if (!Directory.Exists(directory))
                return Enumerable.Empty<string>();

            return Directory.GetFiles(directory, searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Select(a => a.Replace(basePath, string.Empty).PadLeft('/').Trim());
        }

        public void MoveFile(string from, string to)
        {
            if (!Directory.Exists(basePath + Path.GetDirectoryName(to.TrimStart('/'))))
                Directory.CreateDirectory(Path.Combine(basePath, Path.GetDirectoryName(to.TrimStart('/'))));
            File.Copy(basePath + from, basePath + to.TrimStart('/'), true);
            File.Delete(basePath + from);
        }

        public void MoveFileAbsolutePath(string from, string to)
        {
            if (!Directory.Exists(Path.GetDirectoryName(to)))
                Directory.CreateDirectory(Path.GetDirectoryName(to));
            File.Copy(from, to, true);
            File.Delete(from);
        }

        public string GetFullPath(string cur)
        {
            var filename = Path.Combine(basePath + cur.TrimStart('/').TrimEnd('\\'));
            return filename;
        }

        public string GetContentType(string filePath)
        {
            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out contentType);
            return contentType;
        }
    }
}
