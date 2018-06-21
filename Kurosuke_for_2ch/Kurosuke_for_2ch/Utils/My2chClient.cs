using Kurosuke_for_2ch.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Http;
using Portable.Text;
using System.Net;

namespace Kurosuke_for_2ch.Utils
{
    public class My2chClient
    {

        public async Task<ObservableCollection<Category>> GetItaList()
        {
            Uri uri = new Uri("http://menu.2ch.net/bbsmenu.html");
            var html = await GetHtmlFromUri(uri);


            //var html = htmlResult.Content.ToString();
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var font = doc.DocumentNode.Descendants("font").First();
            var fontText = font.InnerHtml.ToString();
            fontText = fontText.Replace("<br>", "");
            var menues = fontText.Replace("<b>", "|").Replace("</b>", "|").Split('|');
            var categoryList = new ObservableCollection<Models.Category>();
            for (int i = 0; i < menues.Count() / 2; i++)
            {
                var set = menues.Skip(2 * i + 1).Take(2).ToList();
                var itaList = new ObservableCollection<Models.Ita>();
                if (set.Count > 1)
                {
                    var urlstrings = set[1];
                    Regex reg = new Regex("<a href=\"(?<url>.*?)\".*?>(?<text>.*?)</a>");
                    var matches = reg.Matches(urlstrings);
                    foreach (Match match in matches)
                    {
                        var ita = new Models.Ita(match.Groups["text"].Value, match.Groups["url"].Value);
                        itaList.Add(ita);
                    }

                    categoryList.Add(new Models.Category(set[0], itaList));
                }
                else
                {
                }
            }

            return categoryList;
        }

        private static async Task<string> GetHtmlFromUri(Uri uri)
        {
            string html;
            Windows.Web.Http.Filters.HttpBaseProtocolFilter filter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            filter.CacheControl.ReadBehavior =
                Windows.Web.Http.Filters.HttpCacheReadBehavior.MostRecent;


            using (var client = new HttpClient(filter))
            {
                using (var res = await client.GetAsync(uri))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        html = await ParseHTML(res);
                    }
                    else
                    {
                        throw new Exception(res.StatusCode + ":" + res.ReasonPhrase);
                    }
                }
            }

            return html;
        }

        private static async Task<string> ParseHTML(HttpResponseMessage res)
        {
            var sjis = Encoding.GetEncoding("Shift_JIS");
            var stream = await res.Content.ReadAsBufferAsync();
            using (var reader = new StreamReader(stream.AsStream(), sjis))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public async Task<ObservableCollection<Thread>> GetThreadList(Ita ita, ObservableCollection<Thread> ThreadList)
        {
            var uri = ita.GetThreadListUri();
            var html = await GetHtmlFromUri(uri);

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var smalls = doc.DocumentNode.Descendants("small");
            var small = (from item in smalls
                         where (from innerItem in item.Attributes
                                where innerItem.Name == "id"
                                select innerItem.Value).FirstOrDefault() == "trad"
                         select item).FirstOrDefault();

            if (small != null)
            {
                var atags = small.InnerHtml.Replace("<a", "|<a").Split('|');
                foreach (var atag in atags)
                {
                    var regex = new Regex("<a href=\"(?<url>.*?)\".*?>(?<number>\\d*?): (?<text>.*?) \\((?<count>\\d*?)\\)</a>");
                    var match = regex.Match(atag);
                    var url = match.Groups["url"].Value;
                    var number = match.Groups["number"].Value;
                    var text = match.Groups["text"].Value;
                    var count = match.Groups["count"].Value;

                    if (!string.IsNullOrEmpty(url))
                    {
                        var newThread = new Thread(WebUtility.HtmlDecode(text), url.Replace("/l50", ""), int.Parse(number), ita, int.Parse(count));
                        newThread.Ita = ita;
                        newThread.ItaId = ita.ItaId;
                        ThreadList.Add(newThread);
                    }
                }

                return ThreadList;
            }
            throw new Exception("スレッド一覧のパースに失敗しました。");
        }

        public async Task<ObservableCollection<Post>> GetFullPostList(Thread thread)
        {
            var html = await GetHtmlFromUri(thread.GetFullThreadUri());
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var ResponseList = new ObservableCollection<Post>();

            var divs = doc.DocumentNode.Descendants("div");
            IEnumerable<HtmlAgilityPack.HtmlNode> htmlPosts = FindNodesByClassName(divs, "post");

            foreach (var htmlPost in htmlPosts)
            {
                /*postタグから読み取り*/
                int id = 0;
                string stringId = FindAttributeValueByName(htmlPost, "id");
                if (!string.IsNullOrEmpty(stringId))
                {
                    id = int.Parse(stringId);
                }

                int dataId = 0;
                string stringDataId = FindAttributeValueByName(htmlPost, "data-id");
                if (!string.IsNullOrEmpty(stringId))
                {
                    dataId = int.Parse(stringDataId);
                }

                string dataUserId = FindAttributeValueByName(htmlPost, "data-userid");
                string dataDate = FindAttributeValueByName(htmlPost, "data-date");

                /*内部のdivを解析*/
                var innerDivs = htmlPost.Descendants("div");

                string number = "";
                var numberdiv = FindNodesByClassName(innerDivs, "number").FirstOrDefault();
                if (numberdiv != null)
                {
                    number = numberdiv.InnerText;
                }

                string name = "";
                string mailTo = "";
                var namediv = FindNodesByClassName(innerDivs, "name").FirstOrDefault();
                if (namediv != null)
                {
                    var b = namediv.Descendants("b").FirstOrDefault();
                    if (b != null)
                    {
                        var a = b.Descendants("a").FirstOrDefault();
                        if (a == null)
                        {
                            name = b.InnerText;
                        }
                        else
                        {
                            var regex = new Regex("<a href=\"mailto:(?<url>.*?)\".*?>(?<text>.*?)</a>");
                            var match = regex.Match(b.InnerHtml);
                            name = match.Groups["text"].Value;
                            mailTo = match.Groups["url"].Value;
                        }
                    }
                }

                string datestring = "";
                var datediv = FindNodesByClassName(innerDivs, "date").FirstOrDefault();
                if (datediv != null)
                {
                    datestring = datediv.InnerText;
                }

                string message = "";
                var messagediv = FindNodesByClassName(innerDivs, "message").FirstOrDefault();
                if (messagediv != null)
                {
                    message = WebUtility.HtmlDecode(messagediv.InnerHtml.Replace("<br>", "\n"));
                }

                var newPost = new Post(id, dataId, dataUserId, dataDate, number, name, mailTo, datestring, message, thread);
                ResponseList.Add(newPost);
            }
            return ResponseList;
        }

        public async Task<System.Net.Http.HttpResponseMessage> PostUpdate(Thread thread, string from, string mail, string postMessage)
        {
            //var message = WebUtility.UrlEncode(postMessage);
            var message = postMessage;
            var html = await GetHtmlFromUri(thread.GetFullThreadUri());
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var form = doc.DocumentNode.Descendants("form").FirstOrDefault();

            if (form != null)
            {
                var action = FindAttributeValueByName(form, "action");

                var inputs = doc.DocumentNode.Descendants("input");
                if (inputs != null)
                {
                    var bbs = FindInputValueByAttributeName(inputs, "bbs");
                    var key = FindInputValueByAttributeName(inputs, "key");
                    var time = FindInputValueByAttributeName(inputs, "time");

                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    var enc = Encoding.GetEncoding("Shift_JIS");

                    var client = new System.Net.Http.HttpClient();
                    client.DefaultRequestHeaders.Add("Accept", @"text/html, application/xhtml+xml, image/jxr, ");
                    client.DefaultRequestHeaders.Add("Referer", thread.Ita.Url + thread.Url);
                    client.DefaultRequestHeaders.Add("Accept-Language", @"ja");
                    client.DefaultRequestHeaders.Add("User-Agent", @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.10240");
                    //client.DefaultRequestHeaders.Add("Content-Type", @"application/x-www-form-urlencoded");
                    //client.DefaultRequestHeaders.Add("Accept-Encoding", @"gzip, deflate");
                    client.DefaultRequestHeaders.Add("Host", @"echo.2ch.net");


                    // 受け入れ言語をセット（オプション）
                    //client.DefaultRequestHeaders.Add("Accept-Language", "ja-JP");


                    var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, action + "?guid=ON");

                    var requestContent = string.Format("submit={0}&FROM={1}&mail={2}&MESSAGE={3}&bbs={4}&key={5}&time={6}&oekaki_thread1={7}",
                        Uri.EscapeDataString("書き込む"),
                        Uri.EscapeDataString(from),
                        Uri.EscapeDataString(mail),
                        Uri.EscapeDataString(message),
                        Uri.EscapeDataString(bbs),
                        Uri.EscapeDataString(key),
                        Uri.EscapeDataString(time),
                        Uri.EscapeDataString(""));
                    request.Content = new System.Net.Http.StringContent(requestContent, enc, "application/x-www-form-urlencoded");

                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var sjis = Encoding.GetEncoding("Shift_JIS");
                        var stream = await response.Content.ReadAsStreamAsync();
                        using (var reader = new StreamReader(stream, sjis))
                        {
                            var reshtml = await reader.ReadToEndAsync();
                            var doc2 = new HtmlAgilityPack.HtmlDocument();
                            doc2.LoadHtml(reshtml);

                            //var inputs2 = doc2.DocumentNode.Descendants("input");
                            var text = doc2.DocumentNode.InnerText;
                            if (text.Contains("書きこみが終わりました。"))
                            {
                                return response;
                            }
                            else
                            {
                                throw new Exception(reshtml);
                                /*
                                var bbs2 = FindInputValueByAttributeName(inputs, "bbs");
                                var key2 = FindInputValueByAttributeName(inputs, "key");
                                var time2 = FindInputValueByAttributeName(inputs, "time");
                                var from2 = FindInputValueByAttributeName(inputs, "FROM");
                                var mail2 = FindInputValueByAttributeName(inputs, "mail");
                                var message2 = FindInputValueByAttributeName(inputs, "MESSAGE");
                                var subject = FindInputValueByAttributeName(inputs, "subject");
                                var sid = FindInputValueByAttributeName(inputs, "sid");
                                var submit = FindInputValueByAttributeName(inputs, "submit");
                             */
                            }
                        }
                    }
                }
            }
            throw new Exception("書き込みに失敗しました。");
        }

        /*public async Task<HttpResponseMessage> PostUpdate(Thread thread, string from, string mail, string postMessage)
        {
            //var message = WebUtility.UrlEncode(postMessage);
            var message = postMessage;
            var html = await GetHtmlFromUri(thread.GetFullThreadUri());
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var form = doc.DocumentNode.Descendants("form").FirstOrDefault();

            if (form != null)
            {
                var action = FindAttributeValueByName(form, "action");

                var inputs = doc.DocumentNode.Descendants("input");
                if (inputs != null)
                {
                    var bbs = FindInputValueByAttributeName(inputs, "bbs");
                    var key = FindInputValueByAttributeName(inputs, "key");
                    var time = FindInputValueByAttributeName(inputs, "time");

                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    var enc = Encoding.GetEncoding("Shift_JIS");

                    var submit = enc.GetBytes("書き込む");
                    var encodedMessage = enc.GetBytes(message);

                    var contentList = new List<KeyValuePair<string, string>>();
                    //contentList.Add(new KeyValuePair<string, string>("submit", submit.AsBuffer()));
                    contentList.Add(new KeyValuePair<string, string>("FROM", from));
                    contentList.Add(new KeyValuePair<string, string>("mail", mail));
                    //contentList.Add(new KeyValuePair<string, string>("MESSAGE", message));
                    contentList.Add(new KeyValuePair<string, string>("bbs", bbs));
                    contentList.Add(new KeyValuePair<string, string>("key", key));
                    contentList.Add(new KeyValuePair<string, string>("time", time));
                    contentList.Add(new KeyValuePair<string, string>("oekaki_thread1", ""));
                    //formContent.Headers.Add(new KeyValuePair<string, string>("action", action));
                    var content = new HttpFormUrlEncodedContent(contentList);

                    content.Headers.ContentType = new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue("application/x-www-form-urlencoded");

                    var acontent = new HttpMultipartFormDataContent();
                    acontent.Add(new HttpBufferContent(submit.AsBuffer()), "submit");
                    acontent.Add(new HttpBufferContent(encodedMessage.AsBuffer()), "submit");
                    acontent.Add(content);

                    acontent.Headers.ContentType = new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue("application/x-www-form-urlencoded");


                    Windows.Web.Http.Filters.HttpBaseProtocolFilter filter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
                    filter.CacheControl.ReadBehavior =
                        Windows.Web.Http.Filters.HttpCacheReadBehavior.MostRecent;
                    var client = new HttpClient(filter);
                    client.DefaultRequestHeaders.UserAgent.Add(new Windows.Web.Http.Headers.HttpProductInfoHeaderValue(@"Mozilla / 5.0(Windows NT 10.0; < 64 - bit tags >) AppleWebKit /< WebKit Rev > (KHTML, like Gecko) Chrome /< Chrome Rev > Safari /< WebKit Rev > Edge /< EdgeHTML Rev >.< Windows Build >"));
                    client.DefaultRequestHeaders.CacheControl.Add(new Windows.Web.Http.Headers.HttpNameValueHeaderValue("no-cache"));
                    client.DefaultRequestHeaders.Accept.Add(new Windows.Web.Http.Headers.HttpMediaTypeWithQualityHeaderValue("text/html, application/xhtml+xml, image/jxr,"));
                    client.DefaultRequestHeaders.Referer = thread.GetFullThreadUri();
                    client.DefaultRequestHeaders.AcceptLanguage.Add(new Windows.Web.Http.Headers.HttpLanguageRangeWithQualityHeaderValue("ja"));
                    client.DefaultRequestHeaders.AcceptEncoding.Add(new Windows.Web.Http.Headers.HttpContentCodingWithQualityHeaderValue("gzip, deflate"));

                    var res = await client.PostAsync(new Uri(action), acontent);

                    if (res.IsSuccessStatusCode)
                    {
                        var reshtml = await ParseHTML(res);
                        var doc2 = new HtmlAgilityPack.HtmlDocument();
                        doc2.LoadHtml(reshtml);

                        var inputs2 = doc2.DocumentNode.Descendants("input");

                        if (inputs2 == null || inputs2.Count() == 8)
                        {
                            return res;
                        }
                        else
                        {
                            /*
                            var bbs2 = FindInputValueByAttributeName(inputs, "bbs");
                            var key2 = FindInputValueByAttributeName(inputs, "key");
                            var time2 = FindInputValueByAttributeName(inputs, "time");
                            var from2 = FindInputValueByAttributeName(inputs, "FROM");
                            var mail2 = FindInputValueByAttributeName(inputs, "mail");
                            var message2 = FindInputValueByAttributeName(inputs, "MESSAGE");
                            var subject = FindInputValueByAttributeName(inputs, "subject");
                            var sid = FindInputValueByAttributeName(inputs, "sid");
                            var submit = FindInputValueByAttributeName(inputs, "submit");
                            
                        }
                    }
                }
            }
            throw new Exception("書き込みに失敗しました。");
        }*/

        private static string FindInputValueByAttributeName(IEnumerable<HtmlAgilityPack.HtmlNode> inputs, string attributeName)
        {
            return (from input in inputs
                    where FindAttributeValueByName(input, "name") == attributeName
                    select FindAttributeValueByName(input, "value")).FirstOrDefault();
        }

        private static string FindAttributeValueByName(HtmlAgilityPack.HtmlNode Node, string attributeName)
        {
            return (from attribute in Node.Attributes
                    where attribute.Name == attributeName
                    select attribute.Value).FirstOrDefault();
        }

        private static IEnumerable<HtmlAgilityPack.HtmlNode> FindNodesByClassName(IEnumerable<HtmlAgilityPack.HtmlNode> Nodes, string ClassName)
        {
            return from node in Nodes
                   where FindAttributeValueByName(node, "class") == ClassName
                   select node;
        }
    }
}
