ARG PROJECT_NAME=Api
ARG FRONTEND_PROJECT_NAME=Frontend

FROM mcr.microsoft.com/dotnet/aspnet:10.0.0-preview.6-alpine3.22 AS base
WORKDIR /app
EXPOSE 8080

# Frontend build stage
FROM node:22-alpine AS frontend
ARG FRONTEND_PROJECT_NAME
WORKDIR /frontend
COPY ./${FRONTEND_PROJECT_NAME}/package*.json .
RUN npm ci
COPY ./${FRONTEND_PROJECT_NAME} .
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview-alpine AS build
ARG PROJECT_NAME
ARG FRONTEND_PROJECT_NAME
WORKDIR /src
COPY . .
RUN dotnet restore "${PROJECT_NAME}/${PROJECT_NAME}.csproj"
WORKDIR "/src/${PROJECT_NAME}"
# Copy frontend dist to wwwroot
COPY --from=frontend /frontend/dist ./wwwroot
RUN dotnet build "${PROJECT_NAME}.csproj" -c Release -o /app/build

FROM build AS publish
ARG PROJECT_NAME
RUN dotnet publish "${PROJECT_NAME}.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
ARG PROJECT_NAME
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV PROJECT_NAME=${PROJECT_NAME}
ENTRYPOINT dotnet ${PROJECT_NAME}.dll