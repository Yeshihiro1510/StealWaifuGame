using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Project.Source
{
    public static class PlayerProgressStorage
    {
        private const string SaveFileName = "progress_data.json";

        public static ProgressData Data { get; private set; }

        static PlayerProgressStorage()
        {
            if (TryLoad(out var loadedData))
            {
                Data = new ProgressData(
                    new CoinsModel(loadedData.Coins),
                    new MagnetModel(loadedData.MagnetUnlocked, loadedData.MagnetLevel));
            }

            typeof(MagnetModel).GetField("_coins", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(Data.Magnet, Data.Coins);
        }

        public static void Save()
        {
            var saveFile = new SaveFile
            {
                Coins = Data.Coins.Value,
                MagnetLevel = Data.Magnet.Level,
                MagnetUnlocked = Data.Magnet.Unlocked
            };
            var json = JsonUtility.ToJson(saveFile);
            var path = Path.Combine(Application.persistentDataPath, SaveFileName);
            File.WriteAllText(path, json);
            Debug.Log($"Progress saved to {path}.");
        }

        private static bool TryLoad(out SaveFile result)
        {
            var path = Path.Combine(Application.persistentDataPath, SaveFileName);
            if (File.Exists(path))
            {
                result = JsonUtility.FromJson<SaveFile>(File.ReadAllText(path));
                Debug.Log($"Progress loaded from {Path.Combine(Application.persistentDataPath, SaveFileName)}.");
                return true;
            }

            result = default;
            return false;
        }
    }

    // FUCK THIS SHIT, I'LL JUST DO THIS WAY
    [Serializable]
    public struct SaveFile
    {
        public int Coins;
        public int MagnetLevel;
        public bool MagnetUnlocked;
    }

    [Serializable]
    public struct ProgressData
    {
        public CoinsModel Coins;
        public MagnetModel Magnet;

        public ProgressData(CoinsModel coins, MagnetModel magnet)
        {
            Coins = coins;
            Magnet = magnet;
        }
    }

    [Serializable]
    public class CoinsModel
    {
        private int _value;

        public int Value => _value;

        public event Action<int> changed;

        public CoinsModel(int value)
        {
            _value = value;
        }

        public bool Collect(CoinController coin)
        {
            if (coin.isActiveAndEnabled && !coin.Collected)
            {
                coin.Collect();
                _value++;
                changed?.Invoke(Value);
                return true;
            }

            return false;
        }

        public bool TryPurchase(int cost)
        {
            if (Value >= cost)
            {
                _value -= cost;
                changed?.Invoke(Value);
                return true;
            }

            return false;
        }
    }

    [Serializable]
    public class MagnetModel
    {
        private readonly Levels _levels;
        private readonly CoinsModel _coins;

        private bool _unlocked;
        private int _level;

        public bool Unlocked => _unlocked;
        public int Level => _level;
        public int MaxLevel => _levels.Length - 1;

        public float Radius
        {
            get
            {
                if (Level <= _levels.Length - 1)
                    return _levels[Level].Radius;

                return _levels[0].Radius * Mathf.Exp(Level);
            }
        }

        public int UnlockCost => _levels[0].Cost;

        public int UpgradeCost
        {
            get
            {
                if (Level < _levels.Length - 1)
                    return _levels[Level + 1].Cost;

                return Mathf.RoundToInt(_levels[0].Cost * Mathf.Exp(Level + 1));
            }
        }

        public bool IsFinalLevel => Level == _levels.Length - 1;

        public event Action<MagnetModel> changed;

        public MagnetModel(bool unlocked, int level)
        {
            _levels = JsonUtility.FromJson<Levels>(Resources.Load<TextAsset>("magnet_levels").text);
            _unlocked = unlocked;
            _level = level;
        }

        public bool Unlock()
        {
            if (!Unlocked && _coins.TryPurchase(UnlockCost))
            {
                _unlocked = true;
                _level = 0;
                changed?.Invoke(this);
                return true;
            }

            return false;
        }

        public bool Upgrade()
        {
            if (Unlocked && _coins.TryPurchase(UpgradeCost))
            {
                _level++;
                changed?.Invoke(this);
                return true;
            }

            return false;
        }

        [Serializable]
        public struct Levels
        {
            public Level[] Data;

            public int Length => Data?.Length ?? 0;
            public Level this[int i] => Data[i];

            public Levels(Level[] data)
            {
                Data = data;
            }

            [Serializable]
            public struct Level
            {
                public float Radius;
                public int Cost;
            }
        }
    }
}