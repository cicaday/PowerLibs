# C# Logger lib

A quick way to start to use log feature for your appliation.

## Usage

*Add log4net.dll to your project as a reference
*Add Log.cs to any folder in your C# project
*Copy the Config/log4net.xml to your project, keep the structure or adjust on your demmand.
**You might need to update the path in Log.cs a little bit.
*Start to use the Log feature

```
Log.Info("Hello Info");
Log.Warn("Hello Warnning!");
Log.Error("Hello Error :(");
```

This lib will generate 2 log files in your working directory.
*Debug_date.log: contains all level of log messsage.
*Error_date.log: contains only error level message, can help locate to error in a fast way.

Enjor it.

## License

[MIT](http://opensource.org/licenses/MIT)
