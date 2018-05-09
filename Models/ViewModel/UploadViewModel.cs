using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models.ViewModel
{
    /// <summary>
    /// Wrapper for a post/upload in the same time call from client part
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UploadViewModel<T> where T : class
    {
        public IFormFile file { get; set; }
        public T entity { get; set; }
    }

    public class UploadViewModel
    {
        public IFormFile file { get; set; }
    }
}
