FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Gadget.Cli/Gadget.Cli.csproj", "Gadget.Cli/"]
RUN dotnet restore "Gadget.Cli/Gadget.Cli.csproj"
COPY . .
WORKDIR "/src/Gadget.Cli"
RUN dotnet build "Gadget.Cli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gadget.Cli.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gadget.Cli.dll"]
