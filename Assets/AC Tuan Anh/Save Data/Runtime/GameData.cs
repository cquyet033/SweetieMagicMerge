using AC.Attribute;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AC.GameTool.SaveData
{
    [System.Serializable]
    public class GameData
    {
        public bool IsNotFirstGame;
        public string Ver;
        public string PlayerName;
        public int Level;
        public int Coin;
        public int Exp;

        public bool Music;
        public bool Vibration;
        public bool Sound;

        public long TodayTicks;
    }
}

