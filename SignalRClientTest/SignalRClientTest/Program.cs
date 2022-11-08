
using Microsoft.AspNetCore.SignalR.Client;



HubConnection hubConnection = new HubConnectionBuilder()
                                  .WithUrl("http://smartplatformbackendapi.azurewebsites.net/device")
                                  .WithAutomaticReconnect()
                                  .Build();

hubConnection.On<string, string>("ReceiveMessage", (deviceID, data) => 
    {
        Console.WriteLine(deviceID);
        Console.WriteLine(data);
    }
);

await hubConnection.StartAsync();
Console.WriteLine("Connected To Hub");
Console.ReadLine();


