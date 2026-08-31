FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SocialMediaDownloader.Api.csproj", "./"]
RUN dotnet restore "SocialMediaDownloader.Api.csproj"

COPY . .
RUN dotnet build "SocialMediaDownloader.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SocialMediaDownloader.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "SocialMediaDownloader.Api.dll"]