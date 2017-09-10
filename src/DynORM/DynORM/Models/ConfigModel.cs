using System;
using System.Collections.Generic;
using System.Text;

namespace DynORM.Models
{
    internal class ConfigModel: CredentialModel
    {
        public string FilePath { get; set; }
        
        public string Endpoint { get; set; }

        public string EnviromentPrefix { get; set; }
    }
}
