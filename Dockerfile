# ------------------------------
# Stage 1: Build .NET projects
# ------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy csproj files first (for caching)
COPY src/ArmA3Manager.Web/*.csproj ./ArmA3Manager.Web/
COPY src/ArmA3Manager.Application/*.csproj ./ArmA3Manager.Application/

# Restore dependencies
RUN dotnet restore ./ArmA3Manager.Web/ArmA3Manager.Web.csproj

# Copy the rest of the source code
COPY src/ ./ 

# Publish Web project (includes references to Application)
RUN dotnet publish ./ArmA3Manager.Web/ArmA3Manager.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ------------------------------
# Stage 2: Runtime with SteamCMD
# ------------------------------
FROM ubuntu:24.04

# Install dependencies
RUN apt-get update && apt-get install -y \
    lib32gcc-s1 \
    lib32stdc++6 \
    wget \
    curl \
    unzip \
    ca-certificates \
    libicu74 \
    && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /app

# Copy published .NET app from build stage
COPY --from=build /app/publish .

# Install .NET runtime
RUN curl -L https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh &&  \
    chmod +x ./dotnet-install.sh && \
    ./dotnet-install.sh --version latest --runtime aspnetcore

ENV DOTNET_ROOT=/root/.dotnet
ENV PATH=$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH

# Install SteamCMD
RUN mkdir -p /steamcmd && \
    cd /steamcmd && \
    wget https://steamcdn-a.akamaihd.net/client/installer/steamcmd_linux.tar.gz && \
    tar -xvzf steamcmd_linux.tar.gz && \
    rm steamcmd_linux.tar.gz

ENV PATH="/steamcmd:${PATH}"
ENV Urls="http://+:5000"

# Create directories for Arma server and missions
RUN mkdir -p /arma3/server
RUN mkdir -p /arma3/MPMissions
RUN mkdir -p /arma3/workshop
RUN mkdir -p /arma3/config

ENV ServerDir=/arma3/server

# Set working directory to .NET app
WORKDIR /app

# Start the .NET Web API
ENTRYPOINT ["dotnet", "ArmA3Manager.Web.dll"]
