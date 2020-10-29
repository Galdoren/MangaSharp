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

namespace Manga.Plugin.Publishers.Mangafox.Services
{
    public class MangafoxService : IPublisherWebService
    {
        #region Constants

        private const string GETLIST_URL = "http://mangafox.me/manga/";
        private const string SIZE_PATTERN = "(?<=of )([0-9]*)(?=\\t)";

        #endregion

        #region Fields
        
        private readonly IPublisherService _publisherService;
        private readonly IMangaService _mangaService;
        private readonly IChapterService _chapterService;
        //private readonly IDownloadManager _downloadManager;
        private readonly Regex _regex;

        #endregion

        #region Ctor

        public MangafoxService(IPublisherService publisherService,
            IMangaService mangaService,
            IChapterService chapterService)
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
                        HtmlDocument doc = new HtmlDocument();
                        // if there is a stream load the html content
                        if (mystream != null)
                        {
                            doc.Load(mystream);
                        }

                        // create a collection of nodes which holds chapters division in html code
                        HtmlNodeCollection coll = doc.DocumentNode.SelectNodes("//div[@id='chapters']//ul[@class='chlist']//li//div");
                        if (coll != null) // if there are chapters which means collection is not empty, continue
                        {
                            IList<Manga.Core.Domain.Chapter> tempList = new List<Manga.Core.Domain.Chapter>();
                            // foreach node in collection, get info about chapter such as
                            for (int i = coll.Count - 1; i >= 0; i--)
                            {
                                Manga.Core.Domain.Chapter chapter = new Core.Domain.Chapter();

                                String name1;
                                // get the extended name of the chapter such as Chapter 13 - !""This part""!
                                HtmlNode node1 = coll[i].SelectSingleNode("h3//span[@class='title nowrap'] | h4//span[@class='title nowrap']");
                                if (node1 == null) // if extended name doesnt exists set name1 to null
                                    name1 = null;
                                else // otherwise get the name and store it in name1 field
                                    name1 = coll[i].SelectSingleNode("h3//span[@class='title nowrap'] | h4//span[@class='title nowrap']").InnerText;
                                // get the number of  the chapter such as !""This Part""! - Hello World and store it in name2 field
                                String name2 = coll[i].SelectSingleNode("h3//a | h4//a").InnerText.RemoveExtaSpaces();
                                // merge the name, if name1 exsists, store them in one string-name-, if it does not store only name2
                                chapter.Name = (name1 == null ? name2.RemoveExtaSpaces() : (name2.RemoveExtaSpaces() + " : " + name1.RemoveExtaSpaces()));
                                // get link of the chapter from nodes
                                chapter.Url = coll[i].SelectSingleNode("h3//a | h4//a").Attributes["href"].Value;
                                // set the chapter order index
                                chapter.Index = coll.Count - 1 - i;
                                // get unique id of the manga and set it to this chapter                                
                                chapter.MangaId = manga.Id;
                                // get release date of the chapter
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
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(manga.URL);
                using (WebResponse webRes = await webReq.GetResponseAsync()) // get web response
                {
                    // open a streamreader to read response content
                    using (System.IO.StreamReader mystream = new StreamReader(webRes.GetResponseStream()))
                    {
                        // htmldocument is used for seeing html code as xml code and selecting spesific parts easily
                        // it decreases time to extract the parts we want from html
                        HtmlDocument doc = new HtmlDocument();
                        // if there is a stream load the html content
                        if (mystream != null)
                        {
                            doc.Load(mystream);
                        }

                        HtmlNodeCollection col = doc.DocumentNode.SelectNodes("//*[@id='title']//table//tr[2]/td");
                        
                        manga.Year = int.Parse(col[0].SelectSingleNode("a").InnerText);

                        HtmlNodeCollection authcol = col[1].SelectNodes("a");

                        if (authcol != null)
                        {
                            manga.Author = String.Empty;
                            foreach (HtmlNode node in authcol)
                            {
                                manga.Author += WebUtility.HtmlDecode(node.InnerText);
                            }
                        }

                        HtmlNodeCollection artcol = col[2].SelectNodes("a");

                        if (artcol != null)
                        {
                            manga.Artist = String.Empty;
                            foreach (HtmlNode node in artcol)
                            {
                                manga.Artist += WebUtility.HtmlDecode(node.InnerText);
                            }
                        }

                        HtmlNodeCollection genrecol = col[3].SelectNodes("a");

                        if (genrecol != null)
                        {
                            manga.Genres = new List<string>(genrecol.Count);
                            for (int i = 0; i < genrecol.Count; i++)
                            {
                                manga.Genres.Add(WebUtility.HtmlDecode(genrecol[i].InnerText));
                            }
                        }

                        HtmlNode node2 = doc.DocumentNode.SelectSingleNode("//*[@id='title']//p[@class='summary']");
                        String summary = (node2 != null ? node2.InnerText : null);
                        manga.Description = WebUtility.HtmlDecode(summary);

                        HtmlNode imgnode = doc.DocumentNode.SelectSingleNode("//div[@class='left']//div[@id='series_info']//div[@class='cover']//img");
                        if (imgnode != null)
                        {
                            string imgpath = imgnode.Attributes["src"].Value;

                            //manga.CoverImage = GetImageFromUrl(imgpath);

                            manga.ImageUrl = imgpath;
                        }

                        HtmlNodeCollection coll = doc.DocumentNode.SelectNodes("//div[@id='chapters']//ul[@class='chlist']//li//div");
                        IEqualityComparer<Chapter> comparer = new ChapterUrlEqualityComparer();
                        if (coll != null)
                        {
                            for (int i = coll.Count - 1; i >= 0; i--)
                            {
                                Manga.Core.Domain.Chapter chapter = new Manga.Core.Domain.Chapter();
                                String name1;
                                HtmlNode node1 = coll[i].SelectSingleNode("h3//span[@class='title nowrap'] | h4//span[@class='title nowrap']");
                                if (node1 == null)
                                    name1 = null;
                                else
                                    name1 = coll[i].SelectSingleNode("h3//span[@class='title nowrap'] | h4//span[@class='title nowrap']").InnerText;
                                String name2 = coll[i].SelectSingleNode("h3//a | h4//a").InnerText.RemoveExtaSpaces();
                                chapter.Name = (name1 == null ? name2.RemoveExtaSpaces() : (name2.RemoveExtaSpaces() + " : " + name1.RemoveExtaSpaces()));
                                chapter.Url = coll[i].SelectSingleNode("h3//a | h4//a").Attributes["href"].Value;
                                chapter.Index = (coll.Count - 1) - i;
                                chapter.MangaId = manga.Id;
                                chapter.Date = coll[i].SelectSingleNode("span[@class='date']").InnerText.ToDateTime();
                                chapter.IsDownloaded = false;
                                chapter.DownloadedImageCount = 0;
                                chapter.DownloadPath = String.Empty;
                                //chapter.Date = coll[i].SelectSingleNode("span[@class='date']").InnerText;

                                if (manga.Chapters.Contains(chapter, comparer))
                                    continue;

                                manga.Chapters.Add(chapter);
                            }
                        }
                    }
                }
                _mangaService.UpdateManga(manga);
            }
            catch(Exception)
            {
                throw;
            }
        }

        protected virtual async Task DownloadChapterToFolder(Core.Domain.Chapter chapter, DownloadOptions options, ChapterQueue queue)
        {
            //options.Progress.Text = String.Format("Downloading {0}", chapter.Name);
            // Create directory path
            string dir = string.Format("{0}\\{1}\\{2}", options.Path,
                    chapter.Manga.Name.Trim().RemoveIllegalCharacters(),
                    chapter.Name.Trim().RemoveIllegalCharacters());

            System.IO.Directory.CreateDirectory(dir);

            // Get chapter size
            chapter.Size = await GetChapterSize(chapter);
            int l_index = chapter.Url.LastIndexOf('/');

            for(int i = 0; i < chapter.Size; i++)
            {
                HtmlDocument document = new HtmlDocument();
                string url = string.Format("{0}{1}.html", chapter.Url.Substring(0, l_index + 1), i + 1);
                string imageUrl = string.Empty;

                WebRequest request = HttpWebRequest.Create(url);
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        if (stream != null)
                            document.Load(stream);

                        HtmlNode node = document.DocumentNode.SelectSingleNode("//body/div/a/img");
                        imageUrl = node.Attributes["src"].Value;
                        stream.Close();
                    }
                    response.Close();
                }

                if(imageUrl != string.Empty)
                {
                    /*
                    _downloadManager.AddTask(new DownloadJob()
                    {
                        Url = imageUrl,
                        LocalPath = dir,
                        Progress = (p) => { p.SubProgress = (i / chapter.Size) * 100; }
                    });
                    */
                }

                // report progress
                //options.Progress.SubProgress = (i / chapter.Size) * 100;
                //options.Progress.Text = String.Format("Downloading {0} Finished", chapter.Name);
            }
        }

        protected virtual async Task<int> GetChapterSize(Core.Domain.Chapter chapter)
        {
            // number to return
            int value;
            // skipping this part for more info check other parts
            HtmlDocument Doc = new HtmlDocument();
            System.Net.WebRequest webReq = System.Net.WebRequest.Create(chapter.Url);
            using (System.Net.WebResponse webRes = await webReq.GetResponseAsync())
            {
                using (System.IO.Stream mystream = webRes.GetResponseStream())
                {
                    if (mystream != null)
                    {
                        Doc.Load(mystream);
                    }
                    else
                    {
                        webRes.Close();
                        return -1;
                    }

                    HtmlNode node = Doc.DocumentNode.SelectSingleNode("//div[@class='l']");
                    // get the size part from html content by using regular expressions
                    value = int.Parse(_regex.Match(node.InnerText).Value);

                    mystream.Close();
                }
                webRes.Close();
            }
            return value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the list from mangafox directory
        /// </summary>
        /// <returns></returns>
        public async Task<IList<Core.Domain.Manga>> GetList()
        {
            Console.WriteLine("Mangafox GetList");
            IList<Manga.Core.Domain.Manga> list_db = new List<Manga.Core.Domain.Manga>();
            IList<Manga.Core.Domain.Manga> list = new List<Manga.Core.Domain.Manga>();
            var publisher = _publisherService.GetPublisherByName("Mangafox");
            list_db = _mangaService.GetAllManga(publisher.Id);
            
            try
            {
                DateTime start = DateTime.Now;
                System.Net.ServicePointManager.Expect100Continue = false;
                System.Net.ServicePointManager.UseNagleAlgorithm = false;
                System.Net.ServicePointManager.CheckCertificateRevocationList = false;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(GETLIST_URL);
                request.Proxy = null;
                
                Console.WriteLine("Loading Request took {0} ms", (DateTime.Now - start).TotalMilliseconds);
                start = DateTime.Now;
                using (WebResponse response = await request.GetResponseAsync()) // get web response
                {   // open a streamreader to read response content
                    Console.WriteLine("Loading Response took {0} ms", (DateTime.Now - start).TotalMilliseconds);
                    start = DateTime.Now;
                    using (var ms = new MemoryStream())
                    {
                        using(var stream = new BufferedStream(response.GetResponseStream()))
                        {
                            Console.WriteLine("Loading stream took {0} ms", (DateTime.Now - start).TotalMilliseconds);
                            start = DateTime.Now;
                            stream.CopyTo(ms);
                            Console.WriteLine("Copying stream took {0} ms", (DateTime.Now - start).TotalMilliseconds);
                        }
                        ms.Seek(0, SeekOrigin.Begin);
                        // htmldocument is used for seeing html code as xml code and selecting spesific parts easily
                        // it decreases time to extract the parts we want from html
                        HtmlDocument doc = new HtmlDocument();
                        // if there is a stream load the html content
                        start = DateTime.Now;
                        if (ms != null)
                        {
                            doc.Load(ms);
                        }
                        Console.WriteLine("Loading HtmlDocument took {0} ms", (DateTime.Now - start).TotalMilliseconds);
                        // select the manga list div field and get all the links from it
                        HtmlNodeCollection col = doc.DocumentNode.SelectNodes("//div[@class='manga_list']//li//a");
                        // create an array of mangamodel[number of nodes in col]

                        DateTime now = DateTime.Now;
                        // for each nodes in collection create a manga and fill the manga information and add it to the array
                        for (int i = 0; i < col.Count; i++)
                        {
                            var manga = new Manga.Core.Domain.Manga()
                            {
                                Name = WebUtility.HtmlDecode(col[i].InnerText),
                                URL = col[i].Attributes["href"].Value,
                                PublisherId = publisher.Id,
                                Year = 0,
                                Status = col[i].Attributes["class"].Value.Split(' ').Contains("manga_open") ? 1 : 0,
                                CreatedDate = now,
                                LastUpdatedDate = now,
                                Size = 0,
                                IsFavourite = false
                            };

                            list.Add(manga);
                            /*
                            if (list.Contains(manga))
                                continue;
                            list.Add(manga);
                            */
                        }

                        list_db = list.Except(list_db, new MangaUrlEqualityComparer()).ToList();
                        // Add to DB
                        _mangaService.InsertManga(list_db);
                    }
                }
            }
            catch (Exception e)
            {

            }
            list.Clear();
            list = _mangaService.GetAllManga(publisher.Id);
            return list;
        }

        public async Task GetDetails(Core.Domain.Manga manga, MangaDetailsLevel detailsLevel)
        {
            switch (detailsLevel)
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

        public async Task Download(IList<Core.Domain.Chapter> chapters, DownloadOptions options)
        {
            if(chapters.Count > 0)
            {                
                //DownloadProgress progress = new DownloadProgress(options.DownloadGuid, 0, 0, String.Format("Downloading the chapters of '{0}'", chapters.First().Manga.Name));

                for(int i = 0; i < chapters.Count; i++)
                {
                    await DownloadChapterToFolder(chapters[i], options);
                    //options.Progress.Progress++;

                    ChapterQueue queue = new ChapterQueue(chapters.Count);
                    queue.Manga = chapters.First().Manga.Name;
                }
            }
        }

        #endregion        
    }
}
