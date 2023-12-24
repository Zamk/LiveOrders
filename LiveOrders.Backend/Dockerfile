FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LiveOrders.Backend/LiveOrders.Backend.csproj", "LiveOrders.Backend/"]
RUN dotnet restore "LiveOrders.Backend/LiveOrders.Backend.csproj"
COPY . .
WORKDIR "/src/LiveOrders.Backend"
RUN dotnet build "LiveOrders.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LiveOrders.Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LiveOrders.Backend.dll"]
