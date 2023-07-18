using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Client;
using UnityEngine;
using UnityEngine.UIElements;

namespace Zeepkist.RandomTrack.Repositories
{
    public interface ITwitchRepository
    {
        public bool isConnected { get; set; }
        public string Username { get; set; }

        public event EventHandler<OnJoinedChannelArgs> OnJoinedChannel;
        public event EventHandler<OnMessageReceivedArgs> OnMessageReceived;

        public void SendMessage(string message);
    }

    public class TwitchRepository : ITwitchRepository
    {
        TwitchClient client = new TwitchClient();
        public bool isConnected { get; set; }
        public string Username { get; set; }

        //public delegate void OnJoinedChannel(object sender, OnJoinedChannelArgs e);
        public event EventHandler<OnJoinedChannelArgs> OnJoinedChannel;

        //public delegate void OnNewMessage(object sender, OnMessageReceivedArgs e);
        public event EventHandler<OnMessageReceivedArgs> OnMessageReceived;

        public TwitchRepository() {
            Username = Plugin.TwitchUsername.Value;
            string oauth = Plugin.TwitchApiKey.Value;

            if (string.IsNullOrEmpty(Username) )
            {
                Debug.Log("Twitch username is null or empty, not connecting to twitch.");
                isConnected = false;
                return;
            }

            if (string.IsNullOrEmpty(oauth))
            {
                Debug.Log("Twitch oauth token is null or empty, not connecting to twitch.");
                isConnected = false;
                return;
            }

            try
            {
                // https://twitchapps.com/tmi/
                ConnectionCredentials credentials = new ConnectionCredentials(Username, oauth);
                client.Initialize(credentials, Username);

                client.OnMessageReceived += Client_OnMessageReceived;
                client.OnConnected += Client_OnConnected;
                client.OnJoinedChannel += Client_OnJoinedChannel;
                client.OnLog += Client_OnLog;
                client.Connect();
            } catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Debug.Log($"TwitchLog: {e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
        {
            Debug.Log($"Connected to twitch with bot {e.BotUsername}.");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Debug.Log($"Connected to chat room ${e.Channel}");
            isConnected = true;

            OnJoinedChannel?.Invoke(this, e);
        }

        private void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            Debug.Log($"New incomming message: ${e.ChatMessage.Message}");
            OnMessageReceived?.Invoke(this, e);
        }

        public void SendMessage(string message)
        {
            client.SendMessage(Username, message);
            Debug.Log($"Sent message: ${message}");
        }
    }
}
