using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Models.ViewModel
{
    public class UploadViewModel<T> where T : class
    {
        public IFormFile file { get; set; }
        public T entity { get; set; }
    }
}
