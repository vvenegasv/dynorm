using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Amazon;
using Amazon.Runtime;
using DynORM.Models;
using Newtonsoft.Json;

namespace DynORM
{
    internal class ConfigReader
    {
        private static volatile ConfigReader _instance;
        private static object _syncRoot = new Object();
        private AWSCredentials _credentials;
        private ConfigModel _configuration;
        private bool _configWasRead = false;


        private ConfigReader()
        {
        }

        public static ConfigReader Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new ConfigReader();
                    }
                }
                return _instance;
            }
        }

        public AWSCredentials Credentials
        {
            get
            {
                if (!_configWasRead)
                {
                    ReadConfiguration();
                    _credentials = GetCredential(_configuration);
                }

                return _credentials;
            }
        }

        public RegionEndpoint Endpoint
        {
            get
            {
                if(_configWasRead)
                    ReadConfiguration();

                switch (_configuration.Endpoint)
                {
                    case "USEast1":
                        return RegionEndpoint.USEast1;
                    case "CACentral1":
                        return RegionEndpoint.CACentral1;
                    case "CNNorth1":
                        return RegionEndpoint.CNNorth1;
                    case "USGovCloudWest1":
                        return RegionEndpoint.USGovCloudWest1;
                    case "SAEast1":
                        return RegionEndpoint.SAEast1;
                    case "APSoutheast1":
                        return RegionEndpoint.APSoutheast1;
                    case "APSouth1":
                        return RegionEndpoint.APSouth1;
                    case "APNortheast2":
                        return RegionEndpoint.APNortheast2;
                    case "APSoutheast2":
                        return RegionEndpoint.APSoutheast2;
                    case "EUCentral1":
                        return RegionEndpoint.EUCentral1;
                    case "EUWest2":
                        return RegionEndpoint.EUWest2;
                    case "EUWest1":
                        return RegionEndpoint.EUWest1;
                    case "USWest2":
                        return RegionEndpoint.USWest2;
                    case "USWest1":
                        return RegionEndpoint.USWest1;
                    case "USEast2":
                        return RegionEndpoint.USEast2;
                    case "APNortheast1":
                        return RegionEndpoint.APNortheast1;
                    default:
                        return RegionEndpoint.USEast1;
                }
            }
        }

        public string EnviromentPrefix
        {
            get
            {
                if (!_configWasRead)
                    ReadConfiguration();

                return _configuration.EnviromentPrefix; ;
            }
        }

        private void ReadConfiguration()
        {
            //Set flag to true to don't read twice or more
            _configWasRead = true;

            string path = Assembly.GetEntryAssembly().Location;
            _configuration = new ConfigModel();

            //Try to get config file from current directory
            //or parent directory. If it does not exists,
            //it uses default values
            var file = Path.Combine(path, "appsettings.json");
            if (!File.Exists(file))
                file = Path.Combine(Directory.GetParent(path).FullName, "appsettings.json");
            if (!File.Exists(file))
                return;

            //Get the config object
            var fileContent = File.ReadAllText(file);
            dynamic configObject = JsonConvert.DeserializeObject(fileContent);

            //Check if exists the configuration for this package 
            if (configObject.DynORM == null)
                return;
            
            //Read config file
            if (configObject.DynORM.PublicKey != null)
                _configuration.PublicKey = configObject.DynORM.PublicKey;

            if (configObject.DynORM.PrivateKey != null)
                _configuration.PrivateKey = configObject.DynORM.PrivateKey;

            if (configObject.DynORM.FilePath != null)
                _configuration.FilePath = configObject.DynORM.FilePath;

            if (configObject.DynORM.Endpoint != null)
                _configuration.Endpoint = configObject.DynORM.Endpoint;

            if (configObject.DynORM.EnviromentPrefix != null)
                _configuration.EnviromentPrefix = configObject.DynORM.EnviromentPrefix;
        }

        private AWSCredentials GetCredential(ConfigModel config)
        {
            if (!string.IsNullOrWhiteSpace(config.PublicKey) && !string.IsNullOrWhiteSpace(config.PrivateKey))
                return ModelToAwsCredentials(config);

            if(!string.IsNullOrWhiteSpace(config.FilePath))
                if (File.Exists(config.FilePath))
                {
                    var fileContent = File.ReadAllText(config.FilePath);
                    var credential = JsonConvert.DeserializeObject<CredentialModel>(fileContent);
                    if(credential != null)
                        return ModelToAwsCredentials(credential);
                }

            return null;
        }

        private AWSCredentials ModelToAwsCredentials(CredentialModel model)
        {
            return new BasicAWSCredentials(model.PublicKey, model.PrivateKey);
        }
    }
}
