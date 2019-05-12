FROM mcr.microsoft.com/windows/iotcore:1809

ARG EXE_DIR=.

WORKDIR /app

COPY $EXE_DIR/ ./

CMD [ "WindowsAiEdgeLabTabular.exe", "-mModel.onnx", "-fInputs.csv", "-efv" ]