using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using O2un.Core.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace O2un.Resource
{
    public class PatchManager : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI _fileSize;

        public AssetLabelReference[] _required;
        private long _patchSize;
        private Dictionary<string, long> _patchMap = new();

        bool _ready2Down = false;
        private async UniTaskVoid Start()
        {
            _ready2Down = false;

            await Addressables.InitializeAsync();
            await CheckPatchFiles();
        }

        private async UniTask CheckPatchFiles()
        {
            _patchSize = default;

            foreach (var l in _required)
            {
                var handle = Addressables.GetDownloadSizeAsync(l);
                await handle;
                _patchSize += handle.Result;
            }

            if (0 < _patchSize)
            {
                // 다운 온;
                _ready2Down = true;
                _fileSize.text = $"FileSize : {_patchSize.FormatBytes()}";
            }
            else
            {
                await UniTask.WaitForSeconds(2);
                // TODO : 필요한 씬으로 변경
                SceneManager.LoadScene("Lobby");
                return;
            }
        }

        private async UniTaskVoid PatchFiles()
        {
            foreach (var l in _required)
            {
                var handle = Addressables.GetDownloadSizeAsync(l);
                await handle;

                if (decimal.Zero < handle.Result)
                {
                    DownLoadLabel(l.labelString).Forget();
                }
            }

            // NOTE : 패치진행 상황 UI 변경 함수를 추가를 이쪽에서 진행
        }

        private async UniTaskVoid DownLoadLabel(string label)
        {
            _patchMap.Add(label, 0);
            var handle = Addressables.DownloadDependenciesAsync(label);
            while (false == handle.IsDone)
            {
                _patchMap[label] = handle.GetDownloadStatus().DownloadedBytes;

                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            }

            _patchMap[label] = handle.GetDownloadStatus().TotalBytes;
            Addressables.Release(handle);
        }
    }
}
