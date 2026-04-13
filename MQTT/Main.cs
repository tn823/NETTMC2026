using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT
{
    public class Main
    {
        //public static MqttFactory mqttFactory;
        //public static ManagedMqttClient client;
        //public static string topic = "";
        //public static string publishtopic = "";

        //public delegate void CallBackDelegate(string message);
        //public static event CallBackDelegate CallBackEvent;

        //public static async Task<bool> Setup(string server, string user, string pass, string mytopic, string mypublishtopic)
        //{
        //    try
        //    {
        //        topic = mytopic;
        //        publishtopic = mypublishtopic;

        //        string clientId = Guid.NewGuid().ToString();
        //        int mqttPort = 1883;
        //        bool mqttSecure = false;

        //        var messageBuilder = new MqttClientOptionsBuilder()
        //          .WithClientId(clientId)
        //          .WithCredentials(user, pass)
        //          .WithTcpServer(server, mqttPort)
        //          .WithCleanSession();

        //        var options = mqttSecure
        //          ? messageBuilder
        //            .WithTls()
        //            .Build()
        //          : messageBuilder
        //            .Build();

        //        var managedOptions = new ManagedMqttClientOptionsBuilder()
        //          .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
        //          .WithClientOptions(options)
        //          .Build();

        //        client = (ManagedMqttClient)new MqttFactory().CreateManagedMqttClient();

        //        client.ConnectedAsync += Client_ConnectedAsync;
        //        client.DisconnectedAsync += Client_DisconnectedAsync;
        //        client.ConnectingFailedAsync += Client_ConnectingFailedAsync;
        //        client.ApplicationMessageReceivedAsync += Client_ApplicationMessageReceivedAsync;

        //        await client.StartAsync(managedOptions);
        //        await client.SubscribeAsync(mypublishtopic);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        //private static Task<bool> Client_ConnectingFailedAsync(ConnectingFailedEventArgs arg)
        //{
        //    Console.WriteLine("MQTT Error");
        //    return Task.FromResult(true);
        //}

        //private static Task<bool> Client_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        //{
        //    Console.WriteLine("MQTT Disconnected");
        //    return Task.FromResult(true);
        //}

        //private static Task<bool> Client_ConnectedAsync(MqttClientConnectedEventArgs arg)
        //{
        //    Console.WriteLine("MQTT Connected");
        //    return Task.FromResult(true);
        //}

        //private static Task Client_ApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        //{
        //    if (arg.ApplicationMessage.Topic.Contains(publishtopic)) TriggerCallBack(arg.ApplicationMessage.ConvertPayloadToString());

        //    return Task.FromResult(true);
        //}

        //public static void TriggerCallBack(string message)
        //{
        //    CallBackEvent?.Invoke(message);
        //}

        //public static async Task<bool> PublishAsync(string payload, bool retainFlag = true, int qos = 1)
        //{
        //    try
        //    {
        //        var applicationMessage = new MqttApplicationMessageBuilder()
        //         .WithTopic(topic)
        //                                    .WithPayload(payload)
        //                                    .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
        //                                    .WithRetainFlag(retainFlag)
        //                                    .Build();
        //        await client.EnqueueAsync(applicationMessage);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
    }
}
