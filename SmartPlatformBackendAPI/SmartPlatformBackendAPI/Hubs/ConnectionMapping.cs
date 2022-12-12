using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;

namespace SmartPlatformBackendAPI.Hubs
{
    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, string> deviceConnections = new Dictionary<T, string>();
        private readonly Dictionary<T, List<string>> userConnections = new Dictionary<T, List<string>>();
        

        /// <summary>
        /// Adds a device to the connected devices
        /// </summary>
        /// <param name="key"></param>
        /// <param name="connectionId"></param>
        public void AddDeviceConnection(T key, string connectionId)
        {
            lock (deviceConnections)
            {
                if (!deviceConnections.ContainsKey(key))
                {
                    deviceConnections.Add(key, connectionId);
                }
                else
                {
                    deviceConnections[key] = connectionId;
                }
            }
        }

        /// <summary>
        /// Adds a user to the connected users
        /// </summary>
        /// <param name="key"></param>
        /// <param name="connectionId"></param>
        public void AddUserConnection(T key, string connectionId)
        {
            lock (userConnections)
            {
                List<string>? connections;
                if (!userConnections.TryGetValue(key, out connections))
                {
                    connections = new List<string>();
                    userConnections.Add(key, connections);
                }

                lock (connections)
                {
                    if (!connections.Contains(connectionId))
                    {
                        connections.Add(connectionId);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a device connection based on DeviceID
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetDeviceConnection(T key)
        {
            return deviceConnections[key];
        }

        /// <summary>
        /// Gets a user connection based on UserID
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<string> GetUserConnections(T key)
        {
            List<string>? connections;
            if (userConnections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Removes a device connection
        /// </summary>
        /// <param name="key"></param>
        /// <param name="connectionId"></param>
        public void RemoveDevieConnection(T key, string connectionId)
        {
            lock (deviceConnections)
            {
                if (deviceConnections.ContainsKey(key))
                {
                    deviceConnections.Remove(key);
                }
            }
        }

        /// <summary>
        /// Removes a user connection
        /// </summary>
        /// <param name="key"></param>
        /// <param name="connectionId"></param>
        public void RemoveUserConnection(T key, string connectionId)
        {
            lock (userConnections)
            {
                List<string>? connections;
                if (!userConnections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        userConnections.Remove(key);
                    }
                }
            }
        }

        /// <summary>
        /// Removes a connection based on connection ID only
        /// </summary>
        /// <param name="connectionID"></param>
        /// <returns></returns>
        public Task RemoveConnection(string connectionID)
        {
            lock (deviceConnections)
            {
                foreach (var connection in deviceConnections)
                {
                    if (connection.Value == connectionID)
                    {
                        deviceConnections.Remove(connection.Key);
                        return Task.CompletedTask;
                    }
                }
            }

            lock (userConnections)
            {
                foreach (var connections in userConnections)
                {
                    T key = connections.Key;
                    if (userConnections.TryGetValue(key, out List<string>? users))
                    {
                        users.Remove(connectionID);
                        return Task.CompletedTask;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
