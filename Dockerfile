FROM mcr.microsoft.com/windows/servercore:10.0.17763.615-amd64
COPY .\Delivery\Instalators\  C:\Installers\
RUN C:\Installers\ msiexec.exe /q /n /i ".\x64\en-US\SetupFiscalPrinterSimulator.msi"
EXPOSE 8181
ENTRYPOINT [ "powershell.exe"]