# Base image for the final container
FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine AS base
WORKDIR /app
RUN apk update && apk upgrade
RUN apk add --no-cache curl libstdc++ lua5.4 lua5.4-dev
# Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
RUN apk update && apk upgrade
RUN apk add --no-cache curl git clang build-base zlib-dev clang binutils musl-dev zlib-static cmake icu-static icu-dev
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
WORKDIR /src
COPY ["./", "./"]
COPY . .
WORKDIR "/src/src/Moongate.Server"
RUN dotnet restore "Moongate.Server.csproj" -a $TARGETARCH
RUN dotnet build "Moongate.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build -a $TARGETARCH
# Publish image with single file
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
RUN dotnet publish "Moongate.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish \
    -a $TARGETARCH \
    -p:PublishAot=true


RUN rm .git/ -Rf
RUN rm .github/ -Rf
RUN rm .gitignore -Rf
RUN rm .dockerignore -Rf
RUN rm ./assets -Rf
RUN rm src -Rf
# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV MOONGATE_ROOT=/app

# Set non-root user for better security
# Creating user inside container rather than using $APP_UID since Alpine uses different user management
RUN adduser -D -h /app moongate && \
    chown -R moongate:moongate /app

# Create directories for data persistence
RUN mkdir -p /app/data /app/logs /app/scripts && \
    chown -R moongate:moongate /app/data /app/logs /app/scripts


USER moongate

ENTRYPOINT ["./Moongate.Server"]
