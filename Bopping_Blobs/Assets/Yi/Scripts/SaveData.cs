// Name: Yi Li
// Date: 09/26/2019
// Purpose: Manage Save Data File
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    // Store player's money
    public int MoneyInfo;

    // Store achievement information
    public List<string[]> AchievementInfo;

    // Store customization information
    public List<string[]> CustomizationInfo;
}
