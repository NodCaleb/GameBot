docker build -f GameBot.Telegram/Dockerfile -t gamebot .
docker tag gamebot nordpoint.azurecr.io/gamebot:latest
az acr login --name nordpoint
docker push nordpoint.azurecr.io/gamebot:latest
pause