FROM mcr.microsoft.com/windows:1809

ARG EXE_DIR=.

WORKDIR /app

COPY $EXE_DIR/ ./

CMD [ "WindowsAiEdgeLabCV.exe", "-mCustomVision.onnx", "-dFront", "-fvwe" ]