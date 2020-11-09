@echo off
REM Send a message about VPN state via Pushover
REM		Supply API credentials in variables below
set token=applicationtokenvalue
set user=usertokenvalue
curl -s -F "token=%token%" -F "user=%user%" -F "title=VPN Notice" -F "message=%~1" https://api.pushover.net/1/messages.json