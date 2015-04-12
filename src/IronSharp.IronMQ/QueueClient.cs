using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using IronIO.Core;
using IronSharp.Core;

namespace IronSharp.IronMQ
{
    public class QueueClient<T> : QueueClient
    {
        private int? _delay;

        private Action<QueueMessageContext<T>, Exception> _errorHandler;

        public QueueClient(IronMqRestClient client, string name)
            : base(client, name)
        {
        }

        /// <summary>
        /// Consumes the next message off the queue. Set context.Success to <c>false</c> to *Release* the message back to the queue; otherwise it will be automatically deleted.
        /// </summary>
        /// <param name="consumeAction"></param>
        /// <param name="timeout"></param>
        /// <returns>
        /// Returns <c>false</c> if the queue is empty; otherwise <c>true</c>.
        /// </returns>
        public bool Consume(Action<QueueMessageContext<T>, T> consumeAction, TimeSpan timeout)
        {
            return Consume(consumeAction, timeout.Seconds);
        }

        /// <summary>
        /// Consumes the next message off the queue. Set context.Success to <c>false</c> to *Release* the message back to the queue; otherwise it will be automatically deleted.
        /// </summary>
        /// <param name="consumeAction"></param>
        /// <param name="timeout"></param>
        /// <returns>
        /// Returns <c>false</c> if the queue is empty; otherwise <c>true</c>.
        /// </returns>
        public bool Consume(Action<QueueMessageContext<T>, T> consumeAction, int? timeout = null)
        {
            QueueMessage queueMessage = Next(timeout);

            if (queueMessage == null)
            {
                return false;
            }

            var context = new QueueMessageContext<T>
            {
                Message = queueMessage,
                Success = true,
                Client = this
            };
            
            try
            {
                consumeAction(context, queueMessage.ReadValueAs<T>());
            }
            catch (Exception ex)
            {
                if (_errorHandler != null)
                {
                    context.Success = false;
                    _errorHandler(context, ex);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                if (context.Success)
                {
                    queueMessage.Delete();
                }
                else
                {
                    queueMessage.Release(_delay);
                }
            }

            return true;
        }

        /// <summary>
        /// Sets the delay when the message is released back to the queue.
        /// </summary>
        /// <param name="delay">The item will not be available on the queue until this many seconds have passed. Default is 0 seconds. Maximum is 604,800 seconds (7 days).</param>
        /// <returns></returns>
        public QueueClient<T> DelayOnRelease(TimeSpan delay)
        {
            return DelayOnRelease(delay.Seconds);
        }

        /// <summary>
        /// Sets the delay when the message is released back to the queue.
        /// </summary>
        /// <param name="delay">The item will not be available on the queue until this many seconds have passed. Default is 0 seconds. Maximum is 604,800 seconds (7 days).</param>
        /// <returns></returns>
        public QueueClient<T> DelayOnRelease(int? delay)
        {
            _delay = delay;
            return this;
        }

        /// <summary>
        /// Called whenever an error occurs while consuming the message.  Set context.Success to <c>true</c> to *Delete* the message; otherwise it will be automatically released back to the queue.
        /// </summary>
        public QueueClient<T> OnError(Action<QueueMessageContext<T>, Exception> errorHandler)
        {
            _errorHandler = errorHandler;
            return this;
        }
    }

    /// <summary>
    /// Iron.io MQ Client
    /// </summary>
    /// <remarks>
    /// https://github.com/iron-io/iron_mq_ruby
    /// </remarks>
    public class QueueClient
    {
        private readonly IronMqRestClient _client;
        private readonly string _name;
        private MqRestClient _restClient;

        public QueueClient(IronMqRestClient client, string name)
        {
            _client = client;
            _name = name;
            _restClient = new MqRestClient(_client.TokenContainer);
        }

        public string EndPoint
        {
            get { return string.Format("{0}/{1}", _client.EndPoint, _name); }
        }

        public IValueSerializer ValueSerializer
        {
            get { return _client.Config.SharpConfig.ValueSerializer; }
        }

        #region Queue

        /// <summary>
        /// This call deletes all messages on a queue, whether they are reserved or not.
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#clear_all_messages_from_a_queue
        /// </remarks>
        public bool Clear()
        {
            return _restClient.Delete<ResponseMsg>(_client.Config, string.Format("{0}/messages", EndPoint), null, new object()).HasExpectedMessage("Cleared");
        }

        /// <summary>
        /// This call deletes a message queue and all its messages.
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#delete_a_message_queue
        /// </remarks>
        public bool Delete()
        {
            return _restClient.Delete<ResponseMsg>(_client.Config, EndPoint).HasExpectedMessage("Deleted.");
        }

        /// <summary>
        /// This call gets general information about the queue.
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#get_info_about_a_message_queue
        /// </remarks>
        public QueueInfo Info()
        {
            QueueContainer container = _restClient.Get<QueueContainer>(_client.Config, EndPoint);
            return container.Queue;
        }

        /// <summary>
        /// This allows you to change the properties of a queue including setting subscribers and the push type if you want it to be a push queue.
        /// </summary>
        /// <param name="updates"> </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#update_a_message_queue
        /// </remarks>
        /// <returns> </returns>
        public QueueInfo Update(QueueInfo updates)
        {
            QueueContainer response = _restClient.Put<QueueContainer>(_client.Config, EndPoint, new QueueContainer(updates));
            return response.Queue;
        }

        #endregion

        #region Messages

        /// <summary>
        /// This call will delete the message. Be sure you call this after you’re done with a message or it will be placed back on the queue.
        /// </summary>
        /// <param name="messageId"> The id of the message to delete. </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#delete_a_message_from_a_queue
        /// </remarks>
        public bool Delete(string messageId)
        {
            return _restClient.Delete<ResponseMsg>(_client.Config, string.Format("{0}/messages/{1}", EndPoint, messageId), null, new object()).HasExpectedMessage("Deleted");
        }

        /// <summary>
        /// This call will delete the message. Be sure you call this after you’re done with a message or it will be placed back on the queue.
        /// </summary>
        /// <param name="messageId"> The id of the message to delete. </param>
        /// <param name="reservationId"> Reservation id of the message to delete. </param>
        /// <param name="subscriberName"></param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#delete_a_message_from_a_queue
        /// </remarks>
        public bool DeleteMessage(string messageId, string reservationId=null, string subscriberName=null)
        {
            var payload = new MessageIdContainer {ReservationId = reservationId, SubscriberName = subscriberName};            
            return _restClient.Delete<ResponseMsg>(_client.Config, string.Format("{0}/messages/{1}", EndPoint, messageId), null, payload).HasExpectedMessage("Deleted");
        }

        /// <summary>
        /// This call will delete multiple messages in one call.
        /// </summary>
        /// <param name="messageIds"> A list of message IDs to delete. </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#delete_a_message_from_a_queue
        /// </remarks>
        public bool Delete(IEnumerable<string> messageIds)
        {
            return
                _restClient.Delete<ResponseMsg>(_client.Config, string.Format("{0}/messages", EndPoint), payload: new ReservedMessageIdCollection(messageIds)).HasExpectedMessage("Deleted");
        }

        public bool Delete(MessageCollection messages)
        {
            return _restClient
                .Delete<ResponseMsg>(_client.Config, string.Format("{0}/messages", EndPoint), payload: new ReservedMessageIdCollection(messages))
                .HasExpectedMessage("Deleted");
        }

        /// <summary>
        /// Get a message by ID.
        /// </summary>
        /// <param name="messageId"> The message ID </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#get_message_by_id
        /// </remarks>
        public QueueMessage Get(string messageId)
        {
            MessageContainer messageContainer = _restClient.Get<MessageContainer>(_client.Config,
                string.Format("{0}/messages/{1}", EndPoint, messageId));
            return messageContainer.Message;            
        }

        /// <summary>
        /// This call gets/reserves messages from the queue.
        /// The messages will not be deleted, but will be reserved until the timeout expires.
        /// If the timeout expires before the messages are deleted, the messages will be placed back onto the queue.
        /// As a result, be sure to delete the messages after you’re done with them.
        /// </summary>
        /// <param name="n">
        /// The maximum number of messages to get.
        /// Default is 1.
        /// Maximum is 100.
        /// </param>
        /// <param name="timeout">
        /// After timeout (in seconds), item will be placed back onto queue.
        /// You must delete the message from the queue to ensure it does not go back onto the queue.
        /// If not set, value from POST is used.
        /// Default is 60 seconds.
        /// Minimum is 30 seconds.
        /// Maximum is 86,400 seconds (24 hours).
        /// </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#get_messages_from_a_queue
        /// https://github.com/iron-io/iron_mq_ruby#get-messages-from-a-queue
        /// </remarks>
        public MessageCollection Get(int? n = null, TimeSpan? timeout = null)
        {
            int? seconds = null;
            if (timeout.HasValue)
            {
                seconds = timeout.Value.Seconds;
            }
            return Get(n, seconds);
        }

        /// <summary>
        /// This call gets/reserves messages from the queue.
        /// The messages will not be deleted, but will be reserved until the timeout expires.
        /// If the timeout expires before the messages are deleted, the messages will be placed back onto the queue.
        /// As a result, be sure to delete the messages after you’re done with them.
        /// </summary>
        /// <param name="n">
        /// The maximum number of messages to get.
        /// Default is 1.
        /// Maximum is 100.
        /// </param>
        /// <param name="timeout">
        /// After timeout (in seconds), item will be placed back onto queue.
        /// You must delete the message from the queue to ensure it does not go back onto the queue.
        /// If not set, value from POST is used.
        /// Default is 60 seconds.
        /// Minimum is 30 seconds.
        /// Maximum is 86,400 seconds (24 hours).
        /// </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#get_messages_from_a_queue
        /// https://github.com/iron-io/iron_mq_ruby#get-messages-from-a-queue
        /// </remarks>
        public MessageCollection Get(int? n = null, int? timeout = null, int? wait = null)
        {
            var query = new NameValueCollection();

            var payload = new Dictionary<string, object>();
            if (n.HasValue)
            {
                payload.Add("n", n);
            }
            if (timeout.HasValue)
            {
                payload.Add("timeout", timeout);
            }

            if (wait.HasValue)
            {
                query.Add("wait", Convert.ToString(wait));
            }

            RestResponse<MessageCollection> result = _restClient.Post<MessageCollection>(_client.Config, string.Format("{0}/reservations", EndPoint), payload, query);

            if (result.CanReadResult())
            {
                return LinkMessageCollection(result);
            }

            throw new RestResponseException("Unable to read MessageCollection response", result.ResponseMessage);
        }

        /// <summary>
        /// This call gets/reserves messages from the queue.
        /// The messages will not be deleted, but will be reserved until the timeout expires.
        /// If the timeout expires before the messages are deleted, the messages will be placed back onto the queue.
        /// As a result, be sure to delete the messages after you’re done with them.
        /// </summary>
        /// <param name="n">
        /// The maximum number of messages to get.
        /// Default is 1.
        /// Maximum is 100.
        /// </param>
        /// <param name="timeout">
        /// After timeout (in seconds), item will be placed back onto queue.
        /// You must delete the message from the queue to ensure it does not go back onto the queue.
        /// If not set, value from POST is used.
        /// Default is 60 seconds.
        /// Minimum is 30 seconds.
        /// Maximum is 86,400 seconds (24 hours).
        /// </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#get_messages_from_a_queue
        /// https://github.com/iron-io/iron_mq_ruby#get-messages-from-a-queue
        /// </remarks>
        public MessageCollection Get(int? n = null, int? timeout = null)
        {
            return Get(n, timeout, null);
        }

        /// <summary>
        /// This call gets/reserves the next messages from the queue.
        /// This message will not be deleted, but will be reserved until the timeout expires.
        /// If the timeout expires before the message is deleted, this message will be placed back onto the queue.
        /// As a result, be sure to delete this message after you’re done with it.
        /// </summary>
        public QueueMessage Next(TimeSpan timeout)
        {
            return Next(timeout.Seconds);
        }

        /// <summary>
        /// This call gets/reserves the next messages from the queue.
        /// This message will not be deleted, but will be reserved until the timeout expires.
        /// If the timeout expires before the message is deleted, this message will be placed back onto the queue.
        /// As a result, be sure to delete this message after you’re done with it.
        /// </summary>
        public QueueMessage Next(int? timeout = null)
        {
            return Get(1, timeout).Messages.FirstOrDefault();
        }

        public QueueMessage Reserve()
        {
            return Get(1, 0).Messages.FirstOrDefault();
        }

        public MessageCollection Reserve(int? n = null, int? timeout = null, int? wait = null)
        {
            return Get(n, timeout, wait);
        }

        public MessageCollection Reserve(int? n, TimeSpan? timeout)
        {
            return Get(n, timeout);
        }

        /// <summary>
        /// This call gets/reserves the next messages from the queue.
        /// This message will not be deleted, but will be reserved until the timeout expires.
        /// If the timeout expires before the message is deleted, this message will be placed back onto the queue.
        /// As a result, be sure to delete this message after you’re done with it.
        /// </summary>
        /// <param name="wait">Time in seconds to wait for a message to become available. 
        /// This enables long polling. Default is 0 (does not wait), maximum is 30.</param>
        public QueueMessage Next(int? timeout, int? wait)
        {
            return Get(1, timeout, wait).Messages.FirstOrDefault();
        }

        /// <summary>
        /// Peeking at a queue returns the next messages on the queue, but it does not reserve them.
        /// </summary>
        /// <param name="n"> The maximum number of messages to peek. Default is 1. Maximum is 100. </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#peek_messages_on_a_queue
        /// </remarks>
        public MessageCollection Peek(int? n = null)
        {
            var query = new NameValueCollection();

            if (n.HasValue)
            {
                query.Add("n", Convert.ToString(n));
            }

            RestResponse<MessageCollection> result = _restClient.Get<MessageCollection>(_client.Config, string.Format("{0}/messages", EndPoint), query);


            if (result.CanReadResult())
            {
                return LinkMessageCollection(result);
            }

            throw new RestResponseException("Unable to read MessageCollection response", result.ResponseMessage);
        }

        /// <summary>
        /// Returns the next messages on the queue, but it does not reserve it.
        /// </summary>
        /// <returns> </returns>
        public QueueMessage PeekNext()
        {
            return Peek(1).Messages.FirstOrDefault();
        }

        public string Post(QueueMessage message)
        {
            MessageIdCollection result = Post(new MessageCollection(message));

            if (result.Success)
            {
                return result.Ids.FirstOrDefault();
            }

            throw new IronSharpException("Failed to queue message");
        }

        public string Post(object message, MessageOptions options = null)
        {
            return Post(new QueueMessage(ValueSerializer.Generate(message), options));
        }

        public string Post(string message, MessageOptions options = null)
        {
            return Post(new QueueMessage(message, options));
        }

        public MessageIdCollection Post(IEnumerable<object> messages, MessageOptions options = null)
        {
            return Post(messages.Select(ValueSerializer.Generate), options);
        }

        public MessageIdCollection Post(IEnumerable<string> messages, MessageOptions options = null)
        {
            return Post(new MessageCollection(messages, options));
        }

        public MessageIdCollection Post(IEnumerable<QueueMessage> messages)
        {
            return Post(new MessageCollection(messages));
        }

        /// <summary>
        /// This call adds or pushes messages onto the queue.
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#add_messages_to_a_queue
        /// </remarks>
        public MessageIdCollection Post(MessageCollection messageCollection)
        {
            return _restClient.Post<MessageIdCollection>(_client.Config, string.Format("{0}/messages", EndPoint), messageCollection);
        }

        /// <summary>
        /// Returns <c> true </c> if the next message is not null. (useful for looping constructs)
        /// </summary>
        /// <param name="message"> The the next message from the queue </param>
        /// <param name="timeout"> The message timeout </param>
        public bool Read(out QueueMessage message, TimeSpan timeout)
        {
            return Read(out message, timeout.Seconds);
        }

        /// <summary>
        /// Returns <c> true </c> if the next message is not null. (useful for looping constructs)
        /// </summary>
        /// <param name="message"> The the next message from the queue </param>
        /// <param name="timeout"> The message timeout </param>
        public bool Read(out QueueMessage message, int? timeout = null)
        {
            message = Next(timeout);
            return message != null;
        }

        /// <summary>
        /// Releasing a reserved message unreserves the message and puts it back on the queue as if the message had timed out.
        /// </summary>
        /// <param name="messageId"> </param>
        /// <param name="delay">
        /// The item will not be available on the queue until this many seconds have passed.
        /// Default is 0 seconds.
        /// Maximum is 604,800 seconds (7 days).
        /// </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#release_a_message_on_a_queue
        /// </remarks>
        public bool Release(string messageId, string reservationId, TimeSpan delay)
        {
            return Release(messageId, reservationId, delay.Seconds);
        }

        /// <summary>
        /// Releasing a reserved message unreserves the message and puts it back on the queue as if the message had timed out.
        /// </summary>
        /// <param name="messageId"> </param>
        /// <param name="delay">
        /// The item will not be available on the queue until this many seconds have passed.
        /// Default is 0 seconds.
        /// Maximum is 604,800 seconds (7 days).
        /// </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#release_a_message_on_a_queue
        /// </remarks>
        public bool Release(string messageId, string reservationId, int? delay = null)
        {
            var payload = new MessageOptions {Delay = delay, ReservationId = reservationId};
            return _restClient.Post<ResponseMsg>(_client.Config, string.Format("{0}/messages/{1}/release", EndPoint, messageId), payload).HasExpectedMessage("Released");
        }

        /// <summary>
        /// Touching a reserved message extends its timeout by the duration specified when the message was created, which is 60 seconds by default.
        /// </summary>
        /// <param name="messageId"> </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#touch_a_message_on_a_queue
        /// </remarks>
        public MessageOptions Touch(string messageId, string reservationId, int? timeout = null)
        {
            var payload = new MessageOptions { ReservationId = reservationId, Timeout = timeout};            
            return _restClient.Post<MessageOptions>(_client.Config, string.Format("{0}/messages/{1}/touch", EndPoint, messageId), payload);
        }

        /// <summary>
        /// Gets a webhook url that can used by a third party to add messages to this queue.  See http://dev.iron.io/mq/reference/api/#add_messages_to_a_queue_via_webhook for more info.
        /// </summary>
        /// <param name="token">(optional) The token to use for the building the request uri if different than the Token specified in the config.</param>
        public Uri WebhookUri(string token = null)
        {
            IRestClientRequest request = new RestClientRequest
            {
                EndPoint = string.Format("{0}/webhook", EndPoint),
                AuthTokenLocation = AuthTokenLocation.Querystring
            };
            return _restClient.BuildRequestUri(_client.Config, request, token);
        }

        private MessageCollection LinkMessageCollection(RestResponse<MessageCollection> response)
        {
            MessageCollection messageCollection = response.Result;

            foreach (QueueMessage msg in messageCollection.Messages)
            {
                msg.Client = this;
            }

            return messageCollection;
        }

        #endregion

        #region Alerts

        /// <summary>
        /// Add alerts to a queue. This is for Pull Queues only.
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#add_alerts_to_a_queue
        /// http://dev.iron.io/mq/reference/queue_alerts/
        /// </remarks>
        public QueueInfo AddAlerts(AlertCollection alertCollection)
        {
            return _restClient.Post<QueueInfo>(_client.Config, string.Format("{0}/alerts", EndPoint), alertCollection);
        }

        /// <summary>
        /// Update queue alerts. This is for Pull Queues only.
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#update_alerts_to_a_queue
        /// http://dev.iron.io/mq/reference/queue_alerts/
        /// </remarks>
        public QueueInfo UpdateAlerts(AlertCollection alertCollection)
        {
            return _restClient.Put<QueueInfo>(_client.Config, string.Format("{0}/alerts", EndPoint), alertCollection);
        }

        /// <summary>
        /// Removes an alert from the queue.
        /// See http://dev.iron.io/mq/reference/queue_alerts/ for more information.
        /// </summary>
        /// <param name="alert"> Alert object to delete. </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#remove_alert_from_a_queue_by_id
        /// </remarks>
        public bool DeleteAlert(Alert alert)
        {
            if (alert == null)
                return false;
            return DeleteAlert(alert.Id);
        }

        /// <summary>
        /// Removes an alert specified by id from the queue.
        /// See http://dev.iron.io/mq/reference/queue_alerts/ for more information.
        /// </summary>
        /// <param name="alert"> Id of alert to delete. </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#remove_alert_from_a_queue_by_id
        /// </remarks>
        public bool DeleteAlert(string alertId)
        {
            if (String.IsNullOrEmpty(alertId))
                return false;
            return _restClient.Delete<ResponseMsg>(_client.Config, string.Format("{0}/alerts/{1}", EndPoint, alertId)).HasExpectedMessage("Deleted");
        }

        /// <summary>
        /// Removes alerts from a queue. This is for Pull Queues only.
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#remove_alerts_from_a_queue
        /// </remarks>
        public QueueInfo RemoveAlerts(AlertCollection alertCollection)
        {
            return _restClient.Delete<QueueInfo>(_client.Config, string.Format("{0}/alerts", EndPoint), payload: alertCollection);
        }

        #endregion

        #region Subscribers

        /// <summary>
        /// Add subscribers (HTTP endpoints) to a queue. This is for Push Queues only.
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#add_subscribers_to_a_queue
        /// http://dev.iron.io/mq/reference/push_queues/
        /// </remarks>
        public QueueInfo AddSubscribers(SubscriberCollection subscriberCollection)
        {
            return _restClient.Post<QueueInfo>(_client.Config, string.Format("{0}/subscribers", EndPoint), subscriberCollection);
        }

        /// <summary>
        /// You can retrieve the push status for a particular message which will let you know which subscribers have received the message, which have failed, how many times it’s tried to be
        /// delivered and the status code returned from the endpoint.
        /// </summary>
        /// <param name="messageId"> The message ID </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#get_push_status_for_a_message
        /// </remarks>
        public SubscriberCollection PushStatus(string messageId)
        {
            return _restClient.Get<SubscriberCollection>(_client.Config, string.Format("{0}/messages/{1}/subscribers", EndPoint, messageId));
        }

        /// <summary>
        /// Removes subscribers from a queue. This is for Push Queues only.
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#remove_subscribers_from_a_queue
        /// </remarks>
        public QueueInfo RemoveSubscribers(SubscriberCollection subscriberCollection)
        {
            return _restClient.Delete<QueueInfo>(_client.Config, string.Format("{0}/subscribers", EndPoint), payload: subscriberCollection);
        }

        #endregion
    }
}