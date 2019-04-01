﻿using log4net;
using Shared;
using Shared.Client;
using Shared.Comms.MailService;
using Shared.Comms.Messages;
using Shared.Conversations;
using Shared.Conversations.SharedStates;
using Shared.PortfolioResources;

namespace Broker.Conversations.CreatePortfolio
{
    public class CreatePortfolioReceiveState : ConversationState
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string Username
        {
            get; set;
        }

        private string Password
        {
            get; set;
        }


        public CreatePortfolioReceiveState(Envelope envelope, Conversation conversation) : base(envelope, conversation, null)
        {
            if (!(envelope.Contents is CreatePortfolioRequestMessage request))
                throw new System.Exception("CreatePortflioReceieveState requires CreatePortfolioRequestMessage.");
            if (request.Account.Password.Equals(request.ConfirmPassword))
            {
                Username = request.Account.Username;
                Password = request.Account.Password;
            }
        }

        public override ConversationState HandleMessage(Envelope incomingMessage)
        {
            if (incomingMessage.Contents is CreatePortfolioRequestMessage m)
            {
                Username = m.Account.Username;
                Password = m.Account.Password;
            }

            return null;
        }

        public override Envelope Prepare()
        {
            Log.Debug($"{nameof(Prepare)} (enter)");
            var message = GetMessage();

            // If we created a portfolio, add the receiver to the client list.
            if (message is PortfolioUpdateMessage m)
            {
                ClientManager.TryToAdd(To);
            }

            var env = new Envelope(message, Config.GetString(Config.BROKER_IP), Config.GetInt(Config.BROKER_PORT))
            {
                To = this.To
            };

            Log.Debug($"{nameof(Prepare)} (exit)");
            return env;
        }

        private Message GetMessage()
        {
            if (!PortfolioManager.TryToCreate(Username, Password, out Portfolio portfolio))
            {
                var errormessage = MessageFactory.GetMessage<ErrorMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM), 0) as ErrorMessage;

                errormessage.ConversationID = Conversation.Id;
                errormessage.ReferenceMessageID = this.MessageId;

                return errormessage;
            }

            var message = MessageFactory.GetMessage<PortfolioUpdateMessage>(Config.GetInt(Config.BROKER_PROCESS_NUM), 0) as PortfolioUpdateMessage;

            message.ConversationID = Conversation.Id;
            message.PortfolioID = portfolio.PortfolioID;
            message.Assets = portfolio.CloneAssets();
            PortfolioManager.ReleaseLock(portfolio);

            return message;
        }

        public override void Send()
        {
            base.Send();
            Conversation.UpdateState(new ConversationDoneState(Conversation, this));
        }
    }
}