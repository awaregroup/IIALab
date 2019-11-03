@ECHO OFF
SET base=%~dp0
echo Base directory: %base:~0,-1%
SET CURDIR=%base%
CD /D "%CURDIR%"

git clean -xdf
git reset --hard
git pull
