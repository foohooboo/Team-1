# Stock Market Simulator

Project board: https://trello.com/b/TfkcX7QO/design-tasks

**IMPORTANT!!** This repo uses a git submodule. After updating to the latest version, you may need to enter the following git command...

git submodule update --init --recursive

System consists of 3 processes: 
  1. StockServer - Streams simulated stock-price data.
  2. Broker - Facilitates stock portfolio managament.
  3. Client - Graphical interface for users to manage thier stock portfolios.
  
There is one root Visual Studio Solution (_\<repo\>_/Project/StockMarketSimulator.sln) which does not include any tests. Additionally, each process (and a shared project) have their own solutions with tests. These can be found in (_\<repo\>_/Project/\<project\>/\<project\>.sln)

**IMPORTANT!!** The stock server uses a public/private key for security on the shock history conversation. In order to run the stock server locally you must place the keys into the following folder:

"C:\Users\%User%\AppData\Local\Team1\StockServer"

The keys are located in the "Team-1\Project\SecurityKeys" folder in the repo. These keys are included here only for testing purposes and would not be the same as a live release.

## Setup Notes

  * This project contains NuGets. After opening the solution for the first time, you may need to "Restore NuGet Packages".

## Project Info

### StockServer

  * .Net 4.7.1 Console application

### Broker

  * .Net 4.7.1 Console application
  
### Client

  * .Net 4.7.1 WPF application
  
### Shared

  * .Net 4.7.1 Class Library
