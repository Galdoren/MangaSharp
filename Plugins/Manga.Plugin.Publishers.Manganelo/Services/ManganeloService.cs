using HtmlAgilityPack;
using Manga.Core;
using Manga.Core.Domain;
using Manga.Services.Chapters;
using Manga.Services.Mangas;
using Manga.Services.Net;
using Manga.Services.Publishers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Manga.Plugin.Publishers.Manganelo.Services
{
    public class ManganeloService : IPublisherWebService
    {
        #region Constants

        private const string GETLIST_URL = "https://manganelo.com/advanced_search?s=all&orby=az&page={0}";
        private const string SIZE_PATTERN = "\\d+";

        #endregion

        #region Fields

        private readonly IPublisherService _publisherService;
        private readonly IMangaService _mangaService;
        private readonly IChapterService _chapterService;
        private readonly IDownloadManager _downloadManager;
        private readonly Regex _regex;

        #endregion

        #region Ctor

        public ManganeloService(IPublisherService publisherService,
            IMangaService mangaService,
            IChapterService chapterService
            /*IDownloadManager downloadManager*/)
        {
            this._publisherService = publisherService;
            this._mangaService = mangaService;
            this._chapterService = chapterService;
            //this._downloadManager = downloadManager;

            this._regex = new Regex(SIZE_PATTERN);
        }

        #endregion

        #region Utilities

        protected virtual async Task GetDetailsMinimal(Core.Domain.Manga manga)
        {
            try
            {
                // create a web request to the manga homepage
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(manga.URL);
                using (WebResponse webRes = await webReq.GetResponseAsync()) // get web response
                {
                    // open a streamreader to read response content
                    using (System.IO.StreamReader mystream = new StreamReader(webRes.GetResponseStream()))
                    {
                        // htmldocument is used for seeing html code as xml code and selecting spesific parts easily
                        // it decreases time to extract the parts we want from html
                        HtmlDocument document = new HtmlDocument();
                        // if there is a stream load the html content
                        if (mystream != null)
                        {
                            document.Load(mystream);
                        }

                        HtmlNodeCollection chapters = document.DocumentNode.SelectNodes("//div[@class='panel-story-chapter-list']//ul[@class='row-content-chapter']//li");
                        if(chapters != null)
                        {
                            IList<Manga.Core.Domain.Chapter> list = new List<Manga.Core.Domain.Chapter>(chapters.Count);
                            for (int i = chapters.Count - 1; i >= 0; i--)
                            {
                                Manga.Core.Domain.Chapter chapter = new Core.Domain.Chapter();
                                // get the name of the chapter
                                HtmlNode node = chapters[i];
                                HtmlNode link = node.SelectSingleNode("a");
                                // get name of the chapter
                                chapter.Name = link.InnerText.RemoveExtaSpaces();
                                // get link of the chapter from nodes
                                chapter.Url = link.Attributes["href"].Value;
                                // set the chapter order index
                                chapter.Index = chapters.Count - 1 - i;
                                // get unique id of the manga and set it to this chapter
                                chapter.MangaId = manga.Id;

                                // TODO
                                // convert 'today', 'yesterday', and these things to the date later
                                //chapter.Date = coll[i].SelectSingleNode("span[@class='date']").InnerText;       
                                manga.Chapters.Add(chapter);
                                _mangaService.UpdateManga(manga);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected virtual async Task GetDetailsExtended(Core.Domain.Manga manga)
        {
            try
            {
                // create a web request to the manga homepage
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(manga.URL);
                using (WebResponse response = await request.GetResponseAsync())
                {
                    // open a streamreader to read response content
                    using (System.IO.StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        HtmlDocument document = new HtmlDocument();
                        if (reader != null)
                        {
                            document.Load(reader);
                        }

                        HtmlNode info = document.DocumentNode.SelectSingleNode("//div[@class='panel-story-info']");

                        HtmlNode imageNode = info.SelectSingleNode("div[@class='story-info-left']/span/img");

                        if (imageNode != null)
                        {
                            manga.ImageUrl = imageNode.Attributes["src"].Value;
                        }

                        HtmlNode infoRight = info.SelectSingleNode("div[@class='story-info-right']");

                        HtmlNodeCollection nodes = infoRight.SelectNodes("table/tbody/tr/td[2]");
                        // Alternative name
                        manga.AlternativeName = nodes[0].SelectSingleNode("h2").InnerText;
                        // Author
                        manga.Author = nodes[1].SelectSingleNode("a").InnerText;
                        // Status
                        manga.Status = nodes[2].InnerText.Equals("Completed") ? 1 : 0;
                        // Genres
                        HtmlNodeCollection genres = nodes[3].SelectNodes("a");
                        manga.Genres = new List<string>(genres.Count);
                        for (int i = 0; i < genres.Count; i++)
                        {
                            manga.Genres.Add(genres[i].InnerText);
                        }
                        // Description
                        HtmlNode description = info.SelectSingleNode("div[@class='panel-story-info-description']");
                        if (description != null)
                        {
                            description.RemoveChild(description.SelectSingleNode("h3"));
                            manga.Description = description.InnerText;
                        }
                        else
                        {
                            manga.Description = null;
                        }
                        // Chapters
                        HtmlNodeCollection chapters = document.DocumentNode.SelectNodes("//div[@class='panel-story-chapter-list']//ul[@class='row-content-chapter']//li");
                        IEqualityComparer<Manga.Core.Domain.Chapter> comparer = new Manga.Core.Domain.ChapterUrlEqualityComparer();
                        if (chapters != null)
                        {
                            for (int i = chapters.Count - 1; i >= 0; i--)
                            {
                                Manga.Core.Domain.Chapter chapter = new Manga.Core.Domain.Chapter();
                                HtmlNode node = chapters[i];
                                HtmlNode link = node.SelectSingleNode("a");
                                chapter.Name = link.InnerText.RemoveExtaSpaces();
                                chapter.Url = link.Attributes["href"].Value;
                                chapter.Index = (chapters.Count - 1) - i;
                                chapter.MangaId = manga.Id;
                                chapter.IsDownloaded = false;
                                chapter.DownloadedImageCount = 0;
                                chapter.DownloadPath = String.Empty;
                                HtmlNode date = node.SelectSingleNode("span[contains(@class, 'chapter-time')]");
                                string dateString = date.Attributes["title"].Value;
                                chapter.Date = DateTime.Parse(dateString);

                                if (manga.Chapters.Contains(chapter, comparer))
                                    continue;
                                manga.Chapters.Add(chapter);
                            }
                        }
                    }
                }
                _mangaService.UpdateManga(manga);
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected virtual async Task DownloadChapterToFolder(Core.Domain.Chapter chapter, DownloadOptions options/*, ChapterQueue queue*/)
        {
            // Create directory path
            string dir = string.Format("{0}\\{1}\\{2}", options.Path,
                    chapter.Manga.Name.Trim().RemoveIllegalCharacters(),
                    chapter.Name.Trim().RemoveIllegalCharacters());
            System.IO.Directory.CreateDirectory(dir);
        }

        protected async Task<int> FetchListPageCount()
        {
            int result;
            string link = String.Format(GETLIST_URL, 1);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
            using (WebResponse response = await request.GetResponseAsync())
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    HtmlDocument document = new HtmlDocument();
                    if (reader != null)
                    {
                        document.Load(reader);
                    }
                    HtmlNode node = document.DocumentNode.SelectSingleNode("//div[@class='panel-page-number']/div[@class='group-page']/a[@class='page-blue page-last']");
                    result = int.Parse(_regex.Match(node.InnerText).Value);
                    reader.Close();
                }
                response.Close();
            }
            return result;
        }

        #endregion

        #region Methods

        public async Task<IList<Core.Domain.Manga>> GetList()
        {
            Console.Write("Manganelo GetList");
            var publisher = _publisherService.GetPublisherByName("Manganelo");
            IList<Manga.Core.Domain.Manga> list = _mangaService.GetAllManga(publisher.Id);
            return list;
        }

        public async Task<IList<Core.Domain.Manga>> Update()
        {
            Console.Write("Manganelo Update");
            IList<Manga.Core.Domain.Manga> list_db = new List<Manga.Core.Domain.Manga>();
            IList<Manga.Core.Domain.Manga> list = new List<Manga.Core.Domain.Manga>();
            var publisher = _publisherService.GetPublisherByName("Manganelo");
            list_db = _mangaService.GetAllManga(publisher.Id);

            WebRequest request = null;
            WebResponse response = null;

            int current = 1;
            int total = await FetchListPageCount();

            String link = String.Format(GETLIST_URL, current);

            try
            {
                DateTime start = DateTime.Now;
                do
                {
                    request = WebRequest.Create(link);
                    Console.WriteLine("Loading Request took {0} ms", (DateTime.Now - start).TotalMilliseconds);
                    start = DateTime.Now;
                    using (response = await request.GetResponseAsync())
                    {
                        // open a streamreader to read response content
                        Console.WriteLine("Loading Response took {0} ms", (DateTime.Now - start).TotalMilliseconds);
                        start = DateTime.Now;
                        using (var ms = new MemoryStream())
                        {
                            using (var stream = new BufferedStream(response.GetResponseStream()))
                            {
                                Console.WriteLine("Loading stream took {0} ms", (DateTime.Now - start).TotalMilliseconds);
                                start = DateTime.Now;
                                stream.CopyTo(ms);
                                Console.WriteLine("Copying stream took {0} ms", (DateTime.Now - start).TotalMilliseconds);
                            }
                            ms.Seek(0, SeekOrigin.Begin);
                            // htmldocument is used for seeing html code as xml code and selecting spesific parts easily
                            // it decreases time to extract the parts we want from html
                            HtmlDocument document = new HtmlDocument();
                            // if there is a stream load the html content
                            start = DateTime.Now;
                            if (ms != null)
                            {
                                document.Load(ms);
                            }
                            Console.WriteLine("Loading HtmlDocument took {0} ms", (DateTime.Now - start).TotalMilliseconds);
                            HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//div[@class='panel-content-genres']/div[@class='content-genres-item']");
                            DateTime now = DateTime.Now;

                            // for each nodes in collection create a manga and fill the manga information and add it to the array
                            for (int i = 0; i < nodes.Count; i++)
                            {
                                HtmlNode linkNode = nodes[i].SelectSingleNode("a[@class='genres-item-img']");
                                HtmlNode imageNode = linkNode.SelectSingleNode("img");
                                HtmlNode infoNode = nodes[i].SelectSingleNode("div[@class='genres-item-info']");
                                HtmlNode nameNode = infoNode.SelectSingleNode("h3/a[contains(@class, 'genres-item-name')]");


                                var manga = new Manga.Core.Domain.Manga()
                                {
                                    Name = nameNode.InnerText.RemoveExtaSpaces(),
                                    URL = linkNode.Attributes["href"].Value,
                                    PublisherId = publisher.Id,
                                    Year = 0,
                                    Status = 1,
                                    CreatedDate = now,
                                    LastUpdatedDate = now,
                                    Size = 0,
                                    IsFavourite = false,
                                    Author = infoNode.SelectSingleNode("//span[@class='genres-item-author']").InnerText.RemoveExtaSpaces(),
                                    Description = infoNode.SelectSingleNode("//div[@class='genres-item-description']").InnerText,
                                    ImageUrl = imageNode.Attributes["src"].Value,
                                };

                                list.Add(manga);
                            }
                        }
                        response.Close();
                    }
                    current++;
                    if (current >= total)
                    {
                        link = null;
                    }
                    else
                    {
                        link = String.Format(GETLIST_URL, current);
                    }
                    //link = null;
                }
                while (link != null);

                list_db = list.Except(list_db, new MangaUrlEqualityComparer()).ToList();
                var hash = new HashSet<string>();
                var duplicates = list.Where(i => !hash.Add(i.Name)).ToList();
                // Add to DB
                _mangaService.InsertManga(list_db);
            }
            catch (Exception)
            {
                throw;
            }
            list.Clear();
            list = _mangaService.GetAllManga(publisher.Id);
            return list;
        }

        public async Task GetDetails(Core.Domain.Manga manga, MangaDetailsLevel detailsLevel)
        {
            switch(detailsLevel)
            {
                case MangaDetailsLevel.Minimal:
                    await GetDetailsMinimal(manga);
                    break;
                case MangaDetailsLevel.Extended:
                    await GetDetailsExtended(manga);
                    break;
                default:
                    break;
            }
        }

        public async Task Download(IList<Manga.Core.Domain.Chapter> chapters, DownloadOptions options)
        {
            if(chapters.Count > 0)
            {
                for (int i = 0; i < chapters.Count; i++)
                {
                    await DownloadChapterToFolder(chapters[i], options);
                    ChapterQueue queue = new ChapterQueue(chapters.Count);
                    queue.Manga = chapters.First().Manga.Name;
                }
            }
        }

        #endregion

    }
}
