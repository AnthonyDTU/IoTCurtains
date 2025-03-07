﻿using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.SignalR;


namespace SmartPlatformBackendAPI.Hubs
{
    public class DeviceHub : Hub
    {
        private readonly static ConnectionMapping<Guid> connections = new ConnectionMapping<Guid>();

        /// <summary>
        /// Used for transmitting data to a device, with the specified DeviceID
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="jsonData"></param>
        public void TransmitDataToDevice(Guid userID, Guid deviceID, string jsonData)
        {
            var connectionID = connections.GetDeviceConnection(deviceID);
            Clients.Client(connectionID).SendAsync("SetDeviceData", jsonData);

            foreach (var connection in connections.GetUserConnections(userID))
            {
                Clients.Client(connection).SendAsync("SyncPlatformState", jsonData);
            }
        }

        /// <summary>
        /// Used for transmitting a command to a device, with the specified DeviceID
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="command"></param>
        public void TransmitCommandToDevice(Guid deviceID, string command)
        {
            var connectionID = connections.GetDeviceConnection(deviceID);
            Clients.Client(connectionID).SendAsync("TransmitDeviceCommand", command);
        }

        /// <summary>
        /// Used by the device to acknowledge it has received a message
        /// </summary>
        /// <param name="userID"></param>
        public void DeviceAcknowledge(Guid userID)
        {
            foreach (var connection in connections.GetUserConnections(userID))
            {
                Clients.Client(connection).SendAsync("PassDeviceAcknowledgeToUsers", "Data recieved");
            }
        }

        /// <summary>
        /// Used by the platform to request data from a specifec device
        /// </summary>
        /// <param name="deviceID"></param>
        public void RequestDataFromDevice(Guid deviceID)
        {
            var connectionID = connections.GetDeviceConnection(deviceID);
            Clients.Client(connectionID).SendAsync("RequestDeviceData", "Please");
        }

        /// <summary>
        /// Used by the device to transmit data to the platform
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="jsonData"></param>
        public void TransmitDataFromDevice(Guid userID, Guid deviceID, string jsonData)
        {
            foreach (var connection in connections.GetUserConnections(userID))
            {
                Clients.Client(connection).SendAsync($"TransmitDeviceData{deviceID}", jsonData);
            }
        }

        /// <summary>
        /// Registers a device
        /// </summary>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public Guid RegisterDevice(Guid deviceID)
        {
            connections.AddDeviceConnection(deviceID, Context.ConnectionId);
            return deviceID;
        }

        /// <summary>
        /// Deegisters a device
        /// </summary>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public Guid DeregisterDevice(Guid deviceID)
        {
            connections.RemoveDevieConnection(deviceID, Context.ConnectionId);
            return deviceID;
        }

        /// <summary>
        /// Registers a user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public Guid RegisterUser(Guid userID)
        {
            connections.AddUserConnection(userID, Context.ConnectionId);
            return userID;
        }

        /// <summary>
        /// Deegisters a user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public Guid DeregisterUser(Guid userID)
        {
            connections.RemoveUserConnection(userID, Context.ConnectionId);
            return userID;
        }


        /// <summary>
        /// Deregisters a connection
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return connections.RemoveConnection(Context.ConnectionId);
        }
    }
}
