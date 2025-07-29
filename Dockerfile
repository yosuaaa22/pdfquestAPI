# Menggunakan SDK .NET 8 untuk build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["pdfquestAPI.csproj", "."]
RUN dotnet restore "./pdfquestAPI.csproj"

COPY . .
WORKDIR "/src/."
RUN dotnet build "pdfquestAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "pdfquestAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Menggunakan runtime .NET 8 untuk produksi
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pdfquestAPI.dll"]