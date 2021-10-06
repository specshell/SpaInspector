FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY SpaInspector.csproj .
RUN dotnet restore "SpaInspector"
COPY . .
RUN dotnet build "SpaInspector.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpaInspector.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/SpaInspector/dist .
COPY nginx.conf /etc/nginx/nginx.conf