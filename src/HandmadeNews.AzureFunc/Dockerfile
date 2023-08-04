#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet:4 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HandmadeNews.AzureFunc/HandmadeNews.AzureFunc.csproj", "HandmadeNews.AzureFunc/"]
COPY ["HandmadeNews.Infrastructure/HandmadeNews.Infrastructure.csproj", "HandmadeNews.Infrastructure/"]
COPY ["HandmadeNews.Domain/HandmadeNews.Domain.csproj", "HandmadeNews.Domain/"]
RUN dotnet restore "HandmadeNews.AzureFunc/HandmadeNews.AzureFunc.csproj"
COPY . .
WORKDIR "/src/HandmadeNews.AzureFunc"
RUN dotnet build "HandmadeNews.AzureFunc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HandmadeNews.AzureFunc.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true