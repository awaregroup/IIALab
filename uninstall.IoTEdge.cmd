powershell -command ". {Invoke-WebRequest -useb aka.ms/iotedge-win} | Invoke-Expression; Uninstall-IoTEdge -Force"
pause