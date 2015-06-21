using System;
using System.Threading.Tasks;
using IronIO.Core.Extensions;
using IronIO.IronMQ;

namespace Demo.IronSharpConsole
{
    internal class IronMqExample
    {
        public static async Task RunAsync()
        {
            // =========================================================
            // Iron.io MQ
            // =========================================================

            var ironMq = Client.New();

            // Get a Queue object
            var queue = ironMq.Queue("my_queue");

            var info = await queue.Info().SendAsync();

            Console.WriteLine(info.Inspect());

            // Put a message on the queue
            var messageId = await queue.Post("hello world!").SendAsync();

            Console.WriteLine(messageId);

            // Get a message

            var msg = await queue.ReserveNext(new ReservationOptions { Timeout = 60, Wait = 100 }).SendAsync();

            Console.WriteLine(msg.Inspect());

            //# Delete the message
            var deleted = await msg.Delete().SendAsync();

            Console.WriteLine("Deleted = {0}", deleted);

            // Post several messages
            queue.Post(new[] { "Hello", "world" });

            var messages = await queue.Reserve(new MessageReservationOptions { Number = 2, Timeout = 60, Wait = 30 }).SendAsync();

            // Post several messages
            var payload1 = new
            {
                message = "hello, my name is Iron.io 1"
            };

            var payload2 = new
            {
                message = "hello, my name is Iron.io 2"
            };

            var payload3 = new
            {
                message = "hello, my name is Iron.io 3"
            };

            var queuedUp = await queue.Post(new[] { payload1, payload2, payload3 }).SendAsync();

            Console.WriteLine(queuedUp.Inspect());

            QueueMessage next;

            do
            {
                next = await queue.ReserveNext().SendAsync();

                if (next != null)
                {
                    Console.WriteLine(next.Inspect());
                    Console.WriteLine(await next.Delete().SendAsync());
                }
            } while (next != null);
        }

        public static void Run()
        {
            // =========================================================
            // Iron.io MQ
            // =========================================================

            var ironMq = Client.New();

            // Get a Queue object
            var queue = ironMq.Queue("my_queue");

            var info = queue.Info().Send();

            Console.WriteLine(info.Inspect());

            // Put a message on the queue
            var messageId = queue.Post("hello world!").Send();

            Console.WriteLine(messageId);

            // Get a message

            var msg = queue.ReserveNext(new ReservationOptions {Timeout = 60, Wait = 100}).Send();

            Console.WriteLine(msg.Inspect());

            //# Delete the message
            var deleted = msg.Delete().Send();

            Console.WriteLine("Deleted = {0}", deleted);

            // Post several messages
            queue.Post(new[] {"Hello", "world"});

            var messages = queue.Reserve(new MessageReservationOptions {Number = 2, Timeout = 60, Wait = 30}).Send();

            // Post several messages
            var payload1 = new
            {
                message = "hello, my name is Iron.io 1"
            };

            var payload2 = new
            {
                message = "hello, my name is Iron.io 2"
            };

            var payload3 = new
            {
                message = "hello, my name is Iron.io 3"
            };

            var queuedUp = queue.Post(new[] {payload1, payload2, payload3}).Send();

            Console.WriteLine(queuedUp.Inspect());

            QueueMessage next;

            while (queue.Read(out next))
            {
                Console.WriteLine(next.Inspect());
                Console.WriteLine(next.Delete().Send());
            }
        }
    }
}