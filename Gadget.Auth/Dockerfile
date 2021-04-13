FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Gadget.Auth/Gadget.Auth.csproj", "Gadget.Auth/"]
RUN dotnet restore "Gadget.Auth/Gadget.Auth.csproj"
COPY . .
WORKDIR "/src/Gadget.Auth"
RUN dotnet build "Gadget.Auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gadget.Auth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gadget.Auth.dll"]
