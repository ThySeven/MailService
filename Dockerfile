FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine as build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/published-app --runtime linux-musl-x64 --self-contained false
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine as runtime
WORKDIR /app
COPY --from=build /app/published-app /app
COPY /MailService/Models/Compliments.html /app/Models/Compliments.html
ENTRYPOINT ["dotnet", "/app/MailService.dll"]
