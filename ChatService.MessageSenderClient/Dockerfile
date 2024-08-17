FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ChatService.MessageSenderClient/ChatService.MessageSenderClient.csproj", "ChatService.MessageSenderClient/"]
RUN dotnet restore "ChatService.MessageSenderClient/ChatService.MessageSenderClient.csproj"
COPY . .
WORKDIR "/src/ChatService.MessageSenderClient"
RUN dotnet build "ChatService.MessageSenderClient.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ChatService.MessageSenderClient.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatService.MessageSenderClient.dll"]
