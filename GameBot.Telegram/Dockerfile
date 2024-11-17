# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy the solution and restore dependencies
COPY ./GameBot.sln ./
COPY ./GameBot.Telegram/GameBot.Telegram.csproj ./GameBot.Telegram/
COPY ./GameBot.Core/GameBot.Core.csproj ./GameBot.Core/
RUN dotnet restore ./GameBot.Telegram/GameBot.Telegram.csproj

# Copy the rest of the source code and build the app
COPY . ./
WORKDIR /source/GameBot.Telegram
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GameBot.Telegram.dll"]
