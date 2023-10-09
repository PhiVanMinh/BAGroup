#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WebApi/BA.API.csproj", "WebApi/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["BA-API/Services.Common/Services.Common.csproj", "BA-API/Services.Common/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Infra_Persistence/Infra_Persistence.csproj", "Infra_Persistence/"]
RUN dotnet restore "WebApi/BA.API.csproj"
COPY . .
WORKDIR "/src/WebApi"
RUN dotnet build "BA.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BA.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BA.API.dll"]