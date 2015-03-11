using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IronSharp.Core;
using IronSharp.IronMQ;
using NUnit.Framework;

namespace Demo.IronSharpConsole.Tests
{
    [TestFixture]
    class LongRunningIntegrationTests
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
        public void TouchMessage()
        {
            queue.Post("one");
            var msg = queue.Reserve(timeout: 30).Messages[0];
            var oldReservationId = msg.ReservationId;
            Thread.Sleep(25000);
            var newReservationId = queue.Touch(msg.Id, msg.ReservationId, 30).ReservationId;
            Thread.Sleep(10000);
            Assert.AreEqual(0, queue.Reserve(1).Messages.Count);
            Assert.AreNotEqual(oldReservationId, newReservationId);
        }

        private String GetQueueName()
        {
            return String.Format("queue{0}", DateTime.Now.Ticks);
        }
    }
}
