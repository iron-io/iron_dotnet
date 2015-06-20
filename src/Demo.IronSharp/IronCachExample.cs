using System;
using System.Threading.Tasks;
using IronIO.Core;
using IronIO.Core.Extensions;
using IronIO.IronCache;

namespace Demo.IronSharpConsole
{
    internal static class IronCachExample
    {
        public static void Main()
        {
            //Run();

            RunAsync().Wait();

            Console.WriteLine("done");
            Console.Read();
        }

        public static async Task RunAsync()
        {
            Console.WriteLine("Begin Async Cache Test");

            // =========================================================
            // Iron.io Cache
            // =========================================================

            var ironCacheClient = Client.New();

            // Get a Cache object
            var cache = ironCacheClient.Cache("my_cache");

            // Put value to cache by key
            await cache.Put("number_item", 42).SendAsync();

            var item = await cache.Get("number_item").SendAsync();

            // Get value from cache by key
            Console.WriteLine(item.Value);

            // Get value from cache by key
            Console.WriteLine(await cache.Get<int>("number_item").SendAsync());

            // Numbers can be incremented
            await cache.Increment("number_item", 10).SendAsync();

            // Immediately delete an item
            await cache.Delete("number_item").SendAsync();

            await cache.Put("complex_item", new {greeting = "Hello", target = "world"}).SendAsync();

            var complexItem = await cache.Get("complex_item").SendAsync();

            // Get value from cache by key
            Console.WriteLine(complexItem.Value);

            await cache.Delete("complex_item").SendAsync();

            await cache.Put("sample_class", new SampleClass {Name = "Sample Class CacheItem"}).SendAsync();

            var sampleClassItem = await cache.Get<SampleClass>("sample_class").SendAsync();

            Console.WriteLine(sampleClassItem.Inspect());

            await cache.Delete("sample_class").SendAsync();
        }

        public static void Run()
        {
            Console.WriteLine("Begin Synchronous Cache Test");

            // =========================================================
            // Iron.io Cache
            // =========================================================

            var ironCacheClient = Client.New();

            // Get a Cache object
            var cache = ironCacheClient.Cache("my_cache");

            // Put value to cache by key
            var ironTask = cache.Put("number_item", 42);

            ironTask.Send();

            var item = cache.Get("number_item").Send();

            // Get value from cache by key
            Console.WriteLine(item.Value);

            // Get value from cache by key
            Console.WriteLine(cache.Get<int>("number_item").Send());

            // Numbers can be incremented
            cache.Increment("number_item", 10).Send();

            // Immediately delete an item
            cache.Delete("number_item").Send();

            cache.Put("complex_item", new {greeting = "Hello", target = "world"}).Send();

            var complexItem = cache.Get("complex_item").Send();

            // Get value from cache by key
            Console.WriteLine(complexItem.Value);

            cache.Delete("complex_item").Send();

            cache.Put("sample_class", new SampleClass {Name = "Sample Class CacheItem"}).Send();

            var sampleClassItem = cache.Get<SampleClass>("sample_class").Send();

            Console.WriteLine(sampleClassItem.Inspect());

            cache.Delete("sample_class").Send();
        }
    }
}