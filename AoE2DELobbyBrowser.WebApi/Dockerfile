# Final stage: run the application using the ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY publish/ ./
# Expose port 80 for the application
# EXPOSE 5100
# EXPOSE 5101
# Set the application to listen on IPv4 only
# ENV ASPNETCORE_URLS="https://*:443;http://*:8080"
# ENV ASPNETCORE_HTTPS_PORTS=5101
# Run the application
ENTRYPOINT ["dotnet", "AoE2DELobbyBrowser.WebApi.dll"]