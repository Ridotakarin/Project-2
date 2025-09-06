using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CropsSaveData
{

    [SerializeField] private SerializableDictionary<Vector3Int, CropData> _cropTiles;

    public SerializableDictionary<Vector3Int, CropData> CropTiles
    {
        get { return _cropTiles; }
        set { _cropTiles = value; }
    }

    public CropsSaveData()
    {
        _cropTiles = new SerializableDictionary<Vector3Int, CropData>();
    }

    public void SetCropsData(SerializableDictionary<Vector3Int, CropData> cropTiles)
    {
        CropTiles = cropTiles;
    }
}
