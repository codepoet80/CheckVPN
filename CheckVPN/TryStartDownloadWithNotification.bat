@echo off
CheckVPN
if %errorlevel% neq 0 (
	push-message "Not downloading since VPN was disconnected!"
)