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

        public Dictionary<string, TrackPartType> VotedActions = new Dictionary<string, TrackPartType>();
        Timer votingTimer = new Timer(30 * 1000);

        public event EventHandler<List<TrackPartType>> OnVotedActions;

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
            List<TrackPartType> votedActions = VotedActions.Values.ToList();
            var groupedActions = votedActions.GroupBy(x => x).OrderByDescending(x => x.Count());

            int count = 0;
            List<TrackPartType> selectedActions = new List<TrackPartType>();
            foreach (var action in groupedActions)
            {
                if(count == 0)
                {
                    count = action.Count();
                    selectedActions.Add(action.Key);
                } else if (count == action.Count())
                {
                    selectedActions.Add(action.Key);
                }
            }

            if (count == 0)
            {
                twitchRepository.SendMessage("No pieces were voted on, selecting a random piece.");
            } else
            {
                twitchRepository.SendMessage($"The following pieces were voted on: {string.Join(", ", selectedActions.Select(x => x.ToString()))}");
            }

            OnVotedActions?.Invoke(this, selectedActions);

            SendNewVotingRound();
        }

        private void TwitchRepository_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            string message = e.ChatMessage.Message;
            if (TwitchModActive)
            {
                bool isAction = Enum.TryParse<TrackPartType>(message.Replace("!", ""), true, out TrackPartType selectedAction);
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
            string[] actions = Enum.GetValues(typeof(TrackPartType)).Cast<TrackPartType>().Select(x => x.ToString()).ToArray();


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

    public enum TrackPartType
    {
        Straight,
        Left,
        Right,
        Booster,
        End
    }
}
