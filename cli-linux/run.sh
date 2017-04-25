#!/bin/bash
docker stop $(docker ps -a -q)
docker rm $(docker ps -a -q)
docker images |grep -v REPOSITORY|awk '{print $1}'|xargs -L1 docker pull
export ESHOP_PROD_EXTERNAL_DNS_NAME_OR_IP=$(curl ipinfo.io/ip)
docker-compose up -f docker-compose.images.yml -f docker-compose.prod.yml -d

