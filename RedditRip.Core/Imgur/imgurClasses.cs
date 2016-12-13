using System.Collections.Generic;

namespace RedditRip.Core.Imgur
{
    public class Params
    {
        public string pgid { get; set; }
        public int unit { get; set; }
        public string jstag_url { get; set; }
    }

    public class Config2
    {
        public string bidder { get; set; }
        public Params @params { get; set; }
    }

    public class Openx
    {
        public bool prod { get; set; }
        public Config2 config { get; set; }
    }

    public class Params2
    {
        public string tagid { get; set; }
    }

    public class Config3
    {
        public string bidder { get; set; }
        public Params2 @params { get; set; }
    }

    public class Sovrn
    {
        public bool prod { get; set; }
        public Config3 config { get; set; }
    }

    public class Params3
    {
        public string placement { get; set; }
        public string network { get; set; }
        public string sizeId { get; set; }
    }

    public class Config4
    {
        public string bidder { get; set; }
        public Params3 @params { get; set; }
    }

    public class Aol
    {
        public bool prod { get; set; }
        public Config4 config { get; set; }
    }

    public class Params4
    {
        public string accountId { get; set; }
        public string siteId { get; set; }
        public string zoneId { get; set; }
        public List<int> sizes { get; set; }
    }

    public class Config5
    {
        public string bidder { get; set; }
        public Params4 @params { get; set; }
    }

    public class Rubicon
    {
        public bool prod { get; set; }
        public Config5 config { get; set; }
    }

    public class Params5
    {
        public string id { get; set; }
        public int siteID { get; set; }
    }

    public class Config6
    {
        public string bidder { get; set; }
        public Params5 @params { get; set; }
    }

    public class IndexExchange
    {
        public bool prod { get; set; }
        public Config6 config { get; set; }
    }

    public class Params6
    {
        public string inventoryCode { get; set; }
    }

    public class Config7
    {
        public string bidder { get; set; }
        public Params6 @params { get; set; }
    }

    public class Triplelift
    {
        public bool prod { get; set; }
        public Config7 config { get; set; }
    }

    public class Prebid
    {
        public Openx openx { get; set; }
        public Sovrn sovrn { get; set; }
        public Aol aol { get; set; }
        public Rubicon rubicon { get; set; }
        public IndexExchange indexExchange { get; set; }
        public Triplelift triplelift { get; set; }
    }

    public class Adblockplus
    {
        public string slot_id { get; set; }
    }

    public class Secure
    {
        public string slot_id { get; set; }
    }

    public class Moderated
    {
        public string slot_id { get; set; }
    }

    public class Flags
    {
        public Adblockplus adblockplus { get; set; }
        public bool gallery_nav { get; set; }
        public bool album { get; set; }
        public bool in_gallery { get; set; }
        public bool mature { get; set; }
        public bool sixth_unmod { get; set; }
        public bool sixth_mod_safe { get; set; }
        public bool sixth_mod_unsafe { get; set; }
        public bool onsfw_unmod { get; set; }
        public bool onsfw_mod_safe { get; set; }
        public bool onsfw_mod_unsafe { get; set; }
        public bool nsfw { get; set; }
        public bool promoted { get; set; }
        public bool referer { get; set; }
        public bool share { get; set; }
        public bool spam { get; set; }
        public bool under_10 { get; set; }
        public bool gallery { get; set; }
        public bool subreddit { get; set; }
        public bool subreddit_nsfw { get; set; }
        public bool logged_in { get; set; }
        public bool logged_out { get; set; }
        public bool page_load { get; set; }
        public bool pro { get; set; }
        public Secure secure { get; set; }
        public bool other { get; set; }
        public bool user_page { get; set; }
        public bool album_page { get; set; }
        public Moderated moderated { get; set; }
    }

    public class UnderSidebar
    {
        public string element { get; set; }
        public string insertSelector { get; set; }
        public string insertMethod { get; set; }
        public List<List<int>> sizes { get; set; }
        public string slot_id { get; set; }
        public Prebid prebid { get; set; }
        public Flags flags { get; set; }
    }

    public class Flags2
    {
        public bool adblockplus { get; set; }
        public bool gallery_nav { get; set; }
        public bool album { get; set; }
        public bool in_gallery { get; set; }
        public bool mature { get; set; }
        public bool sixth_unmod { get; set; }
        public bool sixth_mod_safe { get; set; }
        public bool sixth_mod_unsafe { get; set; }
        public bool onsfw_unmod { get; set; }
        public bool onsfw_mod_safe { get; set; }
        public bool onsfw_mod_unsafe { get; set; }
        public bool nsfw { get; set; }
        public bool promoted { get; set; }
        public bool referer { get; set; }
        public bool share { get; set; }
        public bool spam { get; set; }
        public bool under_10 { get; set; }
        public bool gallery { get; set; }
        public bool subreddit { get; set; }
        public bool subreddit_nsfw { get; set; }
        public bool logged_in { get; set; }
        public bool logged_out { get; set; }
        public bool page_load { get; set; }
        public bool pro { get; set; }
        public bool secure { get; set; }
        public bool other { get; set; }
        public bool user_page { get; set; }
        public bool album_page { get; set; }
    }

    public class Params7
    {
        public string pgid { get; set; }
        public int unit { get; set; }
        public string jstag_url { get; set; }
    }

    public class Config8
    {
        public string bidder { get; set; }
        public Params7 @params { get; set; }
    }

    public class Openx2
    {
        public bool prod { get; set; }
        public Config8 config { get; set; }
    }

    public class Params8
    {
        public string tagid { get; set; }
    }

    public class Config9
    {
        public string bidder { get; set; }
        public Params8 @params { get; set; }
    }

    public class Sovrn2
    {
        public bool prod { get; set; }
        public Config9 config { get; set; }
    }

    public class Params9
    {
        public string placement { get; set; }
        public string network { get; set; }
        public string sizeId { get; set; }
    }

    public class Config10
    {
        public string bidder { get; set; }
        public Params9 @params { get; set; }
    }

    public class Aol2
    {
        public bool prod { get; set; }
        public Config10 config { get; set; }
    }

    public class Params10
    {
        public string accountId { get; set; }
        public string siteId { get; set; }
        public string zoneId { get; set; }
        public List<int> sizes { get; set; }
    }

    public class Config11
    {
        public string bidder { get; set; }
        public Params10 @params { get; set; }
    }

    public class Rubicon2
    {
        public bool prod { get; set; }
        public Config11 config { get; set; }
    }

    public class Params11
    {
        public string id { get; set; }
        public int siteID { get; set; }
    }

    public class Config12
    {
        public string bidder { get; set; }
        public Params11 @params { get; set; }
    }

    public class IndexExchange2
    {
        public bool prod { get; set; }
        public Config12 config { get; set; }
    }

    public class Params12
    {
        public string inventoryCode { get; set; }
    }

    public class Config13
    {
        public string bidder { get; set; }
        public Params12 @params { get; set; }
    }

    public class Triplelift2
    {
        public bool prod { get; set; }
        public Config13 config { get; set; }
    }

    public class Prebid2
    {
        public Openx2 openx { get; set; }
        public Sovrn2 sovrn { get; set; }
        public Aol2 aol { get; set; }
        public Rubicon2 rubicon { get; set; }
        public IndexExchange2 indexExchange { get; set; }
        public Triplelift2 triplelift { get; set; }
    }

    public class TopBanner
    {
        public string element { get; set; }
        public Flags2 flags { get; set; }
        public string insertSelector { get; set; }
        public string insertMethod { get; set; }
        public List<List<int>> sizes { get; set; }
        public string slot_id { get; set; }
        public Prebid2 prebid { get; set; }
    }

    public class Flags3
    {
        public bool adblockplus { get; set; }
        public bool gallery_nav { get; set; }
        public bool album { get; set; }
        public bool in_gallery { get; set; }
        public bool mature { get; set; }
        public bool sixth_unmod { get; set; }
        public bool sixth_mod_safe { get; set; }
        public bool sixth_mod_unsafe { get; set; }
        public bool onsfw_unmod { get; set; }
        public bool onsfw_mod_safe { get; set; }
        public bool onsfw_mod_unsafe { get; set; }
        public bool nsfw { get; set; }
        public bool promoted { get; set; }
        public bool referer { get; set; }
        public bool share { get; set; }
        public bool spam { get; set; }
        public bool under_10 { get; set; }
        public bool gallery { get; set; }
        public bool subreddit { get; set; }
        public bool subreddit_nsfw { get; set; }
        public bool logged_in { get; set; }
        public bool logged_out { get; set; }
        public bool page_load { get; set; }
        public bool pro { get; set; }
        public bool secure { get; set; }
        public bool other { get; set; }
        public bool user_page { get; set; }
        public bool album_page { get; set; }
    }

    public class Spotlight
    {
        public string element { get; set; }
        public string domElement { get; set; }
        public Flags3 flags { get; set; }
        public List<List<int>> sizes { get; set; }
        public string slot_id { get; set; }
        public object prebid { get; set; }
    }

    public class Params13
    {
        public object pgid { get; set; }
        public int unit { get; set; }
        public string jstag_url { get; set; }
    }

    public class Config14
    {
        public string bidder { get; set; }
        public Params13 @params { get; set; }
    }

    public class Openx3
    {
        public bool prod { get; set; }
        public Config14 config { get; set; }
    }

    public class Params14
    {
        public string tagid { get; set; }
    }

    public class Config15
    {
        public string bidder { get; set; }
        public Params14 @params { get; set; }
    }

    public class Sovrn3
    {
        public bool prod { get; set; }
        public Config15 config { get; set; }
    }

    public class Params15
    {
        public string placement { get; set; }
        public string network { get; set; }
        public string sizeId { get; set; }
    }

    public class Config16
    {
        public string bidder { get; set; }
        public Params15 @params { get; set; }
    }

    public class Aol3
    {
        public bool prod { get; set; }
        public Config16 config { get; set; }
    }

    public class Params16
    {
        public string accountId { get; set; }
        public string siteId { get; set; }
        public string zoneId { get; set; }
        public List<int> sizes { get; set; }
    }

    public class Config17
    {
        public string bidder { get; set; }
        public Params16 @params { get; set; }
    }

    public class Rubicon3
    {
        public bool prod { get; set; }
        public Config17 config { get; set; }
    }

    public class Params17
    {
        public string id { get; set; }
        public int siteID { get; set; }
    }

    public class Config18
    {
        public string bidder { get; set; }
        public Params17 @params { get; set; }
    }

    public class IndexExchange3
    {
        public bool prod { get; set; }
        public Config18 config { get; set; }
    }

    public class Params18
    {
        public string inventoryCode { get; set; }
    }

    public class Config19
    {
        public string bidder { get; set; }
        public Params18 @params { get; set; }
    }

    public class Triplelift3
    {
        public bool prod { get; set; }
        public Config19 config { get; set; }
    }

    public class Prebid3
    {
        public Openx3 openx { get; set; }
        public Sovrn3 sovrn { get; set; }
        public Aol3 aol { get; set; }
        public Rubicon3 rubicon { get; set; }
        public IndexExchange3 indexExchange { get; set; }
        public Triplelift3 triplelift { get; set; }
    }

    public class Flags4
    {
        public bool adblockplus { get; set; }
        public bool gallery_nav { get; set; }
        public bool album { get; set; }
        public bool in_gallery { get; set; }
        public bool mature { get; set; }
        public bool sixth_unmod { get; set; }
        public bool sixth_mod_safe { get; set; }
        public bool sixth_mod_unsafe { get; set; }
        public bool onsfw_unmod { get; set; }
        public bool onsfw_mod_safe { get; set; }
        public bool onsfw_mod_unsafe { get; set; }
        public bool nsfw { get; set; }
        public bool promoted { get; set; }
        public bool referer { get; set; }
        public bool share { get; set; }
        public bool spam { get; set; }
        public bool under_10 { get; set; }
        public bool gallery { get; set; }
        public bool subreddit { get; set; }
        public bool subreddit_nsfw { get; set; }
        public bool logged_in { get; set; }
        public bool logged_out { get; set; }
        public bool page_load { get; set; }
        public bool pro { get; set; }
        public bool secure { get; set; }
        public bool other { get; set; }
        public bool user_page { get; set; }
        public bool album_page { get; set; }
    }

    public class PopSky
    {
        public string element { get; set; }
        public string insertSelector { get; set; }
        public string insertMethod { get; set; }
        public List<List<int>> sizes { get; set; }
        public string slot_id { get; set; }
        public Prebid3 prebid { get; set; }
        public Flags4 flags { get; set; }
    }

    public class ActiveSlots
    {
        public UnderSidebar under_sidebar { get; set; }
        public TopBanner top_banner { get; set; }
        public Spotlight spotlight { get; set; }
        public PopSky pop_sky { get; set; }
    }

    public class BidderAdjustments
    {
        public double aol { get; set; }
        public double rubicon { get; set; }
        public int triplelift { get; set; }
        public int indexExchange { get; set; }
        public int sovrn { get; set; }
        public int openx { get; set; }
    }

    public class Gpt
    {
        public string src { get; set; }
    }

    public class Openx4
    {
        public string src { get; set; }
    }

    public class Partners
    {
        public Gpt gpt { get; set; }
        public Openx4 openx { get; set; }
    }

    public class ClientFlags
    {
        public bool adblockplus { get; set; }
        public bool gallery_nav { get; set; }
    }

    public class GlobalFlags
    {
        public bool logged_in { get; set; }
        public bool logged_out { get; set; }
        public bool page_load { get; set; }
        public bool pro { get; set; }
        public bool secure { get; set; }
        public bool other { get; set; }
        public bool user_page { get; set; }
        public bool album_page { get; set; }
    }

    public class GalleryFlags
    {
        public bool gallery { get; set; }
        public bool subreddit { get; set; }
        public bool subreddit_nsfw { get; set; }
    }

    public class ItemFlags
    {
        public bool album { get; set; }
        public bool in_gallery { get; set; }
        public bool mature { get; set; }
        public bool sixth_unmod { get; set; }
        public bool sixth_mod_safe { get; set; }
        public bool sixth_mod_unsafe { get; set; }
        public bool onsfw_unmod { get; set; }
        public bool onsfw_mod_safe { get; set; }
        public bool onsfw_mod_unsafe { get; set; }
        public bool nsfw { get; set; }
        public bool promoted { get; set; }
        public bool referer { get; set; }
        public bool share { get; set; }
        public bool spam { get; set; }
        public bool under_10 { get; set; }
    }

    public class Config
    {
        public List<string> whitelisted_promoted_posts { get; set; }
        public string ads_website { get; set; }
        public bool allow_nsfw { get; set; }
        public bool allow_unmoderated { get; set; }
        public bool allow_subreddits_nsfw { get; set; }
        public bool _enabled_ { get; set; }
        public ActiveSlots active_slots { get; set; }
        public string item_key { get; set; }
        public BidderAdjustments bidder_adjustments { get; set; }
        public Partners partners { get; set; }
        public ClientFlags client_flags { get; set; }
        public GlobalFlags global_flags { get; set; }
        public GalleryFlags gallery_flags { get; set; }
        public ItemFlags item_flags { get; set; }
    }

    public class Place
    {
        public object under_sidebar { get; set; }
        public object top_banner { get; set; }
        public object spotlight { get; set; }
        public object pop_sky { get; set; }
        public List<string> keywords { get; set; }
    }

    public class Image
    {
        public string hash { get; set; }
        public string title { get; set; }
        public object description { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int size { get; set; }
        public string ext { get; set; }
        public bool animated { get; set; }
        public bool prefer_video { get; set; }
        public bool looping { get; set; }
        public string datetime { get; set; }
    }

    public class AlbumImages
    {
        public int count { get; set; }
        public List<Image> images { get; set; }
    }

    public class Place2
    {
        public object under_sidebar { get; set; }
        public object top_banner { get; set; }
        public object spotlight { get; set; }
        public object pop_sky { get; set; }
        public List<string> keywords { get; set; }
    }

    public class Item
    {
        public string id { get; set; }
        public string title { get; set; }
        public string title_clean { get; set; }
        public object description { get; set; }
        public string privacy { get; set; }
        public string cover { get; set; }
        public string order { get; set; }
        public string layout { get; set; }
        public string num_images { get; set; }
        public string datetime { get; set; }
        public string views { get; set; }
        public bool isAd { get; set; }
        public string hash { get; set; }
        public string album_cover { get; set; }
        public string album_privacy { get; set; }
        public object album_description { get; set; }
        public string album_layout { get; set; }
        public int starting_score { get; set; }
        public bool is_album { get; set; }
        public string operation { get; set; }
        public bool favorited { get; set; }
        public bool force_grid { get; set; }
        public bool is_viral { get; set; }
        public string cover_deletehash { get; set; }
        public AlbumImages album_images { get; set; }
        public Place2 place { get; set; }
    }

    public class WindowRunSlots
    {
        public Config _config { get; set; }
        public Place _place { get; set; }
        public Item _item { get; set; }
    }
}
