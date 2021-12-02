## Here, we include the dotnet core SDK as the base to build our app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
## Setting the work directory for our app
WORKDIR /SpaInspectorReader
COPY /SpaInspectorReader/SpaInspectorReader.csproj .
RUN dotnet restore "SpaInspectorReader.csproj"

WORKDIR /SpaInspector
COPY /SpaInspector/SpaInspector.csproj .
RUN dotnet restore "SpaInspector.csproj"

COPY /SpaInspector .
COPY /SpaInspectorReader .
RUN dotnet build "SpaInspector.csproj" -c Release -o /build

## to the publish folder
FROM build-env AS publish
RUN dotnet publish "SpaInspector.csproj" -c Release -o /publish


FROM nginx:latest AS final

WORKDIR /usr/share/nginx/html
COPY --from=publish /publish/wwwroot /usr/local/webapp/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
