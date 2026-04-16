using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Samsara.Features.Character.Domain;
using UnityEngine;

namespace Samsara.Features.Character.Data
{
    public class CharacterAccountRepository : ICharacterAccountRepository
    {
        private readonly string _logClass = $"[{nameof(CharacterAccountRepository)}]";
        private readonly string _savePath =
            Application.persistentDataPath + "/account_save.json";

        private CharacterAccountData _accountData;
        private bool _isDirty;

        public CharacterAccountData AccountData => _accountData;

        public async UniTask LoadDataAsync()
        {
            await UniTask.RunOnThreadPool(() =>
            {
                if (File.Exists(_savePath))
                {
                    var json = File.ReadAllText(_savePath);
                    _accountData = JsonConvert.DeserializeObject<CharacterAccountData>(json);
                }
                else
                {
                    _accountData = new CharacterAccountData();
                }
            });
        }

        public void UnlockEvolutionNode(string nodeId)
        {
            if (_accountData.UnlockedEvolutionNodeIds.Contains(nodeId)) return;
            _accountData.UnlockedEvolutionNodeIds.Add(nodeId);
            _isDirty = true;
        }

        public void RegisterCodex(string nodeId)
        {
            if (_accountData.CompletedCodexIds.Contains(nodeId)) return;
            _accountData.CompletedCodexIds.Add(nodeId);
            _isDirty = true;
        }

        public void MarkDirty()
        {
            _isDirty = true;
        }

        public async UniTask SaveDataAsync()
        {
            if (!_isDirty) return;
            await UniTask.RunOnThreadPool(() =>
            {
                var json = JsonConvert.SerializeObject(_accountData);
                File.WriteAllText(_savePath, json);
            });
            _isDirty = false;
        }

        public void SaveDataSync()
        {
            if (!_isDirty) return;
            var json = JsonConvert.SerializeObject(_accountData);
            File.WriteAllText(_savePath, json);
            _isDirty = false;
        }
    }
}
