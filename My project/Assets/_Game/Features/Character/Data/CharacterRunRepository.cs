using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Samsara.Features.Character.Domain;
using UnityEngine;

namespace Samsara.Features.Character.Data
{
    public class CharacterRunRepository : ICharacterRunRepository
    {
        private readonly string _logClass = $"[{nameof(CharacterRunRepository)}]";
        private readonly string _savePath =
            Application.persistentDataPath + "/run_save.json";

        private CharacterRunData _runData;
        private bool _isDirty;

        public CharacterRunData RunData => _runData;

        public async UniTask LoadDataAsync()
        {
            await UniTask.RunOnThreadPool(() =>
            {
                if (File.Exists(_savePath))
                {
                    var json = File.ReadAllText(_savePath);
                    _runData = JsonConvert.DeserializeObject<CharacterRunData>(json);
                }
                else
                {
                    _runData = new CharacterRunData();
                }
            });
        }

        /// <summary>Dev 전용. 외부에서 dirty 플래그를 강제로 설정한다.</summary>
        public void MarkDirty() => _isDirty = true;

        public void InitializeNewRun(RunConfigSO config)
        {
            _runData = new CharacterRunData
            {
                EvolutionNodeId  = config.DefaultEvolutionNodeId.ToString(),
                Day              = config.InitialDay,
                Gold             = config.InitialGold,
                ActionPoints     = config.InitialActionPoints,
                MaxActionPoints  = config.InitialMaxActionPoints
                // Hp, MaxHp, Strength, Toughness, Agility:
                // DefaultEvolutionNodeId에 해당하는 EvolutionNodeSO.BaseStats에서 설정.
                // GlobalBootstrapper Step 4-E 참조.
            };
            _isDirty = true;
        }

        public async UniTask SaveDataAsync()
        {
            if (!_isDirty) return;
            await UniTask.RunOnThreadPool(() =>
            {
                var json = JsonConvert.SerializeObject(_runData);
                File.WriteAllText(_savePath, json);
            });
            _isDirty = false;
        }

        public void SaveDataSync()
        {
            if (!_isDirty) return;
            var json = JsonConvert.SerializeObject(_runData);
            File.WriteAllText(_savePath, json);
            _isDirty = false;
        }
    }
}
