FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5122

ENV ASPNETCORE_URLS=http://+:5122

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["WebAPIUdemy.csproj", "./"]
RUN dotnet restore "WebAPIUdemy.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "WebAPIUdemy.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "WebAPIUdemy.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebAPIUdemy.dll"]
