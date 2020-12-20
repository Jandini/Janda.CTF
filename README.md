# Janda.CTF

**Capture The Flag workbench** is a console application wrapper. The workbench allows to create, run and work on CTF challenges as independent services. The challenge service is represented by a class with `IChallenge` implementation. 




## Visual Studio Quick Start

1. Create `QuickStart` **Console App (.NET Core)**.
2. Add **Janda.CTF** nuget package.
3. Add **CTF.Run** to `Program.cs`.

###### Program.cs
```C#
using Janda.CTF;

namespace QuickStart
{
    class Program
    {
        static void Main(string[] args) => CTF.Run(args);
    }
}
```


4. Run program with `Add` profile to create `C001` challenge
###### launchSettings.json 

*CTF workbench nuget package provides default profiles* 

```JSON
{
  "profiles": {
    "Add": {
      "commandName": "Project",
      "commandLineArgs": "add --name C001"
    },
    "Help": {
      "commandName": "Project",
      "commandLineArgs": "--help"
    }
  }
}
```

5. Implement **Run** for `C001` challenge.

###### C001.cs

*The challenge class was added by CTF workbench*.

```C#
using Microsoft.Extensions.Logging;

namespace Janda.CTF
{  
    public class C001 : IChallenge
    {
        private readonly ILogger<C001> _logger;

        public C001(ILogger<C001> logger)
        {
            _logger = logger;
        }
        
        public void Run()
        {
            _logger.LogInformation("I will capture the flag!");
        }
    }
}
```

6. Run the challenge with `C001` profile
###### launchSettings.json

*C001 profile was added by CTF workbench*

```JSON
{
  "profiles": {
    "Add": {
      "commandName": "Project",
      "commandLineArgs": "add --name C001"
    },
    "Help": {
      "commandName": "Project",
      "commandLineArgs": "--help"
    },
    "C001": {
      "commandName": "Project",
      "commandLineArgs": "--name C001"
    }
  }
}
```
###### Output
```
[22:24:33 VRB] Running CTF workbench 1.0.0
[22:24:33 VRB] Running challange C001
[22:24:33 INF] I will get the flag!
[22:24:33 VRB] Challenge C001 completed
```

7. Create your services. For example `FlagFinder` service.

###### IFlagFinder.cs

```C#
namespace QuickStart
{
    public interface IFlagFinder
    {
        void FindFlag();
    }
}
```

###### FlagFinder.cs
```C#
using Microsoft.Extensions.Logging;

namespace QuickStart
{
    public class FlagFinder : IFlagFinder
    {
        readonly ILogger<FlagFinder> _logger;

        public FlagFinder(ILogger<FlagFinder> logger)
        {
            _logger = logger;
        }

        public void FindFlag()
        {
            _logger.LogInformation("Looking for a flag...");
        }
    }
}
```

8. Add your service to CTF workbench
###### Program.cs
```C#
using Janda.CTF;
using Microsoft.Extensions.DependencyInjection;

namespace QuickStart
{
    class Program
    {
        static void Main(string[] args) => CTF.Run(args, (services) =>
        {
            services.AddTransient<IFlagFinder, FlagFinder>();
        });
    }
}
```

9. Add your service to the challenge service
###### C001.cs
```C#
using Microsoft.Extensions.Logging;
using QuickStart;

namespace Janda.CTF
{  
    public class C001 : IChallenge
    {
        readonly ILogger<C001> _logger;
        readonly IFlagFinder _flagFinder;

        public C001(ILogger<C001> logger, IFlagFinder flagFinder)
        {
            _logger = logger;
            _flagFinder = flagFinder;
        }

        public void Run()
        {
            _logger.LogInformation("I will capture the flag!");
            _flagFinder.FindFlag();
        }
    }
}

```

10. Run `C001` challenge with `IFlagFinder` service
###### Output
```
[23:01:20 VRB] Running CTF Workbench 1.0.0
[23:01:20 VRB] Running challange C001
[23:01:20 INF] I will capture the flag!
[23:01:20 INF] Looking for a flag...
[23:01:20 VRB] Challenge C001 completed
```

