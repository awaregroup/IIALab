FROM mcr.microsoft.com/windows/iotcore:1809

ARG EXE_DIR=release

WORKDIR /app

COPY $EXE_DIR/ ./

CMD [ "WindowsAiEdgeLabTabular.exe", "-mModel.onnx", "-cTest_data.csv", "-efv" ]