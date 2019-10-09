@ECHO OFF
SET base=%~dp0
echo Base directory: %base:~0,-1%
SET DRIVERSDIR=%base%
CD "%DRIVERSDIR%"

git clean -xdf
git reset --hard