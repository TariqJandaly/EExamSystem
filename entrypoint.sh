#!/bin/sh
ASPNETCORE_URLS=http://+:8080 dotnet /app/api/EExamSystem.Api.dll &

export ASPNETCORE_URLS=http://+:8081
exec dotnet /app/client/EExamSystem.Client.dll
