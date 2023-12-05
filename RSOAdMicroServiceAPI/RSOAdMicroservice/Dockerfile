#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RSOAdMicroservice/RSOAdMicroservice.csproj", "RSOAdMicroservice/"]
RUN dotnet restore "./RSOAdMicroservice/./RSOAdMicroservice.csproj"
COPY . .
WORKDIR "/src/RSOAdMicroservice"
RUN dotnet build "./RSOAdMicroservice.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RSOAdMicroservice.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV ASPNETCORE_ENVIRONMENT="Release"
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RSOAdMicroservice.dll"]