#!/bin/bash

IMAGE_NAME="oahousechatgpt"

args=""
for ((i=0; i<${#keys[@]}; i++)); do
  args+=" -e ${keys[$i]}=${values[$i]}"
done

dotnet build


dotnet publish --os linux --arch x64 /t:PublishContainer -c Release
docker run -d -p 7080:7080 --restart=always --name $IMAGE_NAME $IMAGE_NAME:1.0.0 $args
rm -rf /tmp/Containers



# Check if there are any dangling images
if [ -n "$(docker images -f dangling=true -q)" ]; then
    # Remove dangling images
    docker rmi --force $(docker images -f "dangling=true" -q)
else
    echo "No dangling images found."
fi

