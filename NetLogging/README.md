# Solution NetLogging

## Project Log4NetLogger
contains a sample usage of the Log4Net logging system in .NET 8 applications. The default location is %PROGRAMDATA%\Examples.

## Project ReporterLibrary
contains a shared project that generates the required number of log entries.

## Project SerilogLogger
contains a sample usage of the Serilog logging system in .NET 8 applications. The default location is %PROGRAMDATA%\Examples.

## How to use this solution:

1. Select Log4NetLogger as the default project and run it. The application's output will display the total time spent logging and the latency of a single entry.

2. Select SerilogLogger as the default project and run it. The application's output will display the total time spent logging and the latency of a single entry.

By comparing the results, you will find that Serilog is about 50% faster, so latency for writing individual log item is 50% shorter.

## How to test structured logging:

### What is structured logging?

Structured logging is a feature that allows you to log the public properties of objects in JSON format. Subsequently, these values can be parsed and only objects that match the filter can be found.

### Steps to enable structured logging in the example:

1. Open the file ReporterLibrary/Reporter.cs.

2. Change the original code:
``` 
// Uncomment following lines to get structured logging
// var car = new Car();
// _logger.LogInformation("Logging information message number {Count} for {@car}", i, car);

// comment following line to get structured logging
_logger.LogInformation("Logging information message number {Count}", i);
``` 
to
``` 
// Uncomment following lines to get structured logging
var car = new Car();
_logger.LogInformation("Logging information message number {Count} for {@car}", i, car);

// comment following line to get structured logging
// _logger.LogInformation("Logging information message number {Count}", i);
``` 
3. Run the project SerilogLogger.
4. Open the latest log file %PROGRAMDATA%\Examples\Serilog.*.txt.
5. In the log, you will also see the public properties of the Car object serialized into the JSON file.
6. You can create simple parser and select only objects which match desired criteria.



