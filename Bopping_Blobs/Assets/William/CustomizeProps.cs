using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomizeProps : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject hatModel;
    [SerializeField] private Material eyeModel;
    [SerializeField] private GameObject weaponModel;
    [SerializeField] private Material skinColor;

    [SerializeField] private CustomizationManager customizationManager;

    private void Awake()
    {
        customizationManager = GameObject.FindObjectOfType<CustomizationManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        hatModel = customizationManager.hatModels[PlayerPrefs.GetInt("hatIndex")];
        eyeModel = customizationManager.eyeModels[PlayerPrefs.GetInt("eyeIndex")];
        weaponModel = customizationManager.weaponModels[PlayerPrefs.GetInt("weaponIndex")];
        skinColor = customizationManager.skinColor[PlayerPrefs.GetInt("colorIndex")];

        Material[] mat = player.GetComponent<MeshRenderer>().materials;
        mat[0] = skinColor;
        mat[1] = eyeModel;
        player.GetComponent<MeshRenderer>().materials = mat;

        customizationManager.activeHat = Instantiate(hatModel, player.transform);
        customizationManager.activeHat.transform.localPosition = Vector3.zero + new Vector3(0, 0, 0);
        customizationManager.activeHat.transform.localScale = new Vector3(1, 1, 1);

        // Sets the weapon place according to different scenes
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            customizationManager.activeWeapon = Instantiate(weaponModel, player.transform.parent.parent);
            customizationManager.activeWeapon.transform.localPosition = Vector3.left;
        }
        else
        {
            // customizationManager.activeWeapon = Instantiate(weaponModel, player.transform.parent.parent);
            // customizationManager.activeWeapon.transform.localPosition = Vector3.right;
        }
    }
}
