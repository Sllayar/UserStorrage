#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["UserStorrage6/UserStorrage6.csproj", "UserStorrage6/"]
RUN dotnet restore "UserStorrage6/UserStorrage6.csproj"
COPY . .
WORKDIR "/src/UserStorrage6"
RUN dotnet build "UserStorrage6.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserStorrage6.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserStorrage6.dll"]