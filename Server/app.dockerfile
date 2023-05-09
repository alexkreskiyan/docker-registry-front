FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine as builder
COPY . /src
RUN dotnet publish -c release -o /app /src

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
WORKDIR /app
COPY --from=builder /app /app
VOLUME [ "/app/config.json", "/app/key.pem" ]
CMD ["/app/Server"]