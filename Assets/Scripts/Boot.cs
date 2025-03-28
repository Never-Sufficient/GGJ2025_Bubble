using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

public class Boot : MonoBehaviour
{
    private class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }
        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }
        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }
    /// <summary>
    /// 资源系统运行模式
    /// </summary>
    public EPlayMode PlayMode = EPlayMode.EditorSimulateMode;

    void Awake()
    {
        Debug.Log($"资源系统运行模式：{PlayMode}");
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        DontDestroyOnLoad(this.gameObject);
    }

    async void Start()
    {
        // 初始化资源系统
        YooAssets.Initialize();

        // 设置默认的资源包
        var gamePackage = YooAssets.CreatePackage("DefaultPackage");
        YooAssets.SetDefaultPackage(gamePackage);

        await InitPackage(gamePackage, PlayMode);
        var version = await RequestPackageVersion();
        await UpdatePackageManifest(version);

        // 切换到主页面场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private IEnumerator InitPackage(ResourcePackage package, EPlayMode playMode)
    {
        if (playMode == EPlayMode.EditorSimulateMode)
        {
            var buildResult = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
            var packageRoot = buildResult.PackageRootDirectory;
            var editorFileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
            var initParameters = new EditorSimulateModeParameters();
            initParameters.EditorFileSystemParameters = editorFileSystemParams;
            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;
            if (initOperation.Status == EOperationStatus.Succeed)
                Debug.Log("资源包初始化成功！");
            else
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");
        }
        else if (playMode == EPlayMode.OfflinePlayMode)
        {
            var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            var initParameters = new OfflinePlayModeParameters();
            initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;
            if (initOperation.Status == EOperationStatus.Succeed)
                Debug.Log("资源包初始化成功！");
            else
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");
        }
        else if (playMode == EPlayMode.WebPlayMode)
        {
            string defaultHostServer = "http://localhost/StreamingAssets/yoo";
            string fallbackHostServer = "http://localhost/StreamingAssets/yoo";
            //说明：RemoteServices类定义请参考联机运行模式！
            IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            var webServerFileSystemParams = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
            var webRemoteFileSystemParams = FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remoteServices); //支持跨域下载
    
            var initParameters = new WebPlayModeParameters();
            initParameters.WebServerFileSystemParameters = webServerFileSystemParams;
            initParameters.WebRemoteFileSystemParameters = webRemoteFileSystemParams;
    
            var initOperation = package.InitializeAsync(initParameters);
            yield return initOperation;
    
            if(initOperation.Status == EOperationStatus.Succeed)
                Debug.Log("资源包初始化成功！");
            else 
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");
        }
    }
    
    private async UniTask<string> RequestPackageVersion()
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.RequestPackageVersionAsync();
        await operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            //更新成功
            string packageVersion = operation.PackageVersion;
            Debug.Log($"Request package Version : {packageVersion}");
            return packageVersion;
        }
        else
        {
            //更新失败
            Debug.LogError(operation.Error);
            return "";
        }
    }
    
    private async UniTask UpdatePackageManifest(string packageVersion)
    {
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.UpdatePackageManifestAsync(packageVersion);
        await operation;

        if (operation.Status == EOperationStatus.Succeed)
        {
            //更新成功
        }
        else
        {
            //更新失败
            Debug.LogError(operation.Error);
        }
    }
}