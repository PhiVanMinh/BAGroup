#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ReportDataGrpcService/ReportDataGrpcService.csproj", "ReportDataGrpcService/"]
COPY ["Services.Common/Services.Common.csproj", "Services.Common/"]
RUN dotnet restore "ReportDataGrpcService/ReportDataGrpcService.csproj"
COPY . .
WORKDIR "/src/ReportDataGrpcService"
RUN dotnet build "ReportDataGrpcService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReportDataGrpcService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReportDataGrpcService.dll"]