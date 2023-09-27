#!/bin/bash

# declare -A env_array
# env_array["DiscordToken"]=""
# env_array["OpenAiApiKey"]=""
# env_array["DiscordBotId"]=""
# serialized_array=$(declare -p env_array | sed 's/^declare -A env_array=//')
# export serialized_array

bash build_1_delete_docker_artifacts.sh

bash build_2_publish_docker.sh
