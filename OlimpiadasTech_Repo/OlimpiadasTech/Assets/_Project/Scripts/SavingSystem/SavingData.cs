using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DataStructure
{
    public string dataName;
    public object dataValue;
}

[CreateAssetMenu(fileName = "SavingData", menuName = "Saving Data")]
public class SavingData : ScriptableObject
{
    public List<DataStructure> data = new List<DataStructure>()
    {
        new DataStructure()
        {
            dataName = "Lives",
            dataValue = 3
        },
        new DataStructure()
        {
            dataName = "Health",
            dataValue = 100f
        },
        new DataStructure()
        {
            dataName = "Position",
            dataValue = new Vector3(18f,0f,-0.2f)
},
        new DataStructure()
        {
            dataName = "Rotation",
            dataValue = Quaternion.identity
        },
        new DataStructure()
        {
            dataName = "Chips",
            dataValue = 0
        },
    };
}