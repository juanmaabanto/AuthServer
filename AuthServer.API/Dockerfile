FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
ARG source
WORKDIR /app
EXPOSE 443
COPY ${source:-bin/Debug/netcoreapp3.1/publish} .
ENTRYPOINT ["dotnet", "Expertec.Sigeco.AuthServer.API.dll"]