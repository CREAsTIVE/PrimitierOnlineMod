FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /App
COPY . ./
WORKDIR /App/DataTypes
RUN dotnet restore
WORKDIR /App/Server
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /App
COPY --from=build /App/Server/out .
ENTRYPOINT [ "dotnet", "POMServer.dll" ]