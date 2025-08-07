using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Security.Cryptography;

[Serializable]
public class Transport
{
    public GameObject prefab;
    public Vector3 position;
    public Quaternion rotation;
    public List<string> inventory;
}

[Serializable]
public class SaveData
{
    public GameObject player;
    public List<string> inventory;
    public List<Transport> transports;
    public int coins;
}

public class SaveLoadManager2 : MonoBehaviour
{
    public SaveData data;

    private string path;
    private string saveKey;

    private void Start()
    {
        path = Application.persistentDataPath + "\\Save.json";
        if(!PlayerPrefs.HasKey("Key"))
        {
            saveKey = GenerateKey();
            PlayerPrefs.SetString("Key", saveKey);
        }
        else
        {
            saveKey = PlayerPrefs.GetString("Key");
        }
        Debug.Log(saveKey);
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

    [ContextMenu("Save")]
    public void Save()
    {
        Debug.Log("File saved in " + path);
        File.WriteAllText(path, EncryptData(JsonUtility.ToJson(data)));
    }

    [ContextMenu("Load")]
    public void Load()
    {
        Debug.Log("Succesfully Loaded file from " + path);
        data = JsonUtility.FromJson<SaveData>(DecryptData(File.ReadAllText(path)));
        foreach(var transport in data.transports)
        {
            Instantiate(transport.prefab, transport.position, transport.rotation);
        }
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
