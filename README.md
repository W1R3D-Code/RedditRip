# README #

Prerequisites: .NET 4.5+ or (Untested) Mono 4.05+

### What? ###

* Command line tool for downloading Imgur pics from multiple Subreddits
* Uses RedditSharp (https://github.com/SirCmpwn/RedditSharp)

### How? ###

* Compile the code
* Run RedditRip.exe from the commandline with the correct arguments
* e.g. RedditRip.exe -s "wallpapers,MinimalWallpaper" -d D:\Reddit\img\
* No Help text currently, will add if anyone else ever starts using this!
* SubName can be a single subreddit or multiple (comma separated list, no spaces)
* Output will be to outputPath\SubName\Username\Username_SubName_postid_#
* i.e. D:\Reddit\Img\wallpapers\zlakphoto\zlakphoto_wallpapers_2rr9ec_0001.jpg


### Contribution guidelines ###

* Welcome to refactoring (it needs it atm!!) and updates
* When I figure out how to facilitate that I will update this section


### Commandline Arguments ###

s, --subreddits         Required. CSV list of Subreddits to download Imgur
                        images from

-d, --destination        Required. Destination Directory for downloads

-u, --username           Optional. Reddit username

-p, --password           Optional. Reddit password

-n, --nsfw               (Default: True) Allow NSFW Posts

-x, --onlynsfw           (Default: False) Only download NSFW Posts

-a, --allAuthorsPosts    (Default: False) For every post, download all the
                         imgur posts from that user too!

-v, --verbose            (Default: False) Print more details during
                         execution.
