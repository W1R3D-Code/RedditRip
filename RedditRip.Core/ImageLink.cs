using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditRip.Core
{
    public class ImageLink
    {
        public ImageLink(Post post, string url, string filename)
        {
            Post = post;
            Url = url;
            Filename = filename;
        }

        public Post Post { get; set; }

        public string Url { get; set; }

        public string Filename { get; set; }

        public string GetPostDetails(KeyValuePair<string, List<ImageLink>> post)
        {
            return GetPostDetails(this, post);
        }

        public static string GetPostDetails(ImageLink imageLink, KeyValuePair<string, List<ImageLink>> post)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Subreddit:\t{imageLink?.Post.SubredditName}");

            sb.Append($"User:\t\t{imageLink?.Post.AuthorName}");
            sb.Append(string.IsNullOrWhiteSpace(imageLink?.Post.AuthorFlairText)
                ? string.Empty
                : imageLink?.Post.AuthorFlairText);
            sb.AppendLine();

            if (imageLink?.Post?.Title != null)
            {
                sb.AppendLine(imageLink.Post.NSFW ? " - NSFW" : "");
                sb.AppendLine($"Post:\t\t{imageLink?.Post.Title}");
                sb.AppendLine($"Score:\t\t{imageLink?.Post.Score}");
                sb.AppendLine($"Link:\t\t{imageLink?.Post.Url}");

                sb.AppendLine();

                if (!string.IsNullOrWhiteSpace(imageLink?.Post.SelfText))
                {
                    sb.AppendLine();
                    sb.AppendLine($"Message:\t\t{imageLink?.Post.SelfText}");
                    sb.AppendLine();
                }

                sb.AppendLine($"Images:\t\t{post.Value.Count}");
                sb.AppendLine();
            }
            
            foreach (var link in post.Value)
            {
                sb.AppendLine(link.Url);
            }

            var details = sb.ToString();
            return details;
        }
    }
}