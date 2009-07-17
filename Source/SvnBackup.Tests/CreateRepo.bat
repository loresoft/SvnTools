@echo off

if '%1' == '' goto usage
if '%2' == '' goto usage

rd %1 /S /Q
rd working /S /Q

svnadmin create %1
dir Properties > Properties\path.txt
svn import Properties %2 --message "Import"
svn checkout %2 working

REM first change
dir Properties > working\path.txt
svn commit working --message "Commit 1"

REM second change
dir > working\path.txt
svn commit working --message "Commit 2"

goto done

:usage
echo.
echo Example: CreateRepo.bat "Repo" "file:///d:/repo"
echo.
:done