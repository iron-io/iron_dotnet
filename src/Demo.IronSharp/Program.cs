using System;
using System.Linq;
using System.Threading;
using Common.Logging;
using Common.Logging.Simple;
using IronSharp.Core;
using IronSharp.IronCache;
using IronSharp.IronMQ;
using IronSharp.IronWorker;

namespace Demo.IronSharpConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            LogManager.Adapter = new ConsoleOutLoggerFactoryAdapter();

            // =========================================================
            // Iron.io MQ
            // =========================================================

            //IronMqRestClient ironMq = IronSharp.IronMQ.Client.New();

            // For beta testing
            IronMqRestClient ironMq = IronSharp.IronMQ.Client.New(new IronClientConfig { ProjectId = "53a3b3bd5e8edd1245000005", Token = "O7KrMTwmw997iq0KzL7v", Host = "192.168.1.155", ApiVersion = 3, Port = 8080, Scheme = Uri.UriSchemeHttp });

            // Simple actions

            // Post message to a queue
            TestPosting(ironMq);

            // Post message to a queue and reserve it
            TestReservation(ironMq);

            // Post message, reserve it and delete
            TestDeletingReservedMessage(ironMq);
            
            // Actions on queue

            // Update queue info
            TestUpdatingTheQueue(ironMq);

            // Clear all messages of queue
            TestClearingQueue(ironMq);

            // Delete queue and its messages
            TestDeletingQueue(ironMq);

            // Get list of all queus inside project
            TestGettingListQueue(ironMq);

            // Actions on messages

            //TestPosting(ironMq);
            //TestReservation(ironMq);
            //TestDeletingReservedMessage(ironMq);

            // Get message by id without reservation
            TestGettingMessageById(ironMq);
            
            // Get message without reserving it
            TestPeekingMessage(ironMq);

            // Delete unreserved message
            TestDeletingMessage(ironMq);
            
            // Touch message to prolongate reservation
            TestTouching(ironMq);

            // Release reserved message
            TestReleasing(ironMq);

            // Delete a bunch of messages
            TestDeletingMessages(ironMq);

            // =========================================================
            // Iron.io Worker
            // =========================================================

            Console.WriteLine("Be sure to create a 'Test' worker before running this sample");
            Console.WriteLine("Press ANY key to continue");
            Console.Read();

            IronWorkerRestClient workerClient = IronSharp.IronWorker.Client.New();

            string taskId = workerClient.Tasks.Create("Test", new {Key = "Value"});

            Console.WriteLine("TaskID: {0}", taskId);

            TaskInfoCollection taskInfoCollection = workerClient.Tasks.List("Test");

            foreach (TaskInfo task in taskInfoCollection.Tasks)
            {
                Console.WriteLine(task.Inspect());
            }

            ScheduleOptions options = ScheduleBuilder.Build().
                Delay(TimeSpan.FromMinutes(1)).
                WithFrequency(TimeSpan.FromHours(1)).
                RunFor(TimeSpan.FromHours(3)).
                WithPriority(TaskPriority.Default);

            var payload = new
            {
                a = "b",
                c = new[] {1, 2, 3}
            };

            ScheduleIdCollection schedule = workerClient.Schedules.Create("Test", payload, options);

            Console.WriteLine(schedule.Inspect());

            workerClient.Schedules.Cancel(schedule.Schedules.First().Id);

            // =========================================================
            // Iron.io Cache
            // =========================================================

            IronCacheRestClient ironCacheClient = IronSharp.IronCache.Client.New();

            // Get a Cache object
            CacheClient cache = ironCacheClient.Cache("my_cache");

            // Put value to cache by key
            cache.Put("number_item", 42);

            // Get value from cache by key
            Console.WriteLine(cache.Get("number_item").Value);

            // Get value from cache by key
            Console.WriteLine(cache.Get<int>("number_item"));

            // Numbers can be incremented
            cache.Increment("number_item", 10);

            // Immediately delete an item
            cache.Delete("number_item");

            cache.Put("complex_item", new { greeting = "Hello", target = "world" });

            // Get value from cache by key
            Console.WriteLine(cache.Get("complex_item").Value);

            cache.Delete("complex_item");

            Console.WriteLine("============= Done ==============");
            Console.Read();

        }

        private static void TestPosting(IronMqRestClient ironMq)
        {
            QueueClient q = ironMq.Queue("my_queue");
            string messageId = q.Post("some data");
            Console.WriteLine("Posted message with id {0}", messageId);
        }

        private static void TestReservation(IronMqRestClient ironMq)
        {
            var q = ironMq.Queue("my_reservable_queue");
            q.Post("1");
            q.Post("2");
            q.Post("3");
            var msg = q.Reserve();
            var messages = q.Reserve(wait: 12);
            foreach (var message in messages.Messages)
            {
                Console.WriteLine(message.ReservationId);
            }
            Console.WriteLine(msg.ReservationId);
        }

        private static void TestDeletingReservedMessage(IronMqRestClient ironMq)
        {
            var q = ironMq.Queue("my_msg_deletable_queue");
            q.Clear();
            q.Post("1");
            q.Post("2");

            var message = q.Reserve();
            message.Delete();
            Console.WriteLine("Size of Q should be eq to one: {0}", q.Info().Size);
        }

        private static void TestClearingQueue(IronMqRestClient ironMq)
        {
            var q = ironMq.Queue("my_clearable_queue");
            q.Post("1");
            q.Post("2");
            q.Post("3");
            Console.WriteLine(" >>> {0}", q.Info().Size);
            q.Clear();
            Console.WriteLine(" >>> {0}", q.Info().Size);

        }
        private static void TestDeletingQueue(IronMqRestClient ironMq)
        {
            var q = ironMq.Queue("my_deletable_queue");
            q.Post("1");
            var result = q.Delete();
            var info = q.Info();
            Console.WriteLine("Should be null: {0}", info == null);
        }

        private static void TestGettingListQueue(IronMqRestClient ironMq)
        {
            var names = new[] { "a", "b", "c", "d", "e" };
            foreach (var name in names)
            {
                var queue = ironMq.Queue(name);
                queue.Post("1");
            }

            var queues = ironMq.Queues();
            foreach (var queueInfo in queues)
                Console.WriteLine(queueInfo.Name);

            var pagedQueues = ironMq.Queues(new MqPagingFilter { PerPage = 4, Previous = "c" });
            foreach (var queueInfo in pagedQueues)
                Console.WriteLine(queueInfo.Name);
        }

        private static void TestGettingMessageById(IronMqRestClient ironMq)
        {
            var q = ironMq.Queue("my_test_queue");
            q.Clear();
            var text = "some text " + DateTime.Now.Millisecond;
            var id = q.Post(text);
            var message = q.Get(id);
            Console.WriteLine("Text of mesage should be eq to \"{0}\": {1}", text, message.Body);
        }

        private static void TestPeekingMessage(IronMqRestClient ironMq)
        {
            var q = ironMq.Queue("my_peekable_queue");
            q.Clear();
            q.Post("1");
            q.Post("2");

            var m1 = q.PeekNext();
            var m2 = q.PeekNext();
            Console.WriteLine("Ids of messages should be equal: {0} == {1}", m1.Id, m2.Id);
        }

        private static void TestDeletingMessage(IronMqRestClient ironMq)
        {
            var q = ironMq.Queue("my_msg_deletable_queue");
            q.Clear();
            q.Post("1");
            q.Post("2");

            var message = q.PeekNext();
            message.Delete();
            Console.WriteLine("Size of Q should be equal to one: {0}", q.Info().Size);
        }

        private static void TestDeletingMessages(IronMqRestClient ironMq)
        {
            var q = ironMq.Queue("my_msg_deletable_queue");
            q.Clear();
            q.Post("1");
            q.Post("2");
            q.Post("3");

            var ms = q.Reserve(3, 0);
            q.Delete(ms);
            //     or
            //q.Delete(ms.Messages.ConvertAll(m => m.Id));
            Console.WriteLine(" >> Size of Q should be eq to zero: {0}", q.Info().Size);
        }

        private static void TestTouching(IronMqRestClient ironMq)
        {
            var q = ironMq.Queue("my_touchable_queue");
            q.Post("1");
            q.Post("2");
            q.Post("3");
            var msg = q.Reserve();
            Console.WriteLine(msg.ReservationId);
            Thread.Sleep(2000);
            msg.Touch();
        }

        private static void TestReleasing(IronMqRestClient ironMq)
        {
            var q = ironMq.Queue("my_releasable_queue");
            q.Post("1");
            q.Post("2");
            var msg = q.Reserve();
            Console.WriteLine(msg.ReservationId);
            var result = msg.Release();
            Console.WriteLine(" >>> {0}Released", result ? "" : "Not ");
        }

        private static void ReservedUntilTest(IronMqRestClient ironMq)
        {
            var q = ironMq.Queue("my_reserved_until_queue");
            q.Clear();
            q.Post("1");
            var msg = q.Reserve();
            DateTime t1 = DateTime.Now;

            QueueMessage msg2 = null;
            while (msg2 == null)
            {
                Thread.Sleep(1000);
                msg2 = q.Reserve();
            }
            DateTime t2 = DateTime.Now;
            Console.WriteLine("Ids: {0} {1}", msg.Id, msg2.Id);
            Console.WriteLine("Time: {0}", t2 - t1);
        }
        private static void TestTouchRelease(IronMqRestClient ironMq)
        {
            var q = ironMq.Queue("my_toasdf_queue");
            q.Post("1");
            var msg1 = q.Reserve();
            msg1.Release();
            Thread.Sleep(100);
            var msg2 = q.Reserve();
            msg2.Release();
            Console.WriteLine("{0} {1}", msg1.Id, msg1.ReservationId);
            Console.WriteLine("{0} {1}", msg2.Id, msg2.ReservationId);
        }

        private static void TestUpdatingTheQueue(IronMqRestClient ironMq)
        {
            var q = ironMq.Queue("my_msg_updateable_queue");
            q.Post("1");
            var info = q.Update(new QueueInfo {PushType = PushType.Pull, MessageTimeout = 58, MessageExpiration = 603333});

        }

    }

    public class SampleClass
    {
        public string Name { get; set; }
    }
}