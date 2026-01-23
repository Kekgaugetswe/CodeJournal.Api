# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj first for caching
COPY ["CodeJournal.Api/CodeJournal.Api.csproj", "CodeJournal.Api/"]
RUN dotnet restore "CodeJournal.Api/CodeJournal.Api.csproj"

# Copy everything else
COPY . .

# Publish
WORKDIR /src/CodeJournal.Api
RUN dotnet publish "CodeJournal.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false


# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Kestrel will listen on 8080 in container
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Copy published output
COPY --from=build /app/publish .

# ✅ Create the folder your app expects: /app/images
RUN mkdir -p /app/images

# (Optional) If you want images to persist outside the container, you can mount a volume to /app/images
VOLUME ["/app/images"]

ENTRYPOINT ["dotnet", "CodeJournal.Api.dll"]