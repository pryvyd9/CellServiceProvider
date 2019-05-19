FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["WebClient22/WebClient22.csproj", "WebClient22/"]
RUN dotnet restore "WebClient22/WebClient22.csproj"
COPY . .
WORKDIR "/src/WebClient22"
RUN dotnet build "WebClient22.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WebClient22.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebClient22.dll"]