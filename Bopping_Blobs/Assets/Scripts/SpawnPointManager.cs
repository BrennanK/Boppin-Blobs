using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour {
    private List<TaggingIdentifier> m_players = new List<TaggingIdentifier>();
    private List<GameObject> m_startPoints = new List<GameObject>();

    private void Start() {
        m_startPoints = GameObject.FindGameObjectsWithTag("SpawnPoint").ToList();
        m_players = GameObject.FindObjectsOfType<TaggingIdentifier>().ToList();

        if(m_startPoints.Count == 0) {
            Debug.LogError($"There are no Start Points in the scene.");
        }
        SpawnPlayers();
    }

    private void SpawnPlayers() {
        List<GameObject> tempStartPoints = new List<GameObject>(m_startPoints);

        foreach (TaggingIdentifier player in m_players) {
            var rand = Random.Range(0, tempStartPoints.Count);

            player.gameObject.transform.position = tempStartPoints[rand].transform.position;
            tempStartPoints.RemoveAt(rand);
        }
    }

    /// <summary>
    /// <para>Respawn all players in the scene.</para>
    /// </summary>
    /// <param name="_timeScaleToSet">Set timescale after respawning all players. Default to 1.0</param>
    public void RespawnAllPlayers(float _timeScaleToSet = 1.0f) {
        SpawnPlayers();
        Time.timeScale = _timeScaleToSet;
    }
}