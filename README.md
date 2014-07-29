![Products#](https://raw.github.com/iron-io/iron_dotnet/master/images/products.png)

==========

# IronSharp is a .NET client for [Iron.io](http://www.iron.io/)

Forked from [grcodemonkey/iron_sharp](https://github.com/grcodemonkey/iron_sharp)

## Getting Started

1. Sign up at <https://hud.iron.io/users/new>
2. Get your credentials from [https://hud.iron.io/dashboard](https://hud.iron.io/dashboard "Heads up")
3. Check out the [wiki](https://github.com/grcodemonkey/iron_sharp/wiki "For those that like to read directions")


## IronMQ
<http://dev.iron.io/mq/>

```PM> Install-Package Iron.IronMQ```

```C#
// =========================================================
// Iron.io MQ
// =========================================================

IronMqRestClient ironMq = IronSharp.IronMQ.Client.New();

// Get a Queue object
QueueClient queue = ironMq.Queue("my_queue");

QueueInfo info = queue.Info();

Console.WriteLine(info.Inspect());

// Put a message on the queue
string messageId = @queue.Post("hello world!");

Console.WriteLine(messageId);

// Get a message
QueueMessage msg = queue.Get(n:30, timeout: 60, wait: 100);

Console.WriteLine(msg.Inspect());

//# Delete the message
bool deleted = msg.Delete();

Console.WriteLine("Deleted = {0}", deleted);

// Post several messages
queue.Post(new[] { "Hello", "world" });

MessageCollection messages = queue.Get(n: 2, timeout: 60, wait: 30);
// You can specify only parameters you need:
// MessageCollection messages = queue.Get(wait: 15);

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

MessageIdCollection queuedUp = queue.Post(new[] {payload1, payload2, payload3});

Console.WriteLine(queuedUp.Inspect());

QueueMessage next;

while (queue.Read(out next))
{
    Console.WriteLine(next.Inspect());
    Console.WriteLine(next.Delete());
}
```

## IronWorker
<http://dev.iron.io/worker/>

```PM> Install-Package Iron.IronWorker```

### Overview

```C#
// =========================================================
// Iron.io Worker
// =========================================================

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
```

### Queueing a Task

```
string taskId = workerClient.Tasks.Create("Test", payload, options);
```

Where `payload` could be any object:

```
var payload = new {environment = "development", names = new String[]{"Bob", "Alice"}};
```

There are following possible options:

  - **Priority**: The priority queue to run the job in. Valid values are `TaskPriority.Default` (0), `TaskPriority.Medium` (1), and `TaskPriority.High` (2). The default is 0.
  - **Timeout**: The maximum runtime of your task in seconds. No task can exceed 3600 seconds (60 minutes). The default is 3600 but can be set to a shorter duration.
  - **Delay**: The number of seconds to delay before actually queuing the task. Default is 0.

### Scheduling Options

You can append to `ScheduleBuilder.Build()` (i.e. instance of ScheduleOptionsBuilder) the following methods:

  - **WithFrequency**: The amount of time specified with timespan, between runs.  By default, the task will only run once. It will return a 400 error if it is set to less than 60. Original API parameter name is `run_every`.
  - **StopAt**: The time tasks will stop being queued. Should be an instance of DateTime. Original API parameter name is `end_at`.
  - **StopAfterNumberOfRuns**: The number of times a task will run. Original API parameter name is `run_times`
  - **WithPriority**: The priority queue to run the job in. Valid values are `TaskPriority.Default` (0), `TaskPriority.Medium` (1), and `TaskPriority.High` (2). The default is 0. Higher values means tasks spend less time in the queue once they come off the schedule. Original API parameter name is `priority`
  - **StartingOn**: The time the scheduled task should first be run. Should be an instance of DateTime. Original API parameter name is `start_at`.
  - **RunFor**: The amount of time specified with timespan scheduled task should be run for. The same as `StopAt(DateTime.Now + duration)`
  - **Delay**: The amount of time execution should be delayed. The same as `StartingOn(DateTime.Now + delay)`
  - **NeverStop**: Disables effects from previously called `StopAt` and `StopAfterNumberOfRuns`.


## IronCache
<http://dev.iron.io/cache/>

```PM> Install-Package Iron.IronCache```

```C#
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

cache.Put("complex_item", new {greeting = "Hello", target = "world"});

// Get value from cache by key
Console.WriteLine(cache.Get("complex_item").Value);

cache.Delete("complex_item");
```
