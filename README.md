# Smart Platform
Semester Project (Smart Platform) for course 62413 - Advanced C# &amp; .NET

This is the personal project in DTU course 62413 - Advanced Object Oriented Programming with C# and .NET.

The project is a smart platform with support for an array of different devices, as long as these follows the definition of a SmartDeviceFirmware and acompanied by a SmartPlatformDevicePlugin.

For the project, a MAIU app will be used, to enable the user to interact with devices from all platforms. Devices will run firmware written in C# using .NET NanoFramework, and finally the two will communicate via an ASP.NET Core API backend, hosted by Azure. In the backend users and related devices are stored in an SQL Server database, also hosted in Azure. 

To define plugins and firmware, a Linked Library has been developed, which has a .dll for firmware and another .dll for platform plugins. A product can use these in tandem, to quickly develop new products. As part of the project, a smart curtain product has been implemented using these, demonstration how they fit.
