#!/bin/bash
dotnet publish -r linux-arm -c Release -p:PublishSingleFile=true --self-contained true -p:PublishTrimmed=true
scp bin/Release/net5.0/linux-arm/publish/Bot-Dotnet pi@unifi:~/bot