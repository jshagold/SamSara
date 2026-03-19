using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Samsara.Features.Character.Domain;
using Samsara.Features.Character.MasterData;
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

        public void InitializeNewRun(string startingEvolutionNodeId, CharacterStatsSO baseStats)
        {
            _runData = new CharacterRunData
            {
                Hp = baseStats.Hp,
                Strength = baseStats.Strength,
                Toughness = baseStats.Toughness,
                Speed = baseStats.Speed,
                EvolutionNodeId = startingEvolutionNodeId,
                Day = 1,
                Gold = 0
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
