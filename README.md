![Products#](https://raw.github.com/iron-io/iron_dotnet/master/images/products.png)

==========

# IronSharp is a .NET client for [Iron.io](http://www.iron.io/)

Forked from [grcodemonkey/iron_sharp](https://github.com/grcodemonkey/iron_sharp)

## Getting Started

1. Go to http://hud.iron.io/ and sign up.
2. Create new project at http://hud.iron.io/dashboard
3. Download the iron.json file from "Credentials" block of project

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

## IronMQ

<http://dev.iron.io/mq/>

Note: You read documentation of Iron.MQ v3. There are some differences from the previous version. Check this list at [page should be published at HUD](#hud) <!-- TODO: add valid reference -->

```PM> Install-Package Iron.IronMQ``` <!-- TODO: add version -->

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
IronSharp.IronMQ.Client.New();
```

## IronWorker
<http://dev.iron.io/worker/>

```PM> Install-Package Iron.IronWorker```

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
