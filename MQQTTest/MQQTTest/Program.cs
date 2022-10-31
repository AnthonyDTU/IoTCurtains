// See https://aka.ms/new-console-template for more information

using MQTTnet;
using MQTTnet.Client;
using System.Text.Json;

var mqttFactory = new MqttFactory();
using (var mqttClient = mqttFactory.CreateMqttClient())
{
    // Use builder classes where possible in this project.
    var mqttClientOptions = new MqttClientOptionsBuilder()
        .WithTcpServer("localhost")
        .Build();

    // This will throw an exception if the server is not available.
    // The result from this message returns additional data which was sent 
    // from the server. Please refer to the MQTT protocol specification for details.
    var response = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

    Console.WriteLine("The MQTT client is connected.");

    var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic("samples/temperature/living_room")
                .WithPayload("19.5")
                .Build();

    var status = await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

    var output = "NULL";
    if (status != null)
    {
        output = JsonSerializer.Serialize(status, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    Console.WriteLine($"[{status?.GetType().Name}]:\r\n{output}");
    //response.DumpToConsole();

    // Send a clean disconnect to the server by calling _DisconnectAsync_. Without this the TCP connection
    // gets dropped and the server will handle this as a non clean disconnect (see MQTT spec for details).
    var mqttClientDisconnectOptions = mqttFactory.CreateClientDisconnectOptionsBuilder().Build();

    await mqttClient.DisconnectAsync(mqttClientDisconnectOptions, CancellationToken.None);
}

