#!/bin/sh

docker build . -t docker.io/jozseftorocsik/szakdolgozat-api --no-cache
docker push docker.io/jozseftorocsik/szakdolgozat-api


helm uninstall api -n apps
helm install api ./helm/api -n apps