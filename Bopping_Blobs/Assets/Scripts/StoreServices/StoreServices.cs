using System;
using UnityEngine;

namespace StoreServices.Core {
    public class StoreServices : MonoBehaviour {
        public static StoreServices instance;
        private IStoreConnection m_storeConnection;

        public event Action OnServicesConnected;
        public event Action OnServicesDisconnected;

        private void Awake() {
            if(instance == null) {
                instance = this;
                DontDestroyOnLoad(this);
            } else {
                Destroy(gameObject);
            }

            DetectServices();
            OnServicesConnected += ConnectedToStore;
            OnServicesDisconnected += DisconnectedFromStore;
        }

        private void DetectServices() {
            // TODO
            // if unity android, instantiate everything from android... else instantiate unity social... yeah...
        }

        #region Store Connection
        public bool IsConnected {
            get {
                return m_storeConnection.IsConnected;
            }
        }

        public void ConnectToStore() {
            Debug.Log($"Store Services: trying to connect to store...");
            m_storeConnection.Connect(OnServicesConnected);
        }

        public void DisconnectFromStore() {
            Debug.Log($"Store Services: trying to disconnect from store...");
            m_storeConnection.Disconnect(OnServicesDisconnected);
        }

        private void ConnectedToStore() {
            Debug.Log($"Connected to Store...");
        }

        private void DisconnectedFromStore() {
            Debug.Log($"Disconnected from Store...");
        }
        #endregion

        #region Achievements
        #endregion

        #region Leaderboards
        #endregion
    }
}
