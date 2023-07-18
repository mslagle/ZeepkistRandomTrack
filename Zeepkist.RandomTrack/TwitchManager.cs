using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using UnityEngine;
using Zeepkist.RandomTrack.Repositories;

namespace Zeepkist.RandomTrack
{
    public class TwitchManager
    {
        private readonly ITwitchRepository twitchRepository;
        public bool TwitchModActive { get; set; }

        public Dictionary<string, TwitchActions> VotedActions = new Dictionary<string, TwitchActions>();
        Timer votingTimer = new Timer(30 * 1000);

        public TwitchManager(ITwitchRepository twitchRepository) 
        { 
            this.twitchRepository = twitchRepository;
            this.twitchRepository.OnMessageReceived += TwitchRepository_OnMessageReceived;

            TwitchModActive = false;

            votingTimer.Enabled = false;
            votingTimer.Elapsed += VotingTimer_Elapsed;
        }

        private void VotingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<TwitchActions> votedActions = VotedActions.Values.ToList();
            var groupedActions = votedActions.GroupBy(x => x).OrderByDescending(x => x.Count());
        }

        private void TwitchRepository_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            string message = e.ChatMessage.Message;
            if (TwitchModActive)
            {
                bool isAction = Enum.TryParse<TwitchActions>(message.Replace("!", ""), true, out TwitchActions selectedAction);
                if (isAction && !VotedActions.ContainsKey(e.ChatMessage.Username))
                {
                    VotedActions[e.ChatMessage.Username] = selectedAction;
                }
            }
        }

        public void Start()
        {
            TwitchModActive = true;

            SendStartMessage();
            SendCommands();
            SendNewVotingRound();
        }

        private void SendNewVotingRound()
        {
            this.twitchRepository.SendMessage("New voting round started, you have 30 seconds to vote for next track.");
            VotedActions.Clear();
            votingTimer.Start();
            votingTimer.Enabled=true;
        }

        private void SendCommands()
        {
            string[] actions = Enum.GetValues(typeof(TwitchActions)).Cast<TwitchActions>().Select(x => x.ToString()).ToArray();


            this.twitchRepository.SendMessage("Twitch Build Command List:");
            foreach (string action in actions) 
            {
                this.twitchRepository.SendMessage($"!{action.ToLower()}");
            }
        }

        private void SendStartMessage()
        {
            this.twitchRepository.SendMessage("Twitch Track Building is Activated");
        }
    }

    public enum TwitchActions
    {
        Straight,
        Left,
        Right,
        Booster,
        End
    }
}
