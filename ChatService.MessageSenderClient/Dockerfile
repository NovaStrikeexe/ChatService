FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY ChatService.MessageSenderClient/ChatService.MessageSenderClient.csproj ./ChatService.MessageSenderClient/
WORKDIR /source/ChatService.MessageSenderClient
RUN dotnet restore

COPY ChatService.MessageSenderClient/. ./
RUN dotnet publish -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

RUN ls -al /usr/share/nginx/html

EXPOSE 5068

CMD ["nginx", "-g", "daemon off;"]
