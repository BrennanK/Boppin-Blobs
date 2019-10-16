// Name: Yi Li
// Date: 09/26/2019
// Purpose: Manage Save Data File
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public PlayerProfile playerProfile;

    // Store player's money
    public int MoneyInfo;

    // Store achievement information
    public List<string[]> AchievementInfo;

    // Store customization information
    public List<string[]> CustomizationInfo;

    public SaveData() {
        playerProfile = new PlayerProfile();
    }
}
