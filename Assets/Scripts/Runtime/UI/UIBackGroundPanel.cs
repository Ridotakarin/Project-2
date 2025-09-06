using System;
using System.Collections.Generic;
using UnityEngine;

public class UIBackGroundPanel : MonoBehaviour
{
    [SerializeField] private List<Texture2D> texture2Ds;
    [SerializeField] private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        Material material = meshRenderer.material;

        switch (DateTime.Now.Hour)
        {   
            case 0:
            case 1:
            case 2:
            case 3:
                material.mainTexture = texture2Ds[0];
                break;
            case 4:
                material.mainTexture = texture2Ds[1];
                break;
            case 5:
                material.mainTexture = texture2Ds[2];
                break;
            case 6:
            case 7:
            case 8:
            case 9:
            case 10:
                material.mainTexture = texture2Ds[3];
                break;
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
                material.mainTexture = texture2Ds[4];
                break;
            case 16:
            case 17:
                material.mainTexture = texture2Ds[5];
                break;
            case 18:
            case 19:
                material.mainTexture = texture2Ds[6];
                break;
            case 20:
            case 21:
            case 22:
            case 23:
                material.mainTexture = texture2Ds[7];
                break;
        }
    }
}
