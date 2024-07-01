using AC.Attribute;
using AC.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace AC.GameTool.SaveData
{
    public class SaveManager : Singleton<SaveManager>
    {
        private static readonly string saveFileName = "DhgkrYryujvPiaSGLLXpaUuTBfTQICUD";
        private GameData _gameData;
        //[SerializeField] IntGameEvent _coinChangeEvent;
        [ReadOnlly]
        public CheckLoadCompleted CheckLoadCompleted;
        protected override void Awake()
        {
            base.Awake();
            CheckLoadCompleted = new CheckLoadCompleted();
            LoadSaveData();
        }

        public GameData GameData => _gameData;

        public void SaveGameData()
        {
            SaveGameDataToFile(GameData);
        }

        public void LoadSaveData()
        {
            _gameData = LoadGameDataFromFile();
            if (!_gameData.IsNotFirstGame)
            {
                _gameData.Coin = 0;
                _gameData.IsNotFirstGame = true;
                _gameData.Music = true;
                _gameData.Sound = true;
            }
            SaveGameData();
            CheckLoadCompleted.IsLoadCompleted = true;
        }
        public void ClearSaveData()
        {
            DeleteSaveDataFile();
            _gameData = new GameData();
        }

        //public void ChangeCoin(int coin)
        //{            
        //    //Debug.Log("============Change Coin===============");
        //    coin = coin < 0 ? 0 : coin;
        //    _gameData.Coin = coin;
        //    SaveGameData();
        //    if(_coinChangeEvent != null) 
        //        _coinChangeEvent.RaiseEvent(coin);
        //}
            

        #region Read/Write File
        public static void SaveGameDataToFile(GameData gameData)
        {
            // Mã hóa dữ liệu thành dạng nhị phân
            byte[] serializedData = SerializeData(gameData);

            // Lưu dữ liệu vào tệp
            SaveToFile(serializedData);
        }

        public static GameData LoadGameDataFromFile()
        {
            // Đọc dữ liệu từ tệp
            byte[] serializedData = LoadFromFile();

            GameData gameData = new GameData();
            if (serializedData != null)
            {
                // Giải mã dữ liệu từ dạng nhị phân
                gameData = DeserializeData(serializedData);
            }
            return gameData;
        }

        private static byte[] SerializeData(GameData gameData)
        {
            // Sử dụng BinaryFormatter để mã hóa dữ liệu thành dạng nhị phân
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream();
            formatter.Serialize(memoryStream, gameData);
            byte[] serializedData = memoryStream.ToArray();
            memoryStream.Close();

            return serializedData;
        }

        private static GameData DeserializeData(byte[] serializedData)
        {
            // Sử dụng BinaryFormatter để giải mã dữ liệu từ dạng nhị phân
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream memoryStream = new MemoryStream(serializedData);
            GameData gameData = (GameData)formatter.Deserialize(memoryStream);
            memoryStream.Close();

            return gameData;
        }

        private static void SaveToFile(byte[] data)
        {
            string saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
            // Lưu dữ liệu vào tệp
            FileStream fileStream = new FileStream(saveFilePath, FileMode.Create);
            fileStream.Write(data, 0, data.Length);
            fileStream.Close();
        }

        private static byte[] LoadFromFile()
        {
            string saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
            // Đọc dữ liệu từ tệp
            if (File.Exists(saveFilePath))
            {
                FileStream fileStream = new FileStream(saveFilePath, FileMode.Open);
                byte[] data = new byte[fileStream.Length];
                fileStream.Read(data, 0, data.Length);
                fileStream.Close();

                return data;
            }
            else
            {
                Debug.LogWarning("Save file not found.");
                return null;
            }
        }

        public static void DeleteSaveDataFile()
        {
            string saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
            if (File.Exists(saveFilePath))
            {
                File.Delete(saveFilePath);
            }
        }
        #endregion
    }
}


