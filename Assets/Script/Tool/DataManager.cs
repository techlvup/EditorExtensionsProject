using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections.Generic;

public class DataManager
{
    private static Dictionary<ItemType, HashSet<int>> AllItemUidLists = new Dictionary<ItemType, HashSet<int>>();

    public static T ReadBinaryData<T>(string path)
    {
        object data = null;

        if (File.Exists(path))
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                data = binaryFormatter.Deserialize(fs);
            }
        }

        return (T)data;
    }

    public static void SaveBinaryData(string path, object value)
    {
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fs, value);
        }
    }

    public static void AddItemUId(ItemType itemType, int uid)
    {
        if (!AllItemUidLists.ContainsKey(itemType))
        {
            AllItemUidLists.Add(itemType, new HashSet<int>());
        }

        HashSet<int> itemUidList = AllItemUidLists[itemType];
        bool isCanAdd = false;

        if (!itemUidList.Contains(uid))
        {
            if (itemType == ItemType.Node && uid >= 1 && uid < 100000001)
            {
                isCanAdd = true;
            }
        }

        if(isCanAdd)
        {
            itemUidList.Add(uid);
        }
    }

    public static int GetItemUId(ItemType itemType)
    {
        if(!AllItemUidLists.ContainsKey(itemType))
        {
            AllItemUidLists.Add(itemType, new HashSet<int>());
        }

        int uid = 0;
        HashSet<int> itemUidList = AllItemUidLists[itemType];

        if (itemType == ItemType.Node)
        {
            uid = Random.Range(1, 100000001);
        }

        if(!itemUidList.Add(uid))
        {
            uid = GetItemUId(itemType);
        }

        return uid;
    }

    public static void ClearItemUId(ItemType itemType)
    {
        if (AllItemUidLists.ContainsKey(itemType))
        {
            AllItemUidLists[itemType].Clear();
        }
    }

    public static void ClearItemUId()
    {
        AllItemUidLists.Clear();
    }
}