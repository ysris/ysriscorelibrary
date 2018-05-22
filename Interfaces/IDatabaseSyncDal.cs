using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Interfaces
{
    public interface IDatabaseSyncDal
    {
        Task syncAll();
        void _importYoozXml();
        void _callYoozUpdater();
        void SynchronizeWinBizCustomer();
        void SynchronizeYoozCustomer();
    }
}
