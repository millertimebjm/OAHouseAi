#!/bin/bash

IMAGE_NAME="oahousechatgpt"

eval "declare -A env_array=${serialized_array}"
args=""
for key in "${!env_array[@]}"; do
  args+="-e ${key}=${env_array[$key]} "
done

for key in "${!env_array[@]}"; do
  echo "Key: $key, Value: ${env_array[$key]}"
done

echo "args: ${args[@]}"

dotnet build


dotnet publish --os linux --arch x64 /t:PublishContainer -c Release
docker run -d --restart=always --name $IMAGE_NAME $IMAGE_NAME:1.0.0 ${args[@]}
rm -rf /tmp/Containers



# Check if there are any dangling images
if [ -n "$(docker images -f dangling=true -q)" ]; then
    # Remove dangling images
    docker rmi --force $(docker images -f "dangling=true" -q)
else
    echo "No dangling images found."
fi

