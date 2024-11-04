
@echo off

REM URL of the Swagger JSON endpoint
set SWAGGER_URL=https://localhost:7100/swagger/v1/swagger.json
set OUTPUT_FILE=swagger-docs.json

REM Download the Swagger JSON documentation
curl -o %OUTPUT_FILE% %SWAGGER_URL%

echo API documentation generated at %OUTPUT_FILE%