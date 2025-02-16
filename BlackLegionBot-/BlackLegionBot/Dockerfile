#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["BlackLegionBot/BlackLegionBot.csproj", "BlackLegionBot/"]
RUN dotnet restore "BlackLegionBot/BlackLegionBot.csproj"
COPY . .
WORKDIR "/src/BlackLegionBot"
RUN dotnet build "BlackLegionBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlackLegionBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlackLegionBot.dll"]