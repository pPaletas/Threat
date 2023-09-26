using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingSystem : MonoBehaviour
{
    public static SavingSystem Instance;

    [SerializeField] private SavingData _savingData;

    public object LoadData(string dataName)
    {
        return _savingData.data.Find(d => d.dataName == dataName).dataValue;
    }

    public void SaveData(string dataName, object value)
    {
        int dataIndex = _savingData.data.FindIndex(d => d.dataName == dataName);

        DataStructure newData = _savingData.data[dataIndex];
        newData.dataValue = value;

        _savingData.data[dataIndex] = newData;
    }

    public void ResetData()
    {
        Resources.UnloadAsset(_savingData);
        _savingData = ScriptableObject.CreateInstance("SavingData") as SavingData;
    }

    private void Awake()
    {
        Instance = this;
    }   
}