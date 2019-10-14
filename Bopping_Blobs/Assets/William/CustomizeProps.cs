using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeProps : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject hatModel;
    [SerializeField] private Material eyeModel;
    //private GameObject weaponModel;
    [SerializeField] private Material skinColor;

    [SerializeField] private CustomizationManager customizationManager;

    private void Awake()
    {
        customizationManager = GameObject.FindObjectOfType<CustomizationManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        /*if (CustomizeData.instance)
        {
            int hatID = CustomizeData.instance.hatIndex;
            hatModel = customizationManager.hatModels[hatID];

            int eyeID = CustomizeData.instance.eyeIndex;
            eyeModel = customizationManager.eyeModels[eyeID];

            int colorID = CustomizeData.instance.colorIndex;
            this.skinColor = customizationManager.skinColor[colorID];
        }*/

        Debug.Log("hatIndex is: " + PlayerPrefs.GetInt("hatIndex"));
        Debug.Log("eyeIndex is: " + PlayerPrefs.GetInt("eyeIndex"));
        Debug.Log("colorIndex is: " + PlayerPrefs.GetInt("colorIndex"));
        hatModel = customizationManager.hatModels[PlayerPrefs.GetInt("hatIndex")];
        eyeModel = customizationManager.eyeModels[PlayerPrefs.GetInt("eyeIndex")];
        this.skinColor = customizationManager.skinColor[PlayerPrefs.GetInt("colorIndex")];

        Material[] mat = player.GetComponent<MeshRenderer>().materials;
        mat[0] = skinColor;
        mat[1] = eyeModel;
        player.GetComponent<MeshRenderer>().materials = mat;

        customizationManager.activeHat = Instantiate(hatModel, player.transform);
        customizationManager.activeHat.transform.localPosition = Vector3.zero + new Vector3(0, 0, 0);
        customizationManager.activeHat.transform.localScale = new Vector3(1, 1, 1);

    }


}
