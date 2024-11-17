docker build -f GameBot.Telegram/Dockerfile -t gamebot .
docker tag gamebot nordpoint.azurecr.io/gamebot:latest
docker push nordpoint.azurecr.io/gamebot:latest
pause