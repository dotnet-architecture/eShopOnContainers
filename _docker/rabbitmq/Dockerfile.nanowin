#https://github.com/spring2/dockerfiles/tree/master/rabbitmq

FROM microsoft/windowsservercore

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
 
ENV chocolateyUseWindowsCompression false

RUN iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1')); \
    choco install -y curl;

RUN choco install -y erlang
ENV ERLANG_SERVICE_MANAGER_PATH="C:\Program Files\erl8.2\erts-8.2\bin"
RUN choco install -y rabbitmq
ENV RABBITMQ_SERVER="C:\Program Files\RabbitMQ Server\rabbitmq_server-3.6.5"

ENV RABBITMQ_CONFIG_FILE="c:\rabbitmq"
COPY rabbitmq.config C:/
COPY rabbitmq.config C:/Users/ContainerAdministrator/AppData/Roaming/RabbitMQ/
COPY enabled_plugins C:/Users/ContainerAdministrator/AppData/Roaming/RabbitMQ/


EXPOSE 4369
EXPOSE 5672
EXPOSE 5671
EXPOSE 15672

WORKDIR C:/Program\ Files/RabbitMQ\ Server/rabbitmq_server-3.6.5/sbin
CMD .\rabbitmq-server.bat