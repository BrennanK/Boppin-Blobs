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

        int hatID = CustomizeData.instance.hatIndex;
        hatModel = customizationManager.hatModels[hatID];

        int eyeID = CustomizeData.instance.eyeIndex;
        eyeModel = customizationManager.eyeModels[eyeID];

        int colorID = CustomizeData.instance.colorIndex;
        this.skinColor = customizationManager.skinColor[colorID];

    }

    // Start is called before the first frame update
    void Start()
    {
        Material[] mat = player.GetComponent<MeshRenderer>().materials;
        mat[0] = eyeModel;
        mat[1] = skinColor;
        player.GetComponent<MeshRenderer>().materials = mat;

        GameObject hat = Instantiate(hatModel, player.transform);
        hat.transform.localPosition = Vector3.zero + new Vector3(0, 0.42f, 0);

    }


}
