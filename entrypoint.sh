#!/bin/sh
set -e

# Start the API in the background
ASPNETCORE_URLS=http://+:8080 dotnet /app/api/EExamSystem.Api.dll &
API_PID=$!

# Start the Client in the foreground
ASPNETCORE_URLS=http://+:8081 dotnet /app/client/EExamSystem.Client.dll &
CLIENT_PID=$!

# Exit if either process dies
wait -n $API_PID $CLIENT_PID
