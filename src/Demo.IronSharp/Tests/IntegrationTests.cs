using System;
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

        [SetUp]
        public void Init()
        {
            // Put your ".iron.json" file to home directory, eg. C:\Users\YourUsername
            ironMq = Client.New();

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

        [Test]
        public void PostMessageTest()
        {
            QueueClient q = ironMq.Queue(GetQueueName());
            string messageId = q.Post("some data");
            Console.WriteLine("Posted message with id {0}", messageId);
            Assert.IsTrue(Regex.IsMatch(messageId, "^[a-f0-9]{19}$"));
        }

        [Test]
        public void ReserveMessage()
        {
            var q = ironMq.Queue(GetQueueName());
            for (int i = 0; i < 3; i++)
                q.Post(i.ToString());

            var msg = q.Reserve();
            Assert.AreEqual(19, msg.Id.Length);
            Assert.IsTrue(Regex.IsMatch(msg.ReservationId, "^[a-f0-9]{32}$"));
        }

        [Test]
        public void ReserveMessages()
        {
            var q = ironMq.Queue(GetQueueName());
            for (int i = 0; i < 3; i++)
                q.Post(i.ToString());

            var messages = q.Reserve(wait: 12);
            foreach (var message in messages.Messages)
            {
                Assert.AreEqual(19, message.Id.Length);
                Assert.IsTrue(Regex.IsMatch(message.ReservationId, "^[a-f0-9]{32}$"));
            }
        }

        private String GetQueueName()
        {           
            return String.Format("queue{0}", DateTime.Now.Ticks); ;
        }
    }
}