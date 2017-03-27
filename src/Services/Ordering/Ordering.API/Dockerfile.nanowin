FROM microsoft/dotnet:1.1-runtime-nanoserver
SHELL ["powershell"]
ARG source
WORKDIR /app
RUN set-itemproperty -path 'HKLM:\SYSTEM\CurrentControlSet\Services\Dnscache\Parameters' -Name ServerPriorityTimeLimit -Value 0 -Type DWord
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "Ordering.API.dll"]
