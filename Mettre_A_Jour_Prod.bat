@echo off
setlocal EnableDelayedExpansion
color 0A
echo ===================================================
echo   Mise a jour intelligente du serveur OpenDAoC
echo ===================================================
echo.

cd /d "%~dp0OpenDAoC-Core-master"

echo [1/4] Validation des modifications courantes...
:: On verifie s'il y a des fichiers modifies pour les sauvegarder dans Git
git diff-index --quiet HEAD --
if errorlevel 1 (
    echo - Modifications detectees. Commit automatique de vos scripts...
    git add .
    git commit -m "Auto-sauvegarde locale de la Prod avant mise a jour officielle"
) else (
    echo - Aucune modification detectee depuis la derniere fois.
)
echo.

echo [2/4] Telechargement de la mise a jour officielle (Upstream)...
git pull upstream master
if errorlevel 1 (
    color 0C
    echo.
    echo /!\ ERREUR ou CONFLIT DE FUSION RECONTRÉ.
    echo La mise a jour officielle a modifie les memes lignes que vos propres scripts.
    echo 1. Ouvrez VSCode pour corriger les lignes en rouge.
    echo 2. Tapez une fois fini 'git commit -a'.
    echo 3. Relancez ce script !
    echo.
    pause
    exit /b 1
)
echo.

echo [3/4] Sauvegarde de la Prod sur votre propre GitHub (Fork)...
:: On envoie vers https://github.com/guill4ume/amtenael (origin)
git push origin master
echo.

echo [4/4] Recompilation et redemarrage du serveur Docker...
docker compose up -d --build
if errorlevel 1 (
    color 0C
    echo.
    echo /!\ ERREUR Docker: La compilation (build) a echoue.
    echo Verifiez que vous n'avez pas de faute de frappe dans vos scripts C#!
    echo.
    pause
    exit /b 1
)

echo.
color 0A
echo ===================================================
echo Succes : Le serveur est a jour, sauvegarde sur GitHub, compile, et tourne !
echo ===================================================
echo.
pause
