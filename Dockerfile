FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "SpaInspector/SpaInspector.csproj"
WORKDIR "/src/SpaInspector"



WORKDIR "/src/SpaInspector"
RUN dotnet publish "SpaInspector.csproj" -c Release -o /app

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app .
COPY nginx.conf /etc/nginx/nginx.conf
