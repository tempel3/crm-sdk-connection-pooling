# Overview

If you experience long request times in your web application without a high server load you should check processing of Microsoft Dynamics CRM requests. These are processed serially. 

Since the CRM SDK has some strange logic, which does not handle parallelism very well, i needed a proper solution for connection pooling.

Here is my connection pooling mechanism for `ASP.NET MVC 5`.

# Gettings Started

Setup you connection string in the `.env` file.

Have a look in the `Global.asax.cs` for the configuration.

Just run the application.