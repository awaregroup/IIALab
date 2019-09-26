# escape=`
# Based on https://github.com/dotnet/dotnet-docker/blob/master/2.2/runtime/nanoserver-1809/amd64/Dockerfile

# Installer image
FROM mcr.microsoft.com/windows/iotcore:1809

ARG EXE_DIR=release

WORKDIR /app

COPY $EXE_DIR/ ./

CMD [ "WindowsAiEdgeLabCV.exe", "-mCustomVision.onnx", "-dLifeCam", "-efv" ]