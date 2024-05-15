#!/bin/bash

# Delete docker container FORCED if it exists
# Your image name
IMAGE_NAME="oahouseai"

# Find the container ID by filtering containers based on the image name
CONTAINER_ID=$(docker ps -q --filter "ancestor=$IMAGE_NAME:latest")

dotnet publish --os linux --arch x64 /t:PublishContainer -c Release
docker run -d -p 10080:10080 --restart=always --name $IMAGE_NAME -e AppConfigConnectionString="$1" $IMAGE_NAME:latest
rm -rf /tmp/Containers



# Check if there are any dangling images
if [ -n "$(docker images -f dangling=true -q)" ]; then
    # Remove dangling images
    docker rmi --force $(docker images -f "dangling=true" -q)
else
    echo "No dangling images found."
fi
