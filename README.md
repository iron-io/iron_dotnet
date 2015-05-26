![Products#](https://raw.github.com/iron-io/iron_dotnet/master/images/products.png)

==========

# IronSharp is a .NET client for [Iron.io](http://www.iron.io/)

Forked from [grcodemonkey/iron_sharp](https://github.com/grcodemonkey/iron_sharp)

## Getting Started

1. Go to http://hud.iron.io/ and sign up.
2. Create new project at http://hud.iron.io/dashboard
3. Download the iron.json file from "Credentials" block of project


# IronMQ On-Premise

**Note:** You are reading documentation of Iron.MQ v3. There are some differences from the previous version of IronMQ. For more information please go to [Iron.io Dev Center](http://dev.iron.io/mq-onpremise/).

This version is for **IronMq Version 3** and **On-Premise**. If you are using the public version please use [version 1.0.2](http://github.com/iron-io/iron_dotnet)

### Installation

```PM> Install-Package Iron.IronMQ -Pre```

### Configuration

1\. Reference the library

```C#
using IronSharp.Core;
using IronSharp.IronMQ;
```

2\. [Setup your Iron.io credentials](http://dev.iron.io/mq/reference/configuration/)

Also you need to pass authorization data to the client. There are several ways to do it:

- place `.iron.json` file to home folder eg. `C:\Users\admin\.iron.json`
- place `.iron.json` file near your executable
- instantiate IronMqRestClient by passing project id and token: `IronSharp.IronMQ.Client.New(new IronClientConfig { ProjectId = "XXXXXXX", Token = "YYYYYYY"});`

3\. Create an IronMQ client object:

```C#
var iromMq = IronSharp.IronMQ.Client.New();
```

You can specify host settings in iron.json or explicitly in code, for example:

```C#
var iromMq = IronSharp.IronMQ.Client.New(new IronClientConfig { ProjectId = "XXXXXXX", Token = "YYYYYYY", Host = "localhost", Scheme = "http", Port = 8080});
```

### Keystone Authentication

#### Via Configuration File

Add `keystone` section to your iron.json file:

```javascript
{
  "project_id": "57a7b7b35e8e331d45000001",
  "keystone": {
    "server": "http://your.keystone.host/v2.0/",
    "tenant": "some-group",
    "username": "name",
    "password": "password"
  }
}
```

#### In Code

```C#
KeystoneClientConfig keystone = new KeystoneClientConfig 
{ 
   Tenant = "people",
   Server = "http://your.keystone.host/v2.0/",
   Username = "name",
   Password = "password"
};
var ironMq = IronSharp.IronMQ.Client.New(new IronClientConfig {ProjectId = "XXXXXXX", Keystone = keystone});

```

## The Basics

### Get Queues List

```C#
var queues = ironMq.Queues();
foreach (var queueInfo in queues)
    Console.WriteLine(queueInfo.Name);
```

--

### Get a Queue Object

You can have as many queues as you want, each with their own unique set of messages.

```C#
QueueClient queue = ironMq.Queue("my_queue");
```

Now you can use it.

--

### Post a Message on a Queue

Messages are placed on the queue in a FIFO arrangement.
If a queue does not exist, it will be created upon the first posting of a message.

```C#
QueueClient queue = ironMq.Queue("my_queue");
string messageId = queue.Post("Hello World!");
```

--

### Retrieve Queue Information

```C#
QueueInfo info = queue.Info();
Console.WriteLine(info.Name);
```

--

### Get a Message off a Queue

```C#
QueueMessage message = queue.Reserve();
Console.WriteLine(message.Body);
Console.WriteLine(message.ReservationId);
```

**Note:** since v3 you should reserve message if you want to process it.

--

### Delete a Message from a Queue

```C#
QueueMessage message = queue.Reserve();
message.Delete();
```

Be sure to delete a message from the queue when you're done with it.

--

## Queues

### Retrieve Queue Information

```C#
QueueInfo info = queue.Info();
Console.WriteLine(info.Name);
Console.WriteLine(info.Size);
Console.WriteLine(info.TotalMessages);
```

QueueInfo consists of the following properties:

```C#
public class QueueInfo : IInspectable
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string ProjectId { get; set; }
    public PushType PushType { get; set; }
    public int? Retries { get; set; }
    public int? RetriesDelay { get; set; }
    public int? Size { get; set; }
    public int? TotalMessages { get; set; }
    public int? Timeout { get; set; }         // NEW!
    public int? ExpireTime { get; set; }      // NEW!
}
```

--

### Clear a Message Queue

Delete all messages from a queue without deleting a queue

```C#
queue.Clear();
```

--

### Delete a Message Queue

Delete a queue and all it's messages

```C#
queue.Delete();
```

--

## Messages

### Post Messages to a Queue

**Single message:**

```C#
var id1 = queue.Post("message");
// To control parameters like delay, pass `MessageOptions` instance.
var id2 = queue.Post("message", new MessageOptions {Delay = 20});
// or construct your own message
var id3 = q.Post(new QueueMessage("message", new MessageOptions{Delay = 20}));
```

**Multiple messages:**

You can also pass multiple messages in a single call.

```C#
queue.Post(new[] {"first", "second", "third"});
queue.Post(new object[] {1, 2, 3});
queue.Post(new MessageCollection(new[] { new QueueMessage("1"), new QueueMessage("2"), new QueueMessage("3") }));
```

To control parameters like delay, you can pass an instance of `MessageOptions` as a last parameter

```C#
queue.Post(new[] {"first", "second", "third"}, new MessageOptions{Delay = 20});
queue.Post(new object[] {1, 2, 3}, new MessageOptions{Delay = 20});
```

**Parameters:**

* `Timeout`: **Deprecated**. Timeout is not allowed since v3 for messages because it's not possible to set timeout when posting a message, only when reserving one.

* `Delay`: The item will not be available on the queue until this many seconds have passed.
Default is 0 seconds. Maximum is 604,800 seconds (7 days).

--

### Get Messages from a Queue

Since REST API changed in v3 all messages should be reserved to be processed. So, for backward compatibility method Get now reserves messages.

```C#
// All methods below reserve the message:
QueueMessage msg;
msg = q.Reserve();
msg = q.Next();
```

When you pop/get a message from the queue, it is no longer on the queue but it still exists within the system.
You have to explicitly delete the message or else it will go back onto the queue after the `timeout`.
The default `timeout` is 60 seconds. Minimal `timeout` is 30 seconds.

You also can get several messages at a time:

```C#
// reserve 5 messages
MessageCollection messages;
messages = queue.Reserve(5);
messages = queue.Reserve(5, new TimeSpan(0, 0, 10));
messages = queue.Get(5, new TimeSpan(0, 0, 10));
```

**Note:** You may not receive all n messages on every request, the more sparse the queue, the less likely you are to receive all n messages.

--

### Peek Messages from a Queue

Peeking at a queue returns the next messages on the queue, but it does not reserve them.

```C#
var message = queue.PeekNext();

// or

var messages = q.Peek(13);
```

**Optional parameters:**

* `n`: The maximum number of messages to peek. Default is 1. Maximum is 100. Note: You may not receive all n messages on every request, the more sparse the queue, the less likely you are to receive all n messages.

--

### Touch Message

You can prolongate period of message reservation.

```
message = queue.Reserve();
Thread.Sleep(10000);
message.Touch();
```

This method is not applicable for messages which not been reserved.

```C#
message = queue.PeekNext();
if (!message.Touch())
    Console.WriteLine("Message couldn't be touched");
```

--

### Release Message

Message could be returned back to queue before the expiration of reservation.

```
message = queue.Reserve();
Thread.Sleep(10000);
message.Release();
```

This method is not applicable for messages which not been reserved.

```C#
message = queue.PeekNext();
if (!message.Release())
    Console.WriteLine("Message couldn't be released");
```

You can specify the time in seconds after which message will appear in queue:

```C#
message.Release(5); // message will appear in queue after 5 seconds
```

--

### Delete Message

```
var message = queue.Reserve();
message.Delete();
```

--

## Multithread execution recomendations

Please, try not to use this client more then in 25 threads at the same time inside one process. If you need to have more threads, you can use several processes.

Also try to make not more than 50 requests per second from each thread.

It's also recommended to reuse Client instead of instantiating it for each request.

--

### Delete Messages 

Batch deleting of messages can be done via deleting MessageCollection

```C#
MessageCollection messages = q.Reserve(3);
queue.Delete(messages);
```

Or via specifying ids of messages

```C#
var id1 = queue.Reserve();
var id2 = queue.Reserve();
q.Delete(new[]{id1, id2});
```

--


# IronWorker
<http://dev.iron.io/worker/>

```PM> Install-Package Iron.IronWorker```

## Overview

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

## Queuing a Task

```
string taskId = workerClient.Tasks.Create("Test", payload, options);
```

Where `payload` could be any object:

```
var payload = new {environment = "development", names = new String[]{"Bob", "Alice"}};
```

There are following possible Options:

  - **Priority**: Setting the priority of your job. Valid values are `TaskPriority.Default` (0), `TaskPriority.Medium` (1), and `TaskPriority.High` (2). The default is 0.
  - **Timeout**: The maximum runtime of your task in seconds. No task can exceed 3600 seconds (60 minutes). The default is 3600 but can be set to a shorter duration.
  - **Delay**: The number of seconds to delay before actually queuing the task. Default is 0.

## Scheduling Options

You can append to `ScheduleBuilder.Build()` (i.e. instance of ScheduleOptionsBuilder) the following methods:

  - **WithFrequency**: The amount of time specified with timespan, between runs.  By default, the task will only run once. It will return a 400 error if it is set to less than 60. Original API parameter name is `run_every`.
  - **StopAt**: The time tasks will stop being queued. Should be an instance of DateTime. Original API parameter name is `end_at`.
  - **StopAfterNumberOfRuns**: The number of times a task will run. Original API parameter name is `run_times`
  - **WithPriority**: The priority queue to run the job in. Valid values are `TaskPriority.Default` (0), `TaskPriority.Medium` (1), and `TaskPriority.High` (2). The default is 0. Higher values means tasks spend less time in the queue once they come off the schedule. Original API parameter name is `priority`
  - **StartingOn**: The time the scheduled task should first be run. Should be an instance of DateTime. Original API parameter name is `start_at`.
  - **RunFor**: The amount of time specified with timespan scheduled task should be run for. The same as `StopAt(DateTime.Now + duration)`
  - **Delay**: The amount of time execution should be delayed. The same as `StartingOn(DateTime.Now + delay)`
  - **NeverStop**: Disables effects from previously called `StopAt` and `StopAfterNumberOfRuns`.


# IronCache
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
