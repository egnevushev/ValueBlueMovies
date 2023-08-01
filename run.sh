#!/usr/bin/env bash

docker build -t movies . && docker run -it --rm -p 5050:80 --name movies_api movies