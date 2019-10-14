using StoreServices.Core;
using System;
using UnityEngine;

#if UNITY_ANDROID

using GooglePlayGames;
using GooglePlayGames.BasicApi;

namespace StoreServices.Android {
    public class AndroidConnection : IStoreConnection {
        public bool IsConnected {
            get {
                return PlayGamesPlatform.Instance.IsAuthenticated();
            }
        }

        public AndroidConnection() {
            PlayGamesClientConfiguration configuration = new PlayGamesClientConfiguration.Builder().Build();
            PlayGamesPlatform.InitializeInstance(configuration);
            PlayGamesPlatform.DebugLogEnabled = Debug.isDebugBuild;
            PlayGamesPlatform.Activate();
        }

        public bool Connect(Action callback = null) {
            PlayGamesPlatform.Instance.Authenticate((success) => {
                if(success) {
                    callback?.Invoke();
                }
            });

            return IsConnected;
        }

        public bool Disconnect(Action callback = null) {
            PlayGamesPlatform.Instance.SignOut();
            callback?.Invoke();
            return !IsConnected;
        }
    }
}

#endif
