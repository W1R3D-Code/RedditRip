# README #

Prerequisites: .NET 4.5+ or Mono 4.05+ (Untested)

### What? ###

* .NET Command line tool for downloading Imgur pics from multiple Subreddits
* Supports various Imgur Album formats
* Uses RedditSharp (https://github.com/SirCmpwn/RedditSharp)

### How? ###

* Compile the code or download a [release](https://github.com/W1R3D-Code/RedditRip/releases) and unzip it
* Open a command prompt and change directory RedditRip.exe is in
* Run RedditRip.exe from the commandline with the correct arguments
  * e.g. **RedditRip.exe -s "wallpapers,MinimalWallpaper" -d C:\Reddit\img\**
* If you run RedditRip.exe with no arguments you will get a list of availible arguments
* SubName can be a single subreddit or multiple subreddits (comma separated list, no spaces)
* Output will be to outputPath\SubName\Username\Username_SubName_postid_#
  * i.e. RedditRip.exe -s "wallpapers,MinimalWallpaper" -d C:\Reddit\img\ would output to C:\Reddit\Img\wallpapers\zlakphoto\zlakphoto_wallpapers_2rr9ec_0001.jpg


### Contribution guidelines ###

* If you are interested in contributing please get in touch!

### Command line Arguments ###

|Argument|Required|Description
|:-------|:-------|:---------|
|**-s, --subreddits**		|**Required**	|**Subreddits to download** Imgur images from in a comma separated list|
|**-d, --destination**		|**Required**	|**Destination Directory** for downloads|
|-u, --username			|Optional	|Reddit username|
|-p, --password			|Optional	|Reddit password|
|-n, --nsfw				|(Default: True) |	Allow NSFW Posts|
|-x, --onlynsfw			|(Default: False)|	Only download NSFW Posts|
|-a, --allAuthorsPosts  |(Default: False)|	For every post, download all the imgur posts from that user too!|
|-v, --verbose  		|(Default: False)|	Print more details during execution.|
