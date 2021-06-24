# WhosBlockingMyWindowsShutdown

Have you ever tried to shutdown or reboot your windows machine, only to see a message that some application with a cryptic name is blocking your shutdown?

I sure have

And when you try to find out which application causes this, it is already gone.

Of course, there isnt really a problem with that, but some curious or security conscious people might wonder what weird application is running on their pc, and if it might be something to be concerned about.

I spent a lot of time trying to look up which application is blocking the shutdown with the message "Testing your devices", but i came up blank.

So I wrote this. It is a c# tool that continually polls windows for all Window Names and their respective process names and paths, and prints them to the screen / logs them to a file. The application also prevents itself from being closed via the X, so it can continue to run while the pc is being shut down, and find the offending window, even if it is only created at the last second.

-- rant over.

To use this app: Download and run it, look through the list to see if you already find the offending string, if not try to shut down the PC, then cancel it once you see "This app is preventing shutdown" and then look through the list/log again.

