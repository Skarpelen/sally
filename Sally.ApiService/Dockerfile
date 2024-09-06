# Dockerfile для Sally.ApiService
# Stage 1: Build and generate certificate
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy and restore project files
COPY ["Sally.ApiService/Sally.ApiService.csproj", "Sally.ApiService/"]
RUN dotnet restore "Sally.ApiService/Sally.ApiService.csproj"

# Copy all files and build the project
COPY . .
WORKDIR "/src/Sally.ApiService"
RUN dotnet build "Sally.ApiService.csproj" -c Release -o /app/build
RUN dotnet publish "Sally.ApiService.csproj" -c Release -o /app/publish

# Generate and export developer certificate
RUN dotnet dev-certs https --clean
RUN dotnet dev-certs https -ep /https/aspnetapp.pfx -p password
RUN dotnet dev-certs https --trust

# Stage 2: Setup runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy the generated certificate
COPY --from=build /https /https

# Copy the built and published application
COPY --from=build /app/publish .

# Set the environment variables for Kestrel
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=password

# Entry point to run the application
ENTRYPOINT ["dotnet", "Sally.ApiService.dll"]
