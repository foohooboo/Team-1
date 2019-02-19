# Stock Market Simulator

System consists of 3 processes: 
  1. StockServer - Streams simulated stock-price data.
  2. Broker - Facilitates stock portfolio managament.
  3. Client - Graphical interface for users to manage thier stock portfolios.
  
There is one root Visual Studio Solution (_\<repo\>_/Project/StockMarketSimulator.sln) which does not include any tests. Additionally, each process (and a shared project) have their own solutions with tests. These can be found in (_\<repo\>_/Project/\<project\>/\<project\>.sln)

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

  * .Net 4.7.1 Shared Project
