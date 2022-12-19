using Markdig;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Dpz.Client.Library;

public static class AppTools
{
    /// <summary>
    /// Markdown转为Html
    /// </summary>
    /// <param name="markdown"></param>
    /// <param name="disableHtml">是否禁用html（默认禁用）</param>
    /// <returns></returns>
    public static string MarkdownToHtml(this string markdown, bool disableHtml = true)
    {
        var pipelineBuild = new MarkdownPipelineBuilder()
            .UseAutoLinks()
            .UsePipeTables()
            .UseTaskLists()
            .UseEmphasisExtras()
            .UseAutoIdentifiers();

        if (disableHtml)
        {
            pipelineBuild.DisableHtml();
        }

        var pipeline = pipelineBuild.Build();

        var document = Markdown.Parse(markdown, pipeline);
        foreach (var link in document.Descendants<LinkInline>())
        {
            link.GetAttributes().AddPropertyIfNotExist("target", "_blank");
        }

        foreach (var link in document.Descendants<AutolinkInline>())
        {
            link.GetAttributes().AddPropertyIfNotExist("target", "_blank");
        }

        return document.ToHtml(pipeline);
    }
    
    public static string TimeAgo(this DateTime time)
    {
        var ts = new TimeSpan(DateTime.UtcNow.Ticks - time.ToUniversalTime().Ticks);
        var delta = Math.Abs(ts.TotalSeconds);

        switch (delta)
        {
            case < 60:
                return ts.Seconds == 1 ? "刚刚" : ts.Seconds + "秒前";
            case < 60 * 2:
                return "1分钟前";
            case < 45 * 60:
                return ts.Minutes + "分钟前";
            case < 90 * 60:
                return "1小时前";
            case < 24 * 60 * 60:
                return ts.Hours + "小时前";
            case < 48 * 60 * 60:
                return "昨天";
            case < 30 * 24 * 60 * 60:
                return $"{ts.Days}天前";
            case < 12 * 30 * 24 * 60 * 60:
            {
                var months = Convert.ToInt32(Math.Floor((double) ts.Days / 30));
                return months <= 1 ? "一个月前" : $"{months}个月前";
            }
            default:
            {
                var years = Convert.ToInt32(Math.Floor((double) ts.Days / 365));
                return years <= 1 ? "1年前" : $"{years}年前";
            }
        }
    }
}