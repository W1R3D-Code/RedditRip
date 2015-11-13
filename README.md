# README #

Prerequisites: .NET 4.5+ or (Untested) Mono 4.05+

### What? ###

* Command line tool for downloading Imgur pics from multiple Subreddits
* Uses RedditSharp (https://github.com/SirCmpwn/RedditSharp)

### How? ###

* Compile the code
* Run RedditRip.exe from the commandline with the correct arguments
* e.g. RedditRip.exe username password "wallpapers,MinimalWallpaper" D:\Reddit\img\
* No Help text currently, will add if anyone else ever starts using this!
* Args are username, password, SubName, outputPath
* SubName can be a single subreddit or multiple (comma separated list, no spaces)
* Output will be to outputPath\SubName\Username\postid_#
* i.e. D:\Reddit\Img\wallpapers\zlakphoto\2rr9ec_0001.jpg


### Contribution guidelines ###

* Welcome to refactoring (it needs it atm!!) and updates
* When I figure out how to facilitate that I will update this section