﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using YsrisCoreLibrary.Models;

namespace YsrisCoreLibrary.Dal
{
    public class OpenStreetMapDal : AbstractDal<OsmEntity>
    {
        public IEnumerable<OsmEntity> Query(string q)
        {
            var json_data = new HttpClient().GetAsync($"http://nominatim.openstreetmap.org/search?format=json&q={q}").Result.Content.ReadAsStringAsync().Result;
            var data = JsonConvert.DeserializeObject<IEnumerable<OsmEntity>>(json_data);
            return data;
        }
    }
}