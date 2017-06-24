using SimpleCrypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YsrisCoreLibrary.Helpers;

namespace YsrisCoreLibrary.Helpers
{
    /// <summary>
    /// Encryption helper
    /// </summary>
    public class EncryptionHelper
    {
        private string Salt => ConfigurationHelper.EncryptionSalt;
        private readonly ICryptoService cryptoService = new PBKDF2();

        /// <summary>
        /// Get the hash for a given value with the default salt (set in app.config/web.config)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetHash(string value) => cryptoService.Compute(value, Salt);

        public string GetHash(string value, string _salt) => cryptoService.Compute(value, _salt);
    }
}
