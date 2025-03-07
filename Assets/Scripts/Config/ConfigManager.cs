using System.IO;
using cfg;
using SimpleJSON;
using Singleton;
using UnityEngine;

namespace Config
{
    public class ConfigManager:SingletonBase<ConfigManager>
    {
        private string gameConfDir = Application.streamingAssetsPath + "/DataTablesGen/";
        
        private Tables tables;
        public Tables Tables
        {
            get
            {
                return tables ??= new cfg.Tables(file => JSON.Parse(File.ReadAllText($"{gameConfDir}/{file}.json")));
            }
        }
    }
}