#!/bin/bash

GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m'

echo -e "${BLUE}=== Tailwind CSS Setup & Runner ===${NC}"

if [ ! -d "wwwroot" ]; then
    echo -e "${RED}Error: 'wwwroot' directory not found.${NC}"
    echo "Please run this script from the root of your Blazor project (where the .csproj is located)."
    exit 1
fi

if [ ! -d "node_modules/@tailwindcss/cli" ]; then
    echo -e "${BLUE}Tailwind v4 not found. Starting installation...${NC}"
    
    if [ ! -f "package.json" ]; then
        echo "Creating package.json..."
        npm init -y > /dev/null
    fi

    echo "Installing tailwindcss and @tailwindcss/cli..."
    npm install -D tailwindcss @tailwindcss/cli
    
    echo -e "${GREEN}Installation complete!${NC}"
else
    echo -e "${GREEN}Tailwind is already installed.${NC}"
fi

echo -e "${BLUE}Starting Tailwind in watch mode...${NC}"
npx @tailwindcss/cli -i ./wwwroot/app.css -o ./wwwroot/app.min.css --watch