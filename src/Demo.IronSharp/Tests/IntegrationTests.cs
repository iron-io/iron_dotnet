using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using IronSharp.Core;
using IronSharp.IronMQ;
using NUnit.Framework;

namespace Demo.IronSharpConsole.Tests
{
    [TestFixture]
    class IntegrationTests
    {
        private IronMqRestClient ironMq;

        private QueueClient queue;

        [SetUp]
        public void Init()
        {
            // Put your ".iron.json" file to home directory, eg. C:\Users\YourUsername
            ironMq = Client.New();            
            queue = ironMq.Queue(GetQueueName());
            queue.Clear();

            // Or config the client here:
            // ironMq = Client.New(new IronClientConfig
            // {
            //     ProjectId = "xxxxxxxxxxxxxxxxxxxxxxxx",
            //     Token = "yyyyyyyyyyyyyyyyyyyy",
            //     Host = "host-of-ironmq.com",
            //     ApiVersion = 3,
            //     Port = 80,
            //     Scheme = Uri.UriSchemeHttp
            // });
        }

        [TearDown]
        public void After()
        {
            queue.Delete();
        }

        [Test]
        public void PostMessage()
        {            
            string messageId = queue.Post("some data");            
            Assert.IsTrue(Regex.IsMatch(messageId, "^[a-f0-9]{19}$"));
        }

        [Test]
        public void ReserveMessage()
        {            
            queue.Post("one");

            var msg = queue.Reserve();
            Assert.AreEqual(19, msg.Id.Length);
            Assert.IsTrue(Regex.IsMatch(msg.ReservationId, "^[a-f0-9]{32}$"));
        }

        [Test]
        public void ReserveMessages()
        {            
            for (int i = 0; i < 2; i++)
                queue.Post(i.ToString());

            var messagesContainer = queue.Reserve(3);
            foreach (var message in messagesContainer.Messages)
            {
                Assert.AreEqual(19, message.Id.Length);
                Assert.IsTrue(Regex.IsMatch(message.ReservationId, "^[a-f0-9]{32}$"));
            }
        }

        [Test]
        public void ClearQueue()
        {
            queue.Post("one");
            var cleared = queue.Clear();
            Assert.IsTrue(cleared);
            Assert.AreEqual(queue.Info().Size, 0);
        }

        [Test]
        public void DeleteReservedMessage()
        {            
            PostTwoMessages();
            var message = queue.Reserve();
            var deleted = message.Delete();
            Assert.IsTrue(deleted);
            Assert.AreEqual(queue.Info().Size, 1);
        }

        [Test]
        public void DeleteReservedMessageViaQueue()
        {
            PostTwoMessages();
            var message = queue.Reserve();
            var deleted = queue.DeleteMessage(message.Id, message.ReservationId);
            Assert.IsTrue(deleted);
            Assert.AreEqual(queue.Info().Size, 1);
        }

        [Test]
        public void DeleteNotReservedMessage()
        {
            queue.Post("one");
            var messageId = queue.Post("two");
            var deleted = queue.Delete(messageId);
            Assert.IsTrue(deleted);
            Assert.AreEqual(1, queue.Info().Size);
        }

        [Test]
        public void DeleteNotReservedMessageViaQueue()
        {
            var messageId = queue.Post("one");
            queue.Post("two");
            var deleted = queue.DeleteMessage(messageId);
            Assert.IsTrue(deleted);
            Assert.AreEqual(1, queue.Info().Size);
        }

        [Test]
        public void UpdateQueue()
        {            
            var expectedTimeout = 66;
            var expectedExpiration = 3333;
            queue.Update(new QueueInfo { MessageTimeout = expectedTimeout, MessageExpiration = expectedExpiration });

            var actualQueueInfo = queue.Info();            
            Assert.AreEqual(expectedTimeout, actualQueueInfo.MessageTimeout);
            Assert.AreEqual(expectedExpiration, actualQueueInfo.MessageExpiration);
        }

        [Test]
        public void DeleteQueue()
        {
            var q = ironMq.Queue(GetQueueName());
            q.Post("one");
            var deleted = q.Delete();
            Assert.IsTrue(deleted);
            Assert.IsNull(q.Info());
        }

        [Test]
        public void GetMessageById()
        {            
            var expectedBody = "testGetById";
            var messageId = queue.Post(expectedBody);
            Assert.AreEqual(expectedBody,queue.Get(messageId).Body);
        }

        [Test]
        public void PeekMessage()
        {            
            var firstMessageId = queue.Post("one");
            queue.Post("two");
            var peekedMessage = queue.PeekNext();
            Assert.AreEqual(firstMessageId, peekedMessage.Id);            
        }        

        [Test]
        public void ReleaseMessage()
        {
            queue.Post("one");
            var msg = queue.Reserve();
            var released = msg.Release();
            Assert.IsTrue(released);
            Assert.AreEqual(msg.Id,queue.Reserve().Id);
        }

        [Test]
        public void DeleteMessages()
        {            
            PostTwoMessages();
            var messages = queue.Reserve(2);
            queue.Delete(messages);
            Assert.AreEqual(0, queue.Info().Size);
        }

        [Test]
        public void CreatePushQueue()
        {
            var q = ironMq.Queue(GetQueueName());
            var pushInfo = GenerateSubscribers(1);
            q.Update(new QueueInfo { PushType = PushType.Multicast, PushInfo = pushInfo});

            Assert.AreEqual(PushType.Multicast, q.Info().PushType);
        }

        [Test]
        public void AddSubscribers()
        {
            var q = ironMq.Queue(GetQueueName());
            var pushInfo = GenerateSubscribers(2);
            q.Update(new QueueInfo { PushType = PushType.Multicast, PushInfo = pushInfo });
            var actualPushInfo = q.Info().PushInfo;

            Assert.AreEqual(pushInfo.Subscribers.Count, actualPushInfo.Subscribers.Count);
            Assert.AreEqual(pushInfo.Subscribers[0].Url, actualPushInfo.Subscribers[0].Url);
            Assert.AreEqual(pushInfo.Subscribers[1].Url, actualPushInfo.Subscribers[1].Url);
        }

        private void PostTwoMessages()
        {
            queue.Post("one");
            queue.Post("two");
        }

        private String GetQueueName()
        {
            return String.Format("queue{0}", DateTime.Now.Ticks);
        }

        private PushInfo GenerateSubscribers(int count)
        {
            var subscribes = new List<Subscriber>();
            for(int i=0;i<count;i++)
                subscribes.Add(new Subscriber(String.Format("subscriber_{0}", i), String.Format("http://myURL{0}", i)));
            return new PushInfo {Subscribers = subscribes};
        }
    }
}