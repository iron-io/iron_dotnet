using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IronIO.Core;
using IronIO.Core.Extensions;

namespace IronIO.IronMQ
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
        /// <param name="timeout">
        /// After timeout (in seconds), item will be placed back onto queue. You must delete the message from the queue to ensure it does not go back onto the queue. If
        /// not set, value from queue is used. Default is 60 seconds, minimum is 30 seconds, and maximum is 86,400 seconds (24 hours).
        /// </param>
        /// <returns>
        /// Returns <c>false</c> if the queue is empty; otherwise <c>true</c>.
        /// </returns>
        public bool Consume(Action<QueueMessageContext<T>, T> consumeAction, IronTimespan timeout = default(IronTimespan))
        {
            var queueMessage = ReserveNext(timeout).Send();

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
                consumeAction?.Invoke(context, queueMessage.ReadValueAs<T>());
            }
            catch (Exception ex)
            {
                if (_errorHandler != null)
                {
                    context.Success = false;
                    _errorHandler?.Invoke(context, ex);
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
        /// Called whenever an error occurs while consuming the message.  Set context.Success to <c>true</c> to *Delete* the message; otherwise it will be automatically released back to
        /// the queue.
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

        public QueueClient(IronMqRestClient client, string name)
        {
            _client = client;
            _name = name;
        }

        public string QueuePath => $"{_client.EndPoint}/{_name}";

        public IValueSerializer ValueSerializer => _client.EndpointConfig.Config.SharpConfig.ValueSerializer;

        #region Queue

        /// <summary>
        /// This call deletes all messages on a queue, whether they are reserved or not.
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#clear_all_messages_from_a_queue
        /// </remarks>
        public IIronTask<bool> Clear()
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Delete,
                Path = $"{QueuePath}/messages",
                HttpContent = new JsonContent(new object())
            };

            return new IronTaskThatReturnsAnExpectedResult(builder, "Cleared");
        }

        /// <summary>
        /// This call deletes a message queue and all its messages.
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#delete_a_message_queue
        /// </remarks>
        public IIronTask<bool> Delete()
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Delete,
                Path = QueuePath
            };

            return new IronTaskThatReturnsAnExpectedResult(builder, "Deleted.");
        }

        /// <summary>
        /// This call gets general information about the queue.
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#get_info_about_a_message_queue
        /// </remarks>
        public IIronTask<QueueInfo> Info()
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = QueuePath
            };

            return new IronTaskThatReturnsQueueInfo(builder);
        }

        /// <summary>
        /// This allows you to change the properties of a queue including setting subscribers and the push type if you want it to be a push queue.
        /// </summary>
        /// <param name="updates"> </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#update_a_message_queue
        /// </remarks>
        /// <returns> </returns>
        public IIronTask<QueueInfo> Update(QueueInfo updates)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Put,
                Path = QueuePath,
                HttpContent = new JsonContent(new QueueContainer(updates))
            };

            return new IronTaskThatReturnsQueueInfo(builder);
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
        public IIronTask<bool> Delete(string messageId)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Delete,
                Path = $"{QueuePath}/messages/{messageId}",
                HttpContent = new JsonContent(new object())
            };

            return new IronTaskThatReturnsAnExpectedResult(builder, "Deleted");
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
        public IIronTask<bool> DeleteMessage(string messageId, string reservationId = null, string subscriberName = null)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Delete,
                Path = $"{QueuePath}/messages/{messageId}",
                HttpContent = new JsonContent(new MessageIdContainer
                {
                    ReservationId = reservationId,
                    SubscriberName = subscriberName
                })
            };

            return new IronTaskThatReturnsAnExpectedResult(builder, "Deleted");
        }

        /// <summary>
        /// This call will delete multiple messages in one call.
        /// </summary>
        /// <param name="messageIds"> A list of message IDs to delete. </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#delete_a_message_from_a_queue
        /// </remarks>
        public IIronTask<bool> Delete(IEnumerable<string> messageIds)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Delete,
                Path = $"{QueuePath}/messages",
                HttpContent = new JsonContent(new ReservedMessageIdCollection(messageIds))
            };

            return new IronTaskThatReturnsAnExpectedResult(builder, "Deleted");
        }

        public IIronTask<bool> Delete(MessageCollection messages)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Delete,
                Path = $"{QueuePath}/messages",
                HttpContent = new JsonContent(new ReservedMessageIdCollection(messages))
            };

            return new IronTaskThatReturnsAnExpectedResult(builder, "Deleted");
        }

        /// <summary>
        /// Get a message by ID.
        /// </summary>
        /// <param name="messageId"> The message ID </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#get_message_by_id
        /// </remarks>
        public IIronTask<QueueMessage> Get(string messageId)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = $"{QueuePath}/messages/{messageId}"
            };

            return new IronTaskThatReturnsQueueMessage(builder, this);
        }

        /// <param name="n">
        /// The maximum number of messages to get. Default is 1. Maximum is 100. Note: You may not receive all n messages on every request, the more sparse the queue, the less
        /// likely you are to receive all n messages.
        /// </param>
        public IIronTask<MessageCollection> Reserve(int n)
        {
            return Reserve(new MessageReservationOptions
            {
                Number = n
            });
        }

        /// <param name="n">
        /// The maximum number of messages to get. Default is 1. Maximum is 100. Note: You may not receive all n messages on every request, the more sparse the queue, the less
        /// likely you are to receive all n messages.
        /// </param>
        /// <param name="timeout">
        /// After timeout (in seconds), item will be placed back onto queue. You must delete the message from the queue to ensure it does not go back onto the queue. If
        /// not set, value from queue is used. Default is 60 seconds, minimum is 30 seconds, and maximum is 86,400 seconds (24 hours).
        /// </param>
        public IIronTask<MessageCollection> Reserve(int n, IronTimespan timeout)
        {
            return Reserve(new MessageReservationOptions
            {
                Number = n,
                Timeout = timeout
            });
        }

        /// <summary>
        /// This call gets/reserves messages from the queue.
        /// The messages will not be deleted, but will be reserved until the timeout expires.
        /// If the timeout expires before the messages are deleted, the messages will be placed back onto the queue.
        /// As a result, be sure to delete the messages after you’re done with them.
        /// </summary>
        /// <param name="reservationOptions"></param>
        /// <remarks>
        /// http://dev.iron.io/mq/3/reference/api/#reserve-messages
        /// </remarks>
        public IIronTask<MessageCollection> Reserve(MessageReservationOptions reservationOptions = null)
        {
            var payload = ReservationUtil.BuildReservationFields(reservationOptions);

            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = $"{QueuePath}/reservations"
            };

            builder.SetJsonContent(payload);

            return new IronTaskThatReturnsMessageCollection(builder, this);
        }

        /// <summary>
        /// This call gets/reserves the next messages from the queue.
        /// This message will not be deleted, but will be reserved until the timeout expires.
        /// If the timeout expires before the message is deleted, this message will be placed back onto the queue.
        /// As a result, be sure to delete this message after you’re done with it.
        /// </summary>
        /// <param name="timeout">
        /// After timeout (in seconds), item will be placed back onto queue. You must delete the message from the queue to ensure it does not go back onto the queue. If
        /// not set, value from queue is used. Default is 60 seconds, minimum is 30 seconds, and maximum is 86,400 seconds (24 hours).
        /// </param>
        public IIronTask<QueueMessage> ReserveNext(IronTimespan timeout)
        {
            return ReserveNext(new ReservationOptions
            {
                Timeout = timeout
            });
        }

        /// <summary>
        /// This call gets/reserves the next messages from the queue.
        /// This message will not be deleted, but will be reserved until the timeout expires.
        /// If the timeout expires before the message is deleted, this message will be placed back onto the queue.
        /// As a result, be sure to delete this message after you’re done with it.
        /// </summary>
        /// <param name="reservationOptions"></param>
        public IIronTask<QueueMessage> ReserveNext(ReservationOptions reservationOptions = null)
        {
            var payload = ReservationUtil.BuildReservationFields(reservationOptions);

            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = $"{QueuePath}/reservations"
            };

            builder.SetJsonContent(payload);

            return new IronTaskThatReturnsQueueMessage(builder, this);
        }

        /// <summary>
        /// Peeking at a queue returns the next messages on the queue, but it does not reserve them.
        /// </summary>
        /// <param name="n"> The maximum number of messages to peek. Default is 1. Maximum is 100. </param>
        /// <remarks>
        /// http://dev.iron.io/mq/3/reference/api/#peek-messages
        /// </remarks>
        [SuppressMessage("Design", "CC0021:You should use nameof instead of program element name string", Justification = "Key values must match API parameter names")]
        public IIronTask<MessageCollection> Peek(int? n = null)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = $"{QueuePath}/messages"
            };

            if (n.HasValue)
            {
                builder.Query.Add("n", n.Value.WithRange(1, 100));
            }

            return new IronTaskThatReturnsMessageCollection(builder, this);
        }

        /// <summary>
        /// Returns the next messages on the queue, but it does not reserve it.
        /// </summary>
        /// <returns> </returns>
        public IIronTask<QueueMessage> PeekNext()
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = $"{QueuePath}/messages"
            };

            builder.Query.Add("n", 1);

            return new IronTaskThatReturnsQueueMessage(builder, this);
        }

        public IIronTask<string> Post(QueueMessage message)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = $"{QueuePath}/messages"
            };

            builder.SetJsonContent(new MessageCollection(message));

            return new IronTaskThatReturnsMessageId(builder);
        }

        public IIronTask<string> Post(object message, MessageOptions options = null)
        {
            return Post(new QueueMessage(ValueSerializer.Generate(message), options));
        }

        public IIronTask<string> Post(string message, MessageOptions options = null)
        {
            return Post(new QueueMessage(message, options));
        }

        public IIronTask<MessageIdCollection> Post(IEnumerable<object> messages, MessageOptions options = null)
        {
            return Post(messages.Select(ValueSerializer.Generate), options);
        }

        public IIronTask<MessageIdCollection> Post(IEnumerable<string> messages, MessageOptions options = null)
        {
            return Post(new MessageCollection(messages, options));
        }

        public IIronTask<MessageIdCollection> Post(IEnumerable<QueueMessage> messages)
        {
            return Post(new MessageCollection(messages));
        }

        /// <summary>
        /// This call adds or pushes messages onto the queue.
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#add_messages_to_a_queue
        /// </remarks>
        public IIronTask<MessageIdCollection> Post(MessageCollection messageCollection)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = $"{QueuePath}/messages"
            };

            builder.SetJsonContent(messageCollection);

            return new IronTaskThatReturnsJson<MessageIdCollection>(builder);
        }

        /// <summary>
        /// Returns <c> true </c> if the next message is not null. (useful for looping constructs)
        /// </summary>
        /// <param name="message"> The the next message from the queue </param>
        /// <param name="timeout"> The message timeout </param>
        public bool Read(out QueueMessage message, IronTimespan timeout = default(IronTimespan))
        {
            message = ReserveNext(timeout).Send();
            return message != null;
        }

        /// <summary>
        /// Releasing a reserved message unreserves the message and puts it back on the queue as if the message had timed out.
        /// </summary>
        /// <param name="messageId"> </param>
        /// <param name="reservationId"></param>
        /// <param name="delay">
        /// The item will not be available on the queue until this many seconds have passed.
        /// Default is 0 seconds.
        /// Maximum is 604,800 seconds (7 days).
        /// </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#release_a_message_on_a_queue
        /// </remarks>
        public IIronTask<bool> Release(string messageId, string reservationId, TimeSpan delay)
        {
            return Release(messageId, reservationId, delay.Seconds);
        }

        /// <summary>
        /// Releasing a reserved message unreserves the message and puts it back on the queue as if the message had timed out.
        /// </summary>
        /// <param name="messageId"> </param>
        /// <param name="reservationId"></param>
        /// <param name="delay">
        /// The item will not be available on the queue until this many seconds have passed.
        /// Default is 0 seconds.
        /// Maximum is 604,800 seconds (7 days).
        /// </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#release_a_message_on_a_queue
        /// </remarks>
        public IIronTask<bool> Release(string messageId, string reservationId, int? delay = null)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = $"{QueuePath}/messages/{messageId}/release"
            };

            builder.SetJsonContent(new MessageOptions
            {
                Delay = delay,
                ReservationId = reservationId
            });

            return new IronTaskThatReturnsAnExpectedResult(builder, "Released");
        }

        /// <summary>
        /// Touching a reserved message extends its timeout by the duration specified when the message was created, which is 60 seconds by default.
        /// </summary>
        /// <param name="messageId"> </param>
        /// <param name="reservationId"></param>
        /// <param name="timeout"></param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#touch_a_message_on_a_queue
        /// </remarks>
        public IIronTask<MessageOptions> Touch(string messageId, string reservationId, IronTimespan timeout = new IronTimespan())
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = $"{QueuePath}/messages/{messageId}/touch"
            };

            builder.SetJsonContent(new MessageOptions
            {
                ReservationId = reservationId,
                Timeout = timeout.GetSeconds()
            });

            return new IronTaskThatReturnsJson<MessageOptions>(builder);
        }

        internal IIronTask<MessageOptions> Touch(QueueMessage message)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = $"{QueuePath}/messages/{message.Id}/touch"
            };

            builder.SetJsonContent(new MessageOptions
            {
                ReservationId = message.ReservationId,
                Timeout = message.Timeout
            });

            return new IronTaskThatTouchesMessage(builder, message);
        }

        /// <summary>
        /// Gets a webhook url that can used by a third party to add messages to this queue.  See http://dev.iron.io/mq/reference/api/#add_messages_to_a_queue_via_webhook for more info.
        /// </summary>
        /// <param name="token">(optional) The token to use for the building the request uri if different than the Token specified in the config.</param>
        public Uri WebhookUri(string token = null)
        {
            var endpointConfig = _client.EndpointConfig;

            var webhookAuth = endpointConfig.TokenContainer.GetWebHookToken(token);

            var builder = new IronTaskRequestBuilder(endpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = $"{QueuePath}/webhook",
                AuthToken = webhookAuth
            };

            return builder.Build().RequestUri;
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
        public IIronTask<QueueInfo> AddAlerts(AlertCollection alertCollection)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = $"{QueuePath}/alerts"
            };

            builder.SetJsonContent(alertCollection);

            return new IronTaskThatReturnsJson<QueueInfo>(builder);
        }

        /// <summary>
        /// Update queue alerts. This is for Pull Queues only.
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#update_alerts_to_a_queue
        /// http://dev.iron.io/mq/reference/queue_alerts/
        /// </remarks>
        public IIronTask<QueueInfo> UpdateAlerts(AlertCollection alertCollection)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Put,
                Path = $"{QueuePath}/alerts"
            };

            builder.SetJsonContent(alertCollection);

            return new IronTaskThatReturnsJson<QueueInfo>(builder);
        }

        /// <summary>
        /// Removes an alert from the queue.
        /// See http://dev.iron.io/mq/reference/queue_alerts/ for more information.
        /// </summary>
        /// <param name="alert"> Alert object to delete. </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#remove_alert_from_a_queue_by_id
        /// </remarks>
        public IIronTask<bool> DeleteAlert(Alert alert)
        {
            return DeleteAlert(alert?.Id);
        }

        /// <summary>
        /// Removes an alert specified by id from the queue.
        /// See http://dev.iron.io/mq/reference/queue_alerts/ for more information.
        /// </summary>
        /// <param name="alertId"> Id of alert to delete. </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#remove_alert_from_a_queue_by_id
        /// </remarks>
        public IIronTask<bool> DeleteAlert(string alertId)
        {
            if (string.IsNullOrEmpty(alertId))
            {
                return new NoOpIronTaskResult<bool>(false);
            }

            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Delete,
                Path = $"{QueuePath}/alerts/{alertId}"
            };

            return new IronTaskThatReturnsAnExpectedResult(builder, "Deleted");
        }

        /// <summary>
        /// Removes alerts from a queue. This is for Pull Queues only.
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#remove_alerts_from_a_queue
        /// </remarks>
        public IIronTask<QueueInfo> RemoveAlerts(AlertCollection alertCollection)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Delete,
                Path = $"{QueuePath}/alerts"
            };

            builder.SetJsonContent(alertCollection);

            return new IronTaskThatReturnsJson<QueueInfo>(builder);
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
        public IIronTask<QueueInfo> AddSubscribers(SubscriberCollection subscriberCollection)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Post,
                Path = $"{QueuePath}/subscribers"
            };

            builder.SetJsonContent(subscriberCollection);

            return new IronTaskThatReturnsJson<QueueInfo>(builder);
        }

        /// <summary>
        /// You can retrieve the push status for a particular message which will let you know which subscribers have received the message, which have failed, how many times it’s tried to
        /// be
        /// delivered and the status code returned from the endpoint.
        /// </summary>
        /// <param name="messageId"> The message ID </param>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#get_push_status_for_a_message
        /// </remarks>
        public IIronTask<SubscriberCollection> PushStatus(string messageId)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Get,
                Path = $"{QueuePath}/messages/{messageId}/subscribers"
            };

            return new IronTaskThatReturnsJson<SubscriberCollection>(builder);
        }

        /// <summary>
        /// Removes subscribers from a queue. This is for Push Queues only.
        /// </summary>
        /// <remarks>
        /// http://dev.iron.io/mq/reference/api/#remove_subscribers_from_a_queue
        /// </remarks>
        public IIronTask<QueueInfo> RemoveSubscribers(SubscriberCollection subscriberCollection)
        {
            var builder = new IronTaskRequestBuilder(_client.EndpointConfig)
            {
                HttpMethod = HttpMethod.Delete,
                Path = $"{QueuePath}/subscribers"
            };

            builder.SetJsonContent(subscriberCollection);

            return new IronTaskThatReturnsJson<QueueInfo>(builder);
        }

        #endregion
    }
}