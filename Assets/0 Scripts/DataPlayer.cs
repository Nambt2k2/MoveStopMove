using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DataPlayer : MonoBehaviour
{
    string saveName = "SaveData";
    string saveName1 = "SaveData1";
    [SerializeField] WeaponSO[] weaponDatas;
    [SerializeField] SetSO[] setDatas;

    void SaveData(string dataToSave, string s)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, s);
        File.WriteAllText(fullPath, dataToSave);
    }
    string LoadData(string s)
    {
        string data = "";
        var fullPath = Path.Combine(Application.persistentDataPath, s);
        data = File.ReadAllText(fullPath);
        return data;
    }
    public void SaveGame()
    {
        SaveDataPlayer data = new SaveDataPlayer();
        var dataToSave = JsonUtility.ToJson(data);
        SaveData(dataToSave, saveName);
    }
    public void SaveColorWeeaponCustom()
    {
        SaveDataColorWeeaponCustom data = new SaveDataColorWeeaponCustom();
        var dataToSave = JsonUtility.ToJson(data);
        SaveData(dataToSave, saveName1);
    }
    public SaveDataPlayer LoadGame()
    {
        string dataToLoad = "";
        dataToLoad = LoadData(saveName);
        if (String.IsNullOrEmpty(dataToLoad) == false)
        {
            SaveDataPlayer data = new SaveDataPlayer();
            data = JsonUtility.FromJson<SaveDataPlayer>(dataToLoad);
            return data;
        }
        return null;
    }
    public SaveDataColorWeeaponCustom LoadColorWeeaponCustom()
    {
        string dataToLoad = "";
        dataToLoad = LoadData(saveName1);
        if (String.IsNullOrEmpty(dataToLoad) == false)
        {
            SaveDataColorWeeaponCustom data = JsonUtility.FromJson<SaveDataColorWeeaponCustom>(dataToLoad);
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
        [SerializeField] int gold, indexWeaponOpen, indexWeaponCur, indexSkinWeaponCur;
        [SerializeField] int indexHairCur, indexPantCur, indexShieldCur, indexSetCur;
        [SerializeField] List<int> hairsBought, pantsBought, shieldsBought, setsBought;

        public SaveDataPlayer()
        {
            if (GameManager.Instance == null)
            {
                return;
            }
            gold = GameManager.Instance.Gold;
            namePlayer = GameManager.Instance.Player.NamePlayer;
            indexWeaponOpen = GameManager.Instance.Player.IndexWeaponOpen;
            indexWeaponCur = GameManager.Instance.Player.IndexWeaponCur;
            indexHairCur = GameManager.Instance.Player.HairCur;
            hairsBought = GameManager.Instance.Player.HairsBought;
            indexPantCur = GameManager.Instance.Player.PantCur;
            pantsBought = GameManager.Instance.Player.PantsBought;
            indexShieldCur = GameManager.Instance.Player.ShieldCur;
            shieldsBought = GameManager.Instance.Player.ShieldsBought;
            indexSetCur = GameManager.Instance.Player.SetCur;
            setsBought = GameManager.Instance.Player.SetsBought;
            indexSkinWeaponCur = GameManager.Instance.IndexSkinWeaponCur;
        }

        public string NamePlayer
        {
            get
            {
                return namePlayer;
            }
        }
        public int Gold
        {
            get
            {
                return gold;
            }
        }
        public int IndexWeaponOpen
        {
            get
            {
                return indexWeaponOpen;
            }
        }
        public int IndexWeaponCur
        {
            get
            {
                return indexWeaponCur;
            } 
        }
        public List<int> HairsBought
        {
            get
            {
                return hairsBought;
            }
        }
        public List<int> PantsBought
        {
            get
            {
                return pantsBought;
            }
        }
        public List<int> ShieldsBought
        {
            get
            {
                return shieldsBought;
            }
        }
        public List<int> SetsBought
        {
            get
            {
                return setsBought;
            }
        }
        public int IndexHairCur
        {
            get
            {
                return indexHairCur;
            }
        }
        public int IndexPantCur
        {
            get
            {
                return indexPantCur;
            }
        }
        public int IndexShieldCur
        {
            get
            {
                return indexShieldCur;
            }
        }
        public int IndexSetCur
        {
            get
            {
                return indexSetCur;
            }
        }
        public int IndexSkinWeaponCur
        {
            get
            {
                return indexSkinWeaponCur;
            }
        }
    }

    [Serializable]
    public class SaveDataColorWeeaponCustom
    {
        [SerializeField] List<int> weaponColorCustoms;

        public SaveDataColorWeeaponCustom()
        {
            weaponColorCustoms = GameManager.Instance.WeaponCustoms;
        }

        public List<int> WeaponColorCustoms
        {
            get
            {
                return weaponColorCustoms;
            }
        }
    }
}
