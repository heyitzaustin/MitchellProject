# MitchellProject
Project for Mitchell

Tools used:
.NET stack, C#, WCF, IIS, SQL Server Express (LocalDB)

There are two github repositories that are a part of this project. 

One is a visual studio solution that contains the WCF services to be hosted through IIS. To start the webservices the hosting computer
should have Visual Studio with C# installed, IIS Express, and SQL Server 2014 or later. Simply open the solution visual studio and
run the code from there and the services should be up and running.

Secondly we have the visual studio solution that contains the client that will be acting as various roles, primarily as an example of
an end user accessing these service endpoints in the previous solution. This client contains a service reference to what we'll be using,
along with all the test cases that will be accessing our webservice. To run this solution, just open it in visual studio and run the 
console application while the services are running in the previous instance.

If using seperate computers, they should be on the same network so that the client can see the service running.

If running both solutions on the same computer, one way I've found in starting both is to open Visual Studio normally with one of the 
solutions, then open another instance of visual studio(as an administrator) and open the other solution. Run the WCF web services first 
(MitchellProject) and then the client (MitchellProjectClient).

