using System;
using StoreServices.Core;
using UnityEngine;

namespace StoreServices.UnitySocial {
    /// <summary>
    /// <para>Unity Social Connection implements Unity Social functions, usually it is used to target iOS Store.</para>
    /// </summary>
    public class UnitySocialConnection : IStoreConnection {
        public bool IsConnected {
            get {
                return Social.localUser.authenticated;
            }
        }

        public bool Connect(Action callback = null) {
            if(IsConnected) {
                return false;
            }

            Social.localUser.Authenticate((success) => {
                if(success) {
                    callback?.Invoke();
                }
            });

            return IsConnected;
        }

        public bool Disconnect(Action callback = null) {
            // iOS doesn't allow to disconnect
            return false;
        }
    }
}
