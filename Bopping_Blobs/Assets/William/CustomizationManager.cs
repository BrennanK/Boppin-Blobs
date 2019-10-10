using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationManager : MonoBehaviour
{
    enum ApearanceDetail
    {
        HAT,
        EYE,
        WEAPON,
        SKIN_COLOR
    }

    [SerializeField] private GameObject player;

    [SerializeField] private GameObject[] hatModels;
    [SerializeField] private int hatIndex;
    [SerializeField] private GameObject activeHat;

    [SerializeField] private Material[] eyeModels;
    [SerializeField] private int eyeIndex;
    [SerializeField] private Material activeEye;

    [SerializeField] private GameObject[] weaponModels;
    [SerializeField] private int weaponIndex;
    [SerializeField] private GameObject activeWeapon;

    [SerializeField] private Material[] skinColor;
    [SerializeField] private int colorIndex;

    private void ApplyModification(ApearanceDetail detail, int id)
    {
        switch (detail)
        {
            case ApearanceDetail.HAT:
                if (activeHat != null)
                {
                    Destroy(activeHat);
                }
                activeHat = Instantiate(hatModels[id], player.transform);
                activeHat.transform.localPosition = Vector3.zero + new Vector3(0,0.75f,0);
                break;
            case ApearanceDetail.EYE:
                if (activeEye != null)
                {
                    Destroy(activeEye);
                }
                activeEye = eyeModels[id];
                player.GetComponent<MeshRenderer>().material = activeEye;

                //ApplyModification(ApearanceDetail.SKIN_COLOR, id);

                break;
            case ApearanceDetail.WEAPON:

                break;
            case ApearanceDetail.SKIN_COLOR:

                player.GetComponent<MeshRenderer>().material = skinColor[id];
                break;
        }
    }

    public void NextHat()
    {
        if (hatIndex < hatModels.Length -1)
        {
            hatIndex++;
        }
        else
        {
            hatIndex = 0;
        }

        ApplyModification(ApearanceDetail.HAT, hatIndex);
    }

    public void PreviousHat()
    {
        if (hatIndex > 0)
        {
            hatIndex--;
        }
        else
        {
            hatIndex = hatModels.Length - 1;
        }

        ApplyModification(ApearanceDetail.HAT, hatIndex);
    }

    public void NextEye()
    {
        if (eyeIndex < eyeModels.Length - 1)
        {
            eyeIndex++;
        }
        else
        {
            eyeIndex = 0;
        }

        ApplyModification(ApearanceDetail.EYE, eyeIndex);
    }

    public void PreviousEye()
    {
        if (eyeIndex > 0)
        {
            eyeIndex--;
        }
        else
        {
            eyeIndex = eyeModels.Length - 1;
        }

        ApplyModification(ApearanceDetail.EYE, eyeIndex);
    }

    public void NextWeapon()
    {
        if (weaponIndex < weaponModels.Length - 1)
        {
            weaponIndex++;
        }
        else
        {
            weaponIndex = 0;
        }

        ApplyModification(ApearanceDetail.WEAPON, weaponIndex);
    }

    public void PreviousWeapon()
    {
        if (weaponIndex > 0)
        {
            weaponIndex--;
        }
        else
        {
            weaponIndex = weaponModels.Length - 1;
        }

        ApplyModification(ApearanceDetail.WEAPON, weaponIndex);
    }

    public void NextColor()
    {
        /*if (colorIndex < skinColor.Length - 1)
        {
            colorIndex++;
        }
        else
        {
            colorIndex = 0;
        }

        ApplyModification(ApearanceDetail.SKIN_COLOR, colorIndex);*/
        switch (eyeIndex)
        {
            case 0:
                if (colorIndex > 0 && colorIndex < 4)
                {
                    colorIndex++;
                }
                else if (colorIndex < 0)
                {
                    colorIndex = 0;
                }
                else
                {
                    colorIndex = 0;
                }
                break;
            case 1:
                if (colorIndex > 4 && colorIndex < 9)
                {
                    colorIndex++;
                }
                else if (colorIndex < 4)
                {
                    colorIndex = 5;
                }
                else
                {
                    colorIndex = 5;
                }
                break;
            case 2:
                if (colorIndex > 9 && colorIndex < 14)
                {
                    colorIndex++;
                }
                else if (colorIndex < 9)
                {
                    colorIndex = 10;
                }
                else
                {
                    colorIndex = 10;
                }
                break;
            case 3:
                if (colorIndex > 14 && colorIndex < 19)
                {
                    colorIndex++;
                }
                else if (colorIndex < 14)
                {
                    colorIndex = 15;
                }
                else
                {
                    colorIndex = 15;
                }
                break;
            case 4:
                if (colorIndex > 19 && colorIndex < 24)
                {
                    colorIndex++;
                }
                else if (colorIndex < 19)
                {
                    colorIndex = 20;
                }
                else
                {
                    colorIndex = 20;
                }
                break;
        }
        ApplyModification(ApearanceDetail.SKIN_COLOR, colorIndex);
    }

    public void PreviousColor()
    {
        /*if (colorIndex > 0)
        {
            colorIndex--;
        }
        else
        {
            colorIndex = skinColor.Length - 1;
        }

        ApplyModification(ApearanceDetail.SKIN_COLOR, colorIndex);*/

        switch (eyeIndex)
        {
            case 0:
                if (colorIndex > 0 && colorIndex < 5)
                {
                    colorIndex--;
                }
                else if (colorIndex < 0)
                {
                    colorIndex = 4;
                }
                else
                {
                    colorIndex = 0;
                }
                break;
            case 1:
                if (colorIndex > 5 && colorIndex < 10)
                {
                    colorIndex--;
                }
                else if (colorIndex < 5)
                {
                    colorIndex = 9;
                }
                else
                {
                    colorIndex = 5;
                }
                break;
            case 2:
                if (colorIndex > 10 && colorIndex < 15)
                {
                    colorIndex--;
                }
                else if (colorIndex < 10)
                {
                    colorIndex = 14;
                }
                else
                {
                    colorIndex = 10;
                }
                break;
            case 3:
                if (colorIndex > 15 && colorIndex < 20)
                {
                    colorIndex--;
                }
                else if (colorIndex < 15)
                {
                    colorIndex = 19;
                }
                else
                {
                    colorIndex = 15;
                }
                break;
            case 4:
                if (colorIndex > 20 && colorIndex < 25)
                {
                    colorIndex--;
                }
                else if (colorIndex < 20)
                {
                    colorIndex = 24;
                }
                else
                {
                    colorIndex = 20;
                }
                break;
        }
        ApplyModification(ApearanceDetail.SKIN_COLOR, colorIndex);
    }
}
