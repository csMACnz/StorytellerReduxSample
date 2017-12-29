Storyteller with React Redux Tests Sample
=========================================

This is a sample test based on an article from Jeremy Miller on integrating Storyteller with React.
https://jeremydmiller.com/2017/12/19/subcutaneous-testing-against-react-net-applications/

* Copied the source for the Storyteller.Redux project (to modify if necessary)
* Copied the ReactSamples project from Storyteller
* Created a new dotnet aspnetcore react redux app  (`dotnet new reactredux`)
* Connected up ChromeDriver for selenium
* Added the `reduxharness.ts` to webapp (including changes in `boot-client.tsx`)
* Added ability for Startup to force Webpack mode (for use by test harness)
* tweaked some settings in ReactSamples to be able to `Startup` the web application
* added force send current state functionality to Storyteller.Redux (after initial connection)
* Added tests to increment the counter
* Works!

A more detailed description of how I made this work is on my blog: [Subcutaneous Testing against React + .Net Applications with Storyteller - A Reply](https://blog.csmac.nz/storytellerreduxsample/)
