#### Overview

The SvnBackup command line tool is used to create backup copies of your [subversion](http://subversion.tigris.org/) repositories.  The source code is the life blood of your application.  Keeping the source repository backed up is major part in keeping your team going in case something goes wrong with your repository.

#### Features

*   Backup repository using hotcopy command
*   Backup folder management
*   Support repository parent directory
*   Keep x number of backups
*   Compress backups

#### Backup Process

SvnBackup follows the [recommend way of backing up](http://svnbook.red-bean.com/nightly/en/svn.reposadmin.maint.html#svn.reposadmin.maint.backup) your subversion repository.  While you can xcopy your repository, it may not always be the safest.  SvnBackup automates the process by using [svnadmin hotcopy](http://svnbook.red-bean.com/nightly/en/svn.ref.svnadmin.c.hotcopy.html) command.  The hotcopy command is the only safe way to make a backup copy of your repository.

SvnBackup also support starting from a parent folder that has all your repositories.  The tool will loop through all the repositories in that folder backing each up. The following folder layout contains imaginary repositories: `calculator`, `calendar`, and `spreadsheet`.


	repo/
	   calculator/
	   calendar/
	   spreadsheet/

The backups are stored in a root backup folder.  SvnBackup will create a subfolder for each repository.  Then it will create a folder for the current revision being backed up.  The hotcopy will be placed in the revision folder.  This allows you to keep multiple backup versions of your repository.  The following is an example of the backup folder structure created by SvnBackup.


	backup/
	   calculator/
	      v0000001/
	      v0000008/
	      v0000017/
	   calendar/
	      v0000001/
	      v0000014/
	      v0000127/
	   spreadsheet/
	      v0000001/
	      v0000023/
	      v0000047/

SvnBackup supports pruning your backups to only keep so many.  For example, you can keep the last 10 backups.

Another feature of SvnBackup is to compress the backup.  If you have a lot of repositories, zipping up the backup can save a lot of space.

#### Command Line Options


	SvnBackup.exe /r:<directory> /b:<directory> /c
	
	     - BACKUP OPTIONS -
	
	/history:<int>        Number of backups to keep. (/n)
	/compress             Compress backup folders. (/c)
	/repository:<string>  Repository root folder. (/r)
	/backup:<string>      Backup root folder. (/b)
	/svn:<string>         Path to subversion bin folder. (/s)

#### Project Page

*   Source Code - [http://code.google.com/p/dotsvntools/](http://code.google.com/p/dotsvntools/ "http://code.google.com/p/dotsvntools/")
*   Download - [http://code.google.com/p/dotsvntools/downloads/list](http://code.google.com/p/dotsvntools/downloads/list "http://code.google.com/p/dotsvntools/downloads/list")
