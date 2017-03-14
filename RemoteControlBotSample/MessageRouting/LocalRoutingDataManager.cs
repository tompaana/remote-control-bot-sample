using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoteControlBotSample.MessageRouting
{
    /// <summary>
    /// Routing data manager that stores the data locally.
    /// 
    /// NOTE: USE THIS CLASS ONLY FOR TESTING! Storing the data like this in production would
    /// not work since the bot may have multiple instances.
    /// 
    /// See IRoutingDataManager for general documentation of properties and methods.
    /// </summary>
    [Serializable]
    public class LocalRoutingDataManager : IRoutingDataManager
    {
        /// <summary>
        /// Parties that are users (not this bot).
        /// </summary>
        protected IList<Party> UserParties
        {
            get;
            set;
        }

        /// <summary>
        /// If the bot is addressed from different channels, its identity in terms of ID and name
        /// can vary. Those different identities are stored in this list.
        /// </summary>
        protected IList<Party> BotParties
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public LocalRoutingDataManager()
        {
            UserParties = new List<Party>();
            BotParties = new List<Party>();
        }

        public virtual IList<Party> GetUserParties()
        {
            List<Party> userPartiesAsList = UserParties as List<Party>;
            return userPartiesAsList?.AsReadOnly();
        }

        public virtual IList<Party> GetBotParties()
        {
            List<Party> botPartiesAsList = BotParties as List<Party>;
            return botPartiesAsList?.AsReadOnly();
        }

        public virtual bool AddParty(Party newParty, bool isUser = true)
        {
            if (newParty == null || (isUser ? UserParties.Contains(newParty) : BotParties.Contains(newParty)))
            {
                return false;
            }

            if (isUser)
            {
                UserParties.Add(newParty);
            }
            else
            {
                if (newParty.ChannelAccount == null)
                {
                    throw new NullReferenceException($"Channel account of a bot party ({nameof(newParty.ChannelAccount)}) cannot be null");
                }

                BotParties.Add(newParty);
            }

            return true;
        }

        public virtual bool AddParty(string serviceUrl, string channelId,
            ChannelAccount channelAccount, ConversationAccount conversationAccount,
            bool isUser = true)
        {
            Party newParty = new Party(serviceUrl, channelId, channelAccount, conversationAccount);
            return AddParty(newParty, isUser);
        }

        public virtual bool RemoveParty(Party partyToRemove)
        {
            bool wasRemoved = false;

            // Check user and bot parties
            IList<Party>[] partyLists = new IList<Party>[]
            {
                UserParties,
                BotParties
            };

            foreach (IList<Party> partyList in partyLists)
            {
                IList<Party> partiesToRemove = FindPartiesWithMatchingChannelAccount(partyToRemove, partyList);

                if (partiesToRemove != null)
                {
                    foreach (Party party in partiesToRemove)
                    {
                        if (partiesToRemove.Remove(party))
                        {
                            wasRemoved = true;
                        }
                    }
                }
            }

            return wasRemoved;
        }

        public virtual void DeleteAll()
        {
            UserParties.Clear();
            BotParties.Clear();
        }

        public virtual string ResolveBotNameInConversation(Party party)
        {
            string botName = null;

            if (party != null)
            {
                Party botParty = FindBotPartyByChannelAndConversation(party.ChannelId, party.ConversationAccount);

                if (botParty != null && botParty.ChannelAccount != null)
                {
                    botName = botParty.ChannelAccount.Name;
                }
            }

            return botName;
        }

        public virtual Party FindExistingUserParty(Party partyToFind)
        {
            Party foundParty = null;

            try
            {
                foundParty = UserParties.First(party => partyToFind.Equals(party));
            }
            catch (ArgumentNullException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            return foundParty;
        }

        public virtual Party FindPartyByChannelAccountIdAndConversationId(string channelAccountId, string conversationId)
        {
            Party userParty = null;

            try
            {
                userParty = UserParties.Single(party =>
                        (party.ChannelAccount.Id.Equals(channelAccountId)
                         && party.ConversationAccount.Id.Equals(conversationId)));
            }
            catch (InvalidOperationException)
            {
            }

            return userParty;
        }

        public virtual Party FindBotPartyByChannelAndConversation(string channelId, ConversationAccount conversationAccount)
        {
            Party botParty = null;

            try
            {
                botParty = BotParties.Single(party =>
                        (party.ChannelId.Equals(channelId)
                         && party.ConversationAccount.Id.Equals(conversationAccount.Id)));
            }
            catch (InvalidOperationException)
            {
            }

            return botParty;
        }

        public virtual IList<Party> FindPartiesWithMatchingChannelAccount(Party partyToFind, IList<Party> parties)
        {
            IList<Party> matchingParties = null;
            IEnumerable<Party> foundParties = null;

            try
            {
                foundParties = UserParties.Where(party => party.HasMatchingChannelInformation(partyToFind));
            }
            catch (ArgumentNullException)
            {
            }
            catch (InvalidOperationException)
            {
            }

            if (foundParties != null)
            {
                matchingParties = foundParties.ToArray();
            }

            return matchingParties;
        }
    }
}
