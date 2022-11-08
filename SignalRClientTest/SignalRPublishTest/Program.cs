
using Microsoft.AspNetCore.SignalR.Client;



HubConnection hubConnection = new HubConnectionBuilder()
                                  .WithUrl("http://smartplatformbackendapi.azurewebsites.net/device")
                                  .WithAutomaticReconnect()
                                  .Build();

await hubConnection.StartAsync();

hubConnection.SendAsync("SendMessage", "TestDevice", "Some Test Data");
Console.WriteLine("Message Sent to Hub");
