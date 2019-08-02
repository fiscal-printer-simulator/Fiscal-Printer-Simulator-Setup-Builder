FROM mcr.microsoft.com/windows/servercore:10.0.17763.615-amd64
ADD Delivery c:/Delivery/
WORKDIR c:/Delivery
ENV FP_SERVICE_PORT 8283
RUN msiexec.exe /qn /i SetupFiscalPrinterSimulatorx64-en_US.msi
RUN curl "https://netix.dl.sourceforge.net/project/com0com/com0com/3.0.0.0/com0com-3.0.0.0-i386-and-x64-signed.zip" -H "Upgrade-Insecure-Requests: 1" -H "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36" -H "Referer: https://sourceforge.net/projects/com0com/files/latest/download" --output com2com.zip
RUN powershell.exe Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::ExtractToDirectory('com2com.zip', 'com2com');
WORKDIR c:/Delivery/com2com
RUN dir
RUN msiexec.exe /qn /i Setup_com0com_v3.0.0.0_W7_x64_signed.exe
EXPOSE 8182
ENTRYPOINT [ "powershell.exe"]