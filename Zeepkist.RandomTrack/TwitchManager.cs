using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace Zeepkist.RandomTrack
{
    public class TwitchManager
    {
        public TwitchManager() { }

        public void Start()
        {
            ConnectionCredentials credentials = new ConnectionCredentials("", "");
            TwitchClient client = new TwitchClient();
            client.Initialize(credentials, "");

            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnConnected += Client_OnConnected;
            client.OnLog += Client_OnLog;
            client.Connect();
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            UnityEngine.Debug.Log($"Connected to twitch with bot {e.BotUsername} to channel {e.AutoJoinChannel}");
        }

        private void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            UnityEngine.Debug.Log("New incomming message" + e.ChatMessage.Message);
        }
    }
}
