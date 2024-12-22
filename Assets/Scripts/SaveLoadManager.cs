using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class SaveValues
{
    public Vector3 position;
    public Quaternion rotation;
    public int coins;
    public List<GameObject> inventory;

    public SaveValues(Vector3 position, Quaternion rotation, int coins, List<GameObject> inventory)
    {
        this.position = position;
        this.rotation = rotation;
        this.coins = coins;
        this.inventory = inventory;
    }
}

public class SaveLoadManager : MonoBehaviour
{
    [Header("Example")]
    public Vector3 position;
    public Quaternion rotation;
    public int coins;
    public List<GameObject> inventory;
    [Space(30)]

    public string fileName;

    private string filePath;
    private string saveKey;

    private void Start()
    {
        filePath = Application.persistentDataPath;
        filePath = $"{filePath}\\{fileName}.json";
        Debug.Log(filePath);
        if (PlayerPrefs.HasKey("Key"))
        {
            saveKey = PlayerPrefs.GetString("Key");
            Debug.Log(saveKey);
        }
        else
        {
            saveKey = GenerateKey();
            PlayerPrefs.SetString("Key", saveKey);
            Debug.Log(saveKey);
        }
    }

    public string GenerateKey()
    {
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }
    }

    public void SaveExample()
    {
        Save(new SaveValues(position, rotation, coins, inventory));
    }

    public void ShowLoadData()
    {
        SaveValues data = Load();
        Debug.Log($"Pos: {data.position}, Rot: {data.rotation}, Coins: {data.coins}");
        Debug.Log("Inventory: ");
        foreach(GameObject item in data.inventory)
        {
            Debug.Log(item.name);
        }
    }

    public void Save(SaveValues data)
    {
        File.WriteAllText(filePath, EncryptData(JsonUtility.ToJson(data)));
    }

    public SaveValues Load()
    {
        SaveValues data = new SaveValues(Vector3.zero, Quaternion.identity, 0, new List<GameObject>());
        if(File.Exists(filePath))
        {
             data = JsonUtility.FromJson<SaveValues>(DecryptData(File.ReadAllText(filePath)));
        }
        else Save(data);
        return data;
    }

    public string EncryptData(string data)
    {
        byte[] iv = new byte[16];
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(saveKey);
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(data);
                    }

                    array = memoryStream.ToArray();
                }
            }
        }

        return Convert.ToBase64String(array);
    }

    public string DecryptData(string text)
    {
        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(text);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(saveKey);
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }
}
