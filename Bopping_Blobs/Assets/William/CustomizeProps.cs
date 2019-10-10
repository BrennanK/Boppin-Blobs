using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeProps : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private GameObject hatModel;
    private Material eyeModel;
    //private GameObject weaponModel;
    private Material skinColor;

    private void Awake()
    {
        int hatID = CustomizationManager.instance.hatIndex;
        hatModel = CustomizationManager.instance.hatModels[hatID];

        int eyeID = CustomizationManager.instance.eyeIndex;
        eyeModel = CustomizationManager.instance.eyeModels[eyeID];

        int colorID = CustomizationManager.instance.colorIndex;
        this.skinColor = CustomizationManager.instance.skinColor[colorID];

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
