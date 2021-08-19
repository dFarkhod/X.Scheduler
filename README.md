# X.Scheduler
A Scheduler solution for Company X
The Solution consists of Web API and Web UI tiers.
Web API project is created with Asp.Net Web API in .Net Core
Web UI project is based on Angular

[![Build status](https://ci.appveyor.com/api/projects/status/q4qtfi4tvk5r6fa2?svg=true)](https://ci.appveyor.com/project/dFarkhod/x-scheduler)

Here are the requirements:
* Background 

All engineers in Company X take it in turns to support the business for half a day at a time. Currently, generating a schedule that shows who’s turn is it to support the business is being done manually and we need to automate that! 

* Task 

Your task is to design and build an online tool to choose two random developer for half day respecting the bellow rules and assumptions and show the schedule for the next two weeks.

* Assumptions 

You can assume that Company X has 10 engineers. 
You can assume that the schedule will span two weeks and start on the first working day of the upcoming week. 

* Rules 

An engineer can do at most one-half day shift in a day.
An engineer cannot have two afternoon shifts on consecutive days. 
Each engineer should have completed one whole day of support in any 2 week period. 
If an engineer work on two consecutive days are eligible to get two days exemption.

Important note

These rules are liable to change in the future, so make sure your design is flexible enough to be able to add new rules. 

* Deliverables 

At the end of the assignment, the following must be included in the repository: 
- A consumable API that returns the schedule. 
- A simple Presentation Layer (Front-end) that shows the schedule. 
UI is not an important part of the assessment, any presentation layer that shows the date of the shifts and which engineer is doing which shift is OK. 

* Non-functional requirements

Code quality matters. Please show us what code quality means for you. The more of that you do, the more you’ll be able to own our services. 
It is important that you document your decisions as you will be expected to talk through your approach in our further sessions. 



