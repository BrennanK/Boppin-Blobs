using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeData : MonoBehaviour
{
    public static CustomizeData instance;

    //public GameObject hatModel;
    public int hatIndex;
    //public Material eyeModel;
    public int eyeIndex;
    //public GameObject weaponModel;
    //public Material skinColor;
    public int colorIndex;

    private void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
