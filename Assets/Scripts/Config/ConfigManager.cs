using System.IO;
using cfg;
using Cysharp.Threading.Tasks;
using SimpleJSON;
using Singleton;
using UnityEngine;
using UnityEngine.Networking;

namespace Config
{
    public class ConfigManager:SingletonBase<ConfigManager>
    {
        private readonly string gameConfDir = Application.streamingAssetsPath + "/DataTablesGen";

        public Tables Tables { get; private set; }

        private async UniTask<string> GetTextForStreamingAssets(string path)
        {
            var uri = new System.Uri(Path.Combine(path));
            Debug.Log(uri);
            UnityWebRequest request = UnityWebRequest.Get(uri);
            await request.SendWebRequest().ToUniTask();//读取数据
            if (request.error != null) return null;
            return request.downloadHandler.text;
        }
        
        public async UniTask Init()
        {
            Tables = new Tables();
#if UNITY_WEBGL
            await Tables.Init(async file => JSON.Parse(await GetTextForStreamingAssets($"{gameConfDir}/{file}.json")));
#else
            Tables.Init(file => JSON.Parse(File.ReadAllText($"{gameConfDir}/{file}.json")));
#endif
        }
    }
}