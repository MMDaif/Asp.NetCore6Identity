#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["testidentity2/testidentity2.csproj", "testidentity2/"]
RUN dotnet restore "testidentity2/testidentity2.csproj"
COPY . .
WORKDIR "/src/testidentity2"
RUN dotnet build "testidentity2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "testidentity2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "testidentity2.dll"]