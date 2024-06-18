using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DataPlayer : MonoBehaviour
{
    string saveName = "SaveData";
    [SerializeField] WeaponSO[] weaponDatas;
    [SerializeField] SetSO[] setDatas;

    void SaveData(string dataToSave)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, saveName);
        File.WriteAllText(fullPath, dataToSave);
    }
    string LoadData()
    {
        string data = "";
        var fullPath = Path.Combine(Application.persistentDataPath, saveName);
        data = File.ReadAllText(fullPath);
        return data;
    }
    public void SaveGame()
    {
        SaveDataPlayer data = new SaveDataPlayer();
        var dataToSave = JsonUtility.ToJson(data);
        SaveData(dataToSave);
    }
    public SaveDataPlayer LoadGame()
    {
        string dataToLoad = "";
        dataToLoad = LoadData();
        if (String.IsNullOrEmpty(dataToLoad) == false)
        {
            SaveDataPlayer data = JsonUtility.FromJson<SaveDataPlayer>(dataToLoad);
            return data;
        }
        return null;
    }
    
    public WeaponSO GetWeaponData(int index)
    {
        foreach (WeaponSO w in weaponDatas)
        {
            if (w.index == index)
            {
                return w;
            }
        }
        return null;
    }
    public SetSO GetSetData(int index)
    {
        foreach (SetSO s in setDatas)
        {
            if (s.index == index)
            {
                return s;
            }
        }
        return null;
    }

    [Serializable]
    public class SaveDataPlayer
    {
        [SerializeField] string namePlayer;
        [SerializeField] int gold, indexWeaponOpen, indexWeaponCur;
        [SerializeField] int indexHairCur, indexPantCur, indexShieldCur, indexSetCur;
        [SerializeField] List<int> hairsBought, pantsBought, shieldsBought, setsBought;
        

        public SaveDataPlayer()
        {
            gold = GameManager.Instance.GetGold();
            namePlayer = GameManager.Instance.GetPlayer().GetNamePlayer();
            indexWeaponOpen = GameManager.Instance.GetPlayer().GetIndexWeaponOpen();
            indexWeaponCur = GameManager.Instance.GetPlayer().GetIndexWeaponCur();
            indexHairCur = GameManager.Instance.GetPlayer().GetHairCur();
            hairsBought = GameManager.Instance.GetPlayer().GetHairsBought();
            indexPantCur = GameManager.Instance.GetPlayer().GetPantCur();
            pantsBought = GameManager.Instance.GetPlayer().GetPantsBought();
            indexShieldCur = GameManager.Instance.GetPlayer().GetShieldCur();
            shieldsBought = GameManager.Instance.GetPlayer().GetShieldsBought();
            indexSetCur = GameManager.Instance.GetPlayer().GetSetCur();
            setsBought = GameManager.Instance.GetPlayer().GetSetsBought();
        }

        public string GetNamePlayer()
        {
            return namePlayer;
        }
        public int GetGold()
        {
            return gold;
        }
        public int GetIndexWeaponOpen()
        {
            return indexWeaponOpen;
        }
        public int GetIndexWeaponCur()
        {
            return indexWeaponCur;
        }
        public List<int> GetHairBought()
        {
            return hairsBought;
        }
        public List<int> GetPantBought()
        {
            return pantsBought;
        }
        public List<int> GetShieldBought()
        {
            return shieldsBought;
        }
        public List<int> GetSetBought()
        {
            return setsBought;
        }
        public int GetIndexHairCur()
        {
            return indexHairCur;
        }
        public int GetIndexPantCur()
        {
            return indexPantCur;
        }
        public int GetIndexShieldCur()
        {
            return indexShieldCur;
        }
        public int GetIndexSetCur()
        {
            return indexSetCur;
        }
    }
}
