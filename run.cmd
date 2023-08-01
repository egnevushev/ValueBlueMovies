@echo off

docker build -t movies . && docker run -p 5050:80 --name movies_api movies

pause
