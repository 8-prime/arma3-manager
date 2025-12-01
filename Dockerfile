# =====================================================================
# Stage 1: Build React (pnpm)
# =====================================================================

FROM node:24 AS frontend-build

WORKDIR /app


# PNPM
ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"
RUN corepack enable

# Copy pnpm files for caching
COPY frontend/pnpm-lock.yaml ./
COPY frontend/package.json ./

# Now copy the full project
COPY frontend/ ./

# Rebuild the node_modules symlinks to .pnpm store
RUN pnpm install --frozen-lockfile

# Build the frontend
RUN pnpm build
# → Output expected: /app/dist

# =====================================================================
# Stage 2: Publish ASP.NET Core (with React dist)
# =====================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS dotnet-publish

WORKDIR /src

COPY src/ArmA3Manager.Web/*.csproj ./ArmA3Manager.Web/
COPY src/ArmA3Manager.Application/*.csproj ./ArmA3Manager.Application/

# Restore dependencies
RUN dotnet restore ./ArmA3Manager.Web/ArmA3Manager.Web.csproj

# Copy backend source
COPY src/ ./

# Copy built frontend into Web project's wwwroot
COPY --from=frontend-build /app/dist ./ArmA3Manager.Web/wwwroot/

# Publish .NET app
RUN dotnet publish ./ArmA3Manager.Web/ArmA3Manager.Web.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ------------------------------
# Stage 3: Runtime with SteamCMD
# ------------------------------
FROM ubuntu:24.04

# Install dependencies
RUN apt-get update && apt-get install -y \
    ca-certificates \
    curl \
    lib32gcc-s1 \
    lib32stdc++6 \
    libicu74 \
    unzip \
    wget \
    && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /app

# Copy published .NET app from build stage
COPY --from=dotnet-publish /app/publish .

# Install .NET runtime
ADD https://dot.net/v1/dotnet-install.sh dotnet-install.sh
RUN chmod +x ./dotnet-install.sh && \
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
ENV Urls="http://+:80"

# Create directories for Arma server and missions
RUN mkdir -p /arma3/server && \
    mkdir -p /arma3/MPMissions && \
    mkdir -p /arma3/workshop && \
    mkdir -p /arma3/config

ENV ServerDir=/arma3/server
ENV ConfigDir=/arma3/config

# Set working directory to .NET app
WORKDIR /app

# Start the .NET Web API
ENTRYPOINT ["dotnet", "ArmA3Manager.Web.dll"]
