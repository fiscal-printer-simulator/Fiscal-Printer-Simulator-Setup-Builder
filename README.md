# Fiscal Printer Simulator
This is Open Source software project thats simulate __Real World Fiscal Printers__.

This project was created for improve developing POS-like application. 
Developers who want create application witch comunicate with Fiscal Printer device don't need to buy expensive devices for test own sowftware.

_Application is on Open Source Licence (__GNU General Public License v3.0__)_  

> _If you find any bugs or have an idea to improve the application, please report the issue.   
If you would like to participate in developing the application, it is also possible._  

### Covered Fiscal Printer Protocols:
- [x] - Thermal PosNet
- [ ] - New PosNet

### Project will contains these components:
- [x] Fiscal Printer Simulator ( Windows Service application)
- [x] Fiscal Printer Simulator Client ( Desktop/Web application) 
- [ ] .NET Library to comunicate with Fiscal Printer / Fiscal Printer Simulator
- [ ] Fiscal Print Server ( REST Microservice) 



### Releases: 
- #### 0.1.0 (alfa version)
  - Fiscal Printer Simulator (x86) Windows Installer - [Download](http://google.pl)
  - Fiscal Printer Simulator (x64) Windows Installer - [Download](http://google.pl)
  - Fiscal Printer.NET Communication Librarary [Nuget Package](http://)
  - Windows Docker image for working with Fiscal Printer Service with Docker [Docker HUB](http://hub.docker.com) 


### Integration & Communication:
- For working with this application you need to have one of below configuration:  
  _a)_  Connected real fiscal printer ( .Net library / Fiscal Print Microservice)   
  _b)_  Bridge on ports ( For example COM1 and COM2)  
  _c)_  Driver to create bridge on Serial ports [example driver to download](https://www.virtual-serial-port.org/)  
    
- The Fiscal Print Server is not nesssesary to Fiscal Printer Simulator working.  
  *This is feature for communicating with fiscal printer by REST api.*  
- Communication between FiscalPrinterSimulatorService and FiscalPrinterSimulatorClient is resolved by __Websocket__ protocol. 
 (Default port is _:8080_ but it can be changed in configuration file)  
- Installators will be  builded for run "_Installer_" Configuration of main solution.  
  _In future it will be replaced by CI/CD Scripts with Building Docker Image._ 
 
 ### Warnings:
 - Fiscal Printer Simulator Client can be translated to other then _US-en_ language. (for now _pl-PL_ is supported too).  
   But printouts and eventually byte frame responses from Fiscal Printer will be not translated. 
   
   
# Have a nice use :thumbsup:
