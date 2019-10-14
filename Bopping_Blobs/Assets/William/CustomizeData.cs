using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeData : MonoBehaviour
{
    public static CustomizeData instance;

    public int hatIndex;
    public int eyeIndex;
    public int colorIndex;

    private void Awake()
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
