[System.Serializable]
public class PlayerProfile {
    public int gamesPlayed;
    public float timePlayed;
    public int timesKing;
    public float timeAsKing;
    public int timesAttackedBlobs;

    public PlayerProfile() {
        gamesPlayed = 0;
        timePlayed = 0;
        timesKing = 0;
        timeAsKing = 0;
        timesAttackedBlobs = 0;
    }

    public PlayerProfile(int _gamesPlayed, float _timePlayed, int _timesKing, float _timeAsKing, int _timesAttackedBlobs) {
        this.gamesPlayed = _gamesPlayed;
        this.timePlayed = _timePlayed;
        this.timesKing = _timesKing;
        this.timeAsKing = _timeAsKing;
        this.timesAttackedBlobs = _timesAttackedBlobs;
    }

    public void IncrementProfileData(PlayerProfile _incrementData) {
        this.gamesPlayed += _incrementData.gamesPlayed;
        this.timePlayed += _incrementData.timePlayed;
        this.timesKing += _incrementData.timesKing;
        this.timeAsKing += _incrementData.timeAsKing;
        this.timesAttackedBlobs += _incrementData.timesAttackedBlobs;
    }
}