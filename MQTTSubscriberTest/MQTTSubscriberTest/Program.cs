
using MQTTnet;
using MQTTnet.Client;
using System.Text.Json;
using System;



/*
* This sample subscribes to a topic and processes the received message.
*/

var mqttFactory = new MqttFactory();

using (var mqttClient = mqttFactory.CreateMqttClient())
{
    var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("localhost").Build();

    // Setup message handling before connecting so that queued messages
    // are also handled properly. When there is no event handler attached all
    // received messages get lost.
    mqttClient.ApplicationMessageReceivedAsync += e =>
    {
         Console.WriteLine($"Received application message with payload: {e.ApplicationMessage.ConvertPayloadToString()}");

        //var output = "NULL";
        //if (e != null)
        //{
        //    output = JsonSerializer.Serialize(e, new JsonSerializerOptions
        //    {
        //        WriteIndented = true
        //    });
        //}

        //Console.WriteLine($"[{e?.GetType().Name}]:\r\n{output}");

        //e.DumpToConsole();

        return Task.CompletedTask;
    };

    await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

    var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
        .WithSubscriptionIdentifier(10)
        .WithTopicFilter(
            f =>
            {
                f.WithTopic("samples/temperature/living_room");
            })
        .Build();

    await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

    Console.WriteLine("MQTT client subscribed to topic.\n");

    Console.WriteLine("Press enter to exit.");
    Console.ReadLine();
}
