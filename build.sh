#!/bin/bash

#export keys=("DiscordToken" "OpenAiApiKey" "DiscordBotId")
#export values=("" "" "")

bash build_1_delete_docker_artifacts.sh

bash build_2_publish_docker.sh
