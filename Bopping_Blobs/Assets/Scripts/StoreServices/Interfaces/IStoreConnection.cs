using System;

namespace StoreServices.Core {
    /// <summary>
    /// <para>Interface with needed methods and attributes to connect to an Online Service Store</para>
    /// </summary>
    public interface IStoreConnection {
        /// <summary>
        /// <para>Return if current player is authenticated or not</para>
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// <para>Try to stablish a connection with the store.</para>
        /// </summary>
        /// <param name="callback">Action to be executed if connection succeeds.</param>
        /// <returns>Connection success or failure</returns>
        bool Connect(Action callback = null);

        /// <summary>
        /// <para>Try to disconnect from the store</para>
        /// </summary>
        /// <param name="callback">Action to be executed if disconnection succeeds.</param>
        /// <returns>Disconnection success or failure</returns>
        bool Disconnect(Action callback = null);
    }
}
