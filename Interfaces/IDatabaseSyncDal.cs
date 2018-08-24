using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Interfaces
{
    public interface IDatabaseSyncDal
    {
        Task SyncAllHourly();
        Task SyncAllDaily();
    }
}
