# Dockerfile для Sally.DiscordBot
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Sally.DiscordBot/Sally.DiscordBot.csproj", "Sally.DiscordBot/"]
RUN dotnet restore "Sally.DiscordBot/Sally.DiscordBot.csproj"
COPY . .
WORKDIR "/src/Sally.DiscordBot"
RUN dotnet build "Sally.DiscordBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sally.DiscordBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir -p /etc/sally

ENTRYPOINT ["dotnet", "Sally.DiscordBot.dll"]
