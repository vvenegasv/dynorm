namespace DynORM.InternalModels
{
    internal class ConfigModel: CredentialModel
    {
        public string FilePath { get; set; }
        
        public string Endpoint { get; set; }

        public string EnviromentPrefix { get; set; }
    }
}
