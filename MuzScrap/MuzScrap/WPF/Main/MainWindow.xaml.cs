using MuzScrap.BaseContext;
using MuzScrap.WPF.Category;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using System.ComponentModel;
using MuzScrap.Domain.Models;
using System.Windows.Media.Imaging;

namespace MuzScrap.WPF.Main
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UserIdNow _userID = new(default);
        public delegate void ThreadStart();
        public MainWindow(UserIdNow userID)
        {
            InitializeComponent();
            _userID = userID;

            Thread myThread1 = new Thread(Parsing_Muztorg_Acoustic);
            Thread myThread2 = new Thread(Parsing_Muztorg_Electro);
            Thread myThread3 = new Thread(Parsing_Muztorg_Bass);
            Thread myThread4 = new Thread(Parsing_JazzShop_Acoustic);
            //Thread myThread5 = new Thread(Parsing_JazzShop_Electro);
            //Thread myThread6 = new Thread(Parsing_JazzShop_Bass);

            myThread1.Start();
            myThread2.Start();
            myThread3.Start();
            myThread4.Start();
            //myThread5.Start();
            //myThread6.Start();



            void Parsing_Muztorg_Acoustic()
            {
                static string GetPage(string url)
                {
                    var result = String.Empty;
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    var response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseStream = response.GetResponseStream();
                        if (responseStream != null)
                        {
                            StreamReader streamReader;
                            if (response.CharacterSet != null)
                                streamReader = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
                            else
                                streamReader = new StreamReader(responseStream);
                            result = streamReader.ReadToEnd();
                            streamReader.Close();
                        }
                        response.Close();
                    }
                    return result;
                }

                var baseURL = "https://www.muztorg.ru/category/akusticheskie-gitary?in-stock=1";
                var txtHTML = GetPage(baseURL.ToString());
                var doc = new HtmlDocument();
                doc.LoadHtml(txtHTML);

                //var lastPageNumber = 1;
                var lastPageNumber = doc.DocumentNode.SelectNodes("//input[@id='pagination-total']/@value").Select(el => el.Attributes["value"].Value).Last();
                var baseURLPage = @"https://www.muztorg.ru";
                var catalogPages = new List<string>();
                catalogPages.AddRange(
                    Enumerable
                    .Range(1, Convert.ToInt32(lastPageNumber))
                    .Select(el => $"{baseURL}&page={el}")
                );

                var pages = new List<string>();
                foreach (var productPage in catalogPages)
                {
                    txtHTML = GetPage(productPage);
                    doc.LoadHtml(txtHTML);
                    pages.AddRange(doc.DocumentNode
                        .SelectNodes("//section[@class='product-thumbnail']")
                        .Select(el => el.InnerHtml)
                        .ToList()
                        );
                }

                var productTitle = new List<string>();
                var productType = new List<string>();
                var productPrice = new List<string>();
                var productBrand = new List<string>();
                var productSource = new List<string>();
                var productImage = new List<string>();
                foreach (var page in pages)
                {
                    doc.LoadHtml(page);
                    productTitle.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='product-header']//a")
                    .Where(el => el.Attributes["href"].Value.Trim().Contains("/product"))
                    .Select(el => el.InnerText.Trim().Trim())
                    );
                    productType.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='product-catalog-grid']//a")
                    .Where(el => el.Attributes["href"].Value.Trim().Contains("/product"))
                    .Select(el => el.InnerText.Trim().Replace("Акустическая гитара", "Акустические гитары").ToString())
                    );
                    productPrice.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='product-price']//p")
                    .Select(el => el.InnerText.Trim().Replace("&nbsp;", "").Replace("р.", "").Trim())
                    );
                    productBrand.AddRange(doc.DocumentNode
                    .SelectNodes("/meta")
                    .Select(el => el.Attributes["content"].Value.Trim())
                    );
                    productSource.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='product-header']//a")
                    .Select(el => el.Attributes["href"].Value.Trim())
                    .Where(el => el.Contains("/product"))
                    .Select(el => $"{baseURLPage}" + el)
                    .ToList()
                    );
                    productImage.AddRange(doc.DocumentNode
                    .SelectNodes("//img[@class='img-responsive']")
                    .Select(el => el.Attributes["data-src"]?.Value)
                    );
                }


                for (int ind = 0; ind < productSource.Count(); ind++)
                {
                    using MuzScrapDbContext db = new MuzScrapDbContext();
                    Product product = new Product();
                    product.Title = productTitle[ind].Trim();
                    product.ProductType = productType[ind].Trim();
                    product.Price = productPrice[ind].Trim();
                    product.Brand = productBrand[ind].Trim();
                    product.Source = productSource[ind].Trim();
                    product.Store = baseURLPage.Trim();
                    if (productImage[ind] == null)
                    {
                        product.Image = "https://www.muztorg.ru/img/no_photo.png";
                    }
                    else
                        product.Image = productImage[ind].Trim();

                    var pSource = db.Products.Select(p => p.Source);
                    if (!pSource.Contains(product.Source))
                    {
                        db.Products.Add(product);
                        db.SaveChanges();
                    }
                }
            }

            void Parsing_Muztorg_Electro()
            {
                static string GetPage(string url)
                {
                    var result = String.Empty;
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    var response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseStream = response.GetResponseStream();
                        if (responseStream != null)
                        {
                            StreamReader streamReader;
                            if (response.CharacterSet != null)
                                streamReader = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
                            else
                                streamReader = new StreamReader(responseStream);
                            result = streamReader.ReadToEnd();
                            streamReader.Close();
                        }
                        response.Close();
                    }
                    return result;
                }

                var baseURL = "https://www.muztorg.ru/category/elektrogitary?in-stock=1";
                var txtHTML = GetPage(baseURL.ToString());
                var doc = new HtmlDocument();
                doc.LoadHtml(txtHTML);

                //var lastPageNumber = 1;
                var lastPageNumber = doc.DocumentNode.SelectNodes("//input[@id='pagination-total']/@value").Select(el => el.Attributes["value"].Value).Last();
                var baseURLPage = @"https://www.muztorg.ru";
                var catalogPages = new List<string>();
                catalogPages.AddRange(
                    Enumerable
                    .Range(1, Convert.ToInt32(lastPageNumber))
                    .Select(el => $"{baseURL}&page={el}")
                );

                List<string> pages = new List<string>();
                foreach (var productPage in catalogPages)
                {
                    txtHTML = GetPage(productPage);
                    doc.LoadHtml(txtHTML);
                    pages.AddRange(doc.DocumentNode
                        .SelectNodes("//section[@class='product-thumbnail']")
                        .Select(el => el.InnerHtml)
                        .ToList()
                        );
                }

                var productTitle = new List<string>();
                var productType = new List<string>();
                var productPrice = new List<string>();
                var productBrand = new List<string>();
                var productSource = new List<string>();
                var productImage = new List<string>();
                foreach (var page in pages)
                {
                    doc.LoadHtml(page);
                    productTitle.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='product-header']//a")
                    .Where(el => el.Attributes["href"].Value.Trim().Contains("/product"))
                    .Select(el => el.InnerText.Trim().Trim())
                    );
                    productType.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='product-catalog-grid']//a")
                    .Where(el => el.Attributes["href"].Value.Trim().Contains("/product"))
                    .Select(el => el.InnerText.Trim().Replace("Электрогитара", "Электрогитары"))
                    );
                    productPrice.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='product-price']//p")
                    .Select(el => el.InnerText.Trim().Replace("&nbsp;", "").Replace("р.", "").Trim())
                    );
                    productBrand.AddRange(doc.DocumentNode
                    .SelectNodes("/meta")
                    .Select(el => el.Attributes["content"].Value.Trim())
                    );
                    productSource.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='product-header']//a")
                    .Select(el => el.Attributes["href"].Value.Trim())
                    .Where(el => el.Contains("/product"))
                    .Select(el => $"{baseURLPage}" + el)
                    .ToList()
                    );
                    productImage.AddRange(doc.DocumentNode
                    .SelectNodes("//img[@class='img-responsive']")
                    .Select(el => el.Attributes["data-src"]?.Value)
                    );
                }
                for (int ind = 0; ind < productSource.Count(); ind++)
                {
                    using MuzScrapDbContext db = new MuzScrapDbContext();
                    Product product = new Product();
                    product.Title = productTitle[ind].Trim();
                    product.ProductType = productType[ind].Trim();
                    product.Price = productPrice[ind].Trim();
                    product.Brand = productBrand[ind].Trim();
                    product.Source = productSource[ind].Trim();
                    if (productImage[ind] == null)
                    {
                        product.Image = "https://www.muztorg.ru/img/no_photo.png";
                    }
                    else
                        product.Image = productImage[ind].Trim(); product.Store = baseURLPage.Trim();

                    var pSource = db.Products.Select(p => p.Source);
                    if (!pSource.Contains(product.Source))
                    {
                        db.Products.Add(product);
                        db.SaveChanges();
                    }
                }
            }

            void Parsing_Muztorg_Bass()
            {
                static string GetPage(string url)
                {
                    var result = String.Empty;
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    var response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseStream = response.GetResponseStream();
                        if (responseStream != null)
                        {
                            StreamReader streamReader;
                            if (response.CharacterSet != null)
                                streamReader = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
                            else
                                streamReader = new StreamReader(responseStream);
                            result = streamReader.ReadToEnd();
                            streamReader.Close();
                        }
                        response.Close();
                    }
                    return result;
                }

                var baseURL = "https://www.muztorg.ru/category/bas-gitary?in-stock=1";
                var txtHTML = GetPage(baseURL.ToString());
                var doc = new HtmlDocument();
                doc.LoadHtml(txtHTML);

                //var lastPageNumber = 1;
                var lastPageNumber = doc.DocumentNode.SelectNodes("//input[@id='pagination-total']/@value").Select(el => el.Attributes["value"].Value).Last();
                var baseURLPage = @"https://www.muztorg.ru";
                var catalogPages = new List<string>();
                catalogPages.AddRange(
                    Enumerable
                    .Range(1, Convert.ToInt32(lastPageNumber))
                    .Select(el => $"{baseURL}&page={el}")
                );

                List<string> pages = new List<string>();
                foreach (var productPage in catalogPages)
                {
                    txtHTML = GetPage(productPage);
                    doc.LoadHtml(txtHTML);
                    pages.AddRange(doc.DocumentNode
                        .SelectNodes("//section[@class='product-thumbnail']")
                        .Select(el => el.InnerHtml)
                        .ToList()
                        );
                }

                var productTitle = new List<string>();
                var productType = new List<string>();
                var productPrice = new List<string>();
                var productBrand = new List<string>();
                var productSource = new List<string>();
                var productImage = new List<string>();
                foreach (var page in pages)
                {
                    doc.LoadHtml(page);
                    productTitle.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='product-header']//a")
                    .Where(el => el.Attributes["href"].Value.Trim().Contains("/product"))
                    .Select(el => el.InnerText.Trim().Trim())
                    );
                    productType.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='product-catalog-grid']//a")
                    .Where(el => el.Attributes["href"].Value.Trim().Contains("/product"))
                    .Select(el => el.InnerText.Trim().Replace("Бас-гитара", "Бас-гитары"))
                    );
                    productPrice.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='product-price']//p")
                    .Select(el => el.InnerText.Trim().Replace("&nbsp;", "").Replace("р.", "").Trim())
                    );
                    productBrand.AddRange(doc.DocumentNode
                    .SelectNodes("/meta")
                    .Select(el => el.Attributes["content"].Value.Trim())
                    );
                    productSource.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='product-header']//a")
                    .Select(el => el.Attributes["href"].Value.Trim())
                    .Where(el => el.Contains("/product"))
                    .Select(el => $"{baseURLPage}" + el)
                    .ToList()
                    );
                    productImage.AddRange(doc.DocumentNode
                    .SelectNodes("//img[@class='img-responsive']")
                    .Select(el => el.Attributes["data-src"]?.Value)
                     );
                }
                for (int ind = 0; ind < productSource.Count(); ind++)
                {
                    using MuzScrapDbContext db = new MuzScrapDbContext();
                    Product product = new Product();
                    product.Title = productTitle[ind].Trim();
                    product.ProductType = productType[ind].Trim();
                    product.Price = productPrice[ind].Trim();
                    product.Brand = productBrand[ind].Trim();
                    product.Source = productSource[ind].Trim();
                    if (productImage[ind] == null)
                    {
                        product.Image = "https://www.muztorg.ru/img/no_photo.png";
                    }
                    else
                        product.Image = productImage[ind].Trim();

                    product.Store = baseURLPage.Trim();

                    if (!db.Products.Select(p => p.Source).Contains(product.Source))
                    {
                        db.Products.AddRange(product);
                        db.SaveChanges();
                    }
                }
            }

            void Parsing_JazzShop_Acoustic()
            {
                static string GetPage(string url)
                {
                    var result = String.Empty;
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    var response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseStream = response.GetResponseStream();
                        if (responseStream != null)
                        {
                            StreamReader streamReader;
                            if (response.CharacterSet != null)
                                streamReader = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
                            else
                                streamReader = new StreamReader(responseStream);
                            result = streamReader.ReadToEnd();
                            streamReader.Close();
                        }
                        response.Close();
                    }
                    return result;
                }

                var baseURL = "https://jazz-shop.ru/tomsk/catalog/akusticeskie-gitary";
                var txtHTML = GetPage(baseURL.ToString());
                var doc = new HtmlDocument();
                doc.LoadHtml(txtHTML);

                //var lastPageNumber = 1;
                var lastPageNumber = doc.DocumentNode.SelectNodes("//li[@class='page-item last']//a[@class='page-link']").Select(el => el.Attributes["data-page"].Value).Last();
                lastPageNumber = (int.Parse(lastPageNumber.ToString()) + 1).ToString();

                var baseURLPage = "https://jazz-shop.ru";
                var catalogPages = new List<string>();
                catalogPages.AddRange(
                    Enumerable
                    .Range(1, Convert.ToInt32(lastPageNumber))
                    .Select(el => $"{baseURL}?in_stock=in_stock&page={el}")
                );

                var productType = new List<string>();
                var productSource = new List<string>();
                var productImage = new List<string>();
                foreach (var productPage in catalogPages)
                {
                    txtHTML = GetPage(productPage);
                    doc.LoadHtml(txtHTML);
                    productSource.AddRange(doc.DocumentNode
                        .SelectNodes("//div[@class='product-trumb-name']//a")
                        .Select(el => el.Attributes["href"].Value.ToString().Trim())
                        .ToList()
                        );
                    productType.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='product-trumb-category']//span")
                    .Select(el => el.InnerText.ToString().Replace("\"", "").Replace("Акустическая гитара", "Акустические гитары").Trim())
                    );
                    productImage.AddRange(doc.DocumentNode
                   .SelectNodes("//div[@class='product-thumb-image-box']//source[2]")
                   .Select(el => el.Attributes["srcset"]?.Value)
                   );
                }

                var inPage = new List<string>();
                foreach (var productPage in productSource)
                {
                    txtHTML = GetPage(productPage);
                    doc.LoadHtml(txtHTML);
                    inPage.AddRange(doc.DocumentNode
                        .SelectNodes("//div[@class='col-lg-7']")
                        .Select(el => el.InnerHtml)
                        .ToList()
                        );
                }

                var productTitle = new List<string>();
                var productPrice = new List<string>();
                var productBrand = new List<string>();
                foreach (var page in inPage)
                {
                    doc.LoadHtml(page);
                    productTitle.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='buy-btns']//a[@class='cart']")
                    .Select(el => el.Attributes["data-title"].Value.Trim())
                    .Select(el => el.ToString().Replace("Акустическая гитара ", "").Replace("Электроакустическая гитара ", "").Replace("“", "").Replace("”", "").Trim())
                    );
                    productPrice.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='buy-btns']//a[@class='cart']")
                    .Select(el => el.Attributes["data-price"].Value.Trim())
                    .Select(el => el.Replace(" ", "").Trim())
                    );
                    productBrand.AddRange(doc.DocumentNode
                    .SelectNodes("//a[@class='all-brand ml-10']//span[@itemprop='brand']")
                    .Select(el => el.InnerText.Trim())
                    );
                }

                for (int ind = 0; ind < productSource.Count(); ind++)
                {
                    using MuzScrapDbContext db = new MuzScrapDbContext();
                    Product product = new Product();
                    product.Title = productTitle[ind].Trim();
                    product.ProductType = productType[ind].Trim();
                    product.Price = productPrice[ind].Trim();
                    product.Brand = productBrand[ind].Trim();
                    product.Source = productSource[ind].Trim();
                    product.Store = baseURLPage.Trim();
                    if (productImage[ind] == null)
                    {
                        product.Image = "https://www.muztorg.ru/img/no_photo.png";
                    }
                    else
                        product.Image = productImage[ind].Trim();

                    var pSource = db.Products.Select(p => p.Source);
                    if (!pSource.Contains(product.Source))
                    {
                        db.Products.Add(product);
                        db.SaveChanges();
                    }
                }
            }

            void Parsing_JazzShop_Electro()
            {
                static string GetPage(string url)
                {
                    var result = String.Empty;
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    var response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseStream = response.GetResponseStream();
                        if (responseStream != null)
                        {
                            StreamReader streamReader;
                            if (response.CharacterSet != null)
                                streamReader = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
                            else
                                streamReader = new StreamReader(responseStream);
                            result = streamReader.ReadToEnd();
                            streamReader.Close();
                        }
                        response.Close();
                    }
                    return result;
                }

                var baseURL = "https://jazz-shop.ru/tomsk/catalog/elektrogitary?in_stock=in_stock";
                var txtHTML = GetPage(baseURL.ToString());
                var doc = new HtmlDocument();
                doc.LoadHtml(txtHTML);

                //var lastPageNumber = 1;
                var lastPageNumber = doc.DocumentNode.SelectNodes("//li[@class='page-item last']//a[@class='page-link']").Select(el => el.Attributes["data-page"].Value).Last();
                lastPageNumber = (int.Parse(lastPageNumber.ToString()) + 1).ToString();

                var baseURLPage = "https://jazz-shop.ru";
                var catalogPages = new List<string>();
                catalogPages.AddRange(
                    Enumerable
                    .Range(1, Convert.ToInt32(lastPageNumber))
                    .Select(el => $"{baseURL}?in_stock=in_stock&page={el}")
                );

                var productType = new List<string>();
                var productSource = new List<string>();
                foreach (var productPage in catalogPages)
                {
                    txtHTML = GetPage(productPage);
                    doc.LoadHtml(txtHTML);
                    productSource.AddRange(doc.DocumentNode
                        .SelectNodes("//div[@class='product-trumb-name']//a")
                        .Select(el => el.Attributes["href"].Value.ToString().Trim())
                        .ToList()
                        );
                    productType.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='product-trumb-category']//span")
                    .Select(el => el.InnerText.ToString().Trim().Replace("Электрогитара", "Электрогитары"))
                    );
                }

                var inPage = new List<string>();
                foreach (var productPage in productSource)
                {
                    txtHTML = GetPage(productPage);
                    doc.LoadHtml(txtHTML);
                    inPage.AddRange(doc.DocumentNode
                        .SelectNodes("//div[@class='col-lg-7']")
                        .Select(el => el.InnerHtml)
                        .ToList()
                        );
                }

                var productTitle = new List<string>();
                var productPrice = new List<string>();
                var productBrand = new List<string>();
                foreach (var page in inPage)
                {
                    doc.LoadHtml(page);
                    productTitle.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='buy-btns']//a[@class='cart']")
                    .Select(el => el.Attributes["data-title"].Value.Trim())
                    .Select(el => el.ToString().Replace("Электрогитара ", "").Replace("Электроакустическая гитара ", "").Replace("“", "").Replace("”", "").Trim())
                    );
                    productPrice.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='buy-btns']//a[@class='cart']")
                    .Select(el => el.Attributes["data-price"].Value.Trim())
                    .Select(el => el.Replace(" ", "").Trim())
                    );
                    productBrand.AddRange(doc.DocumentNode
                    .SelectNodes("//a[@class='all-brand ml-10']//span[@itemprop='brand']")
                    .Select(el => el.InnerText.Trim())
                    );
                }

                for (int ind = 0; ind < productSource.Count(); ind++)
                {
                    using MuzScrapDbContext db = new MuzScrapDbContext();
                    Product product = new Product();
                    product.Title = productTitle[ind].Trim();
                    product.ProductType = productType[ind].Trim();
                    product.Price = productPrice[ind].Trim();
                    product.Brand = productBrand[ind].Trim();
                    product.Source = productSource[ind].Trim();
                    product.Store = baseURLPage.Trim();

                    var pSource = db.Products.Select(p => p.Source);
                    if (!pSource.Contains(product.Source))
                    {
                        db.Products.Add(product);
                        db.SaveChanges();
                    }
                }
            }

            void Parsing_JazzShop_Bass()
            {
                static string GetPage(string url)
                {
                    var result = String.Empty;
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    var response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseStream = response.GetResponseStream();
                        if (responseStream != null)
                        {
                            StreamReader streamReader;
                            if (response.CharacterSet != null)
                                streamReader = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
                            else
                                streamReader = new StreamReader(responseStream);
                            result = streamReader.ReadToEnd();
                            streamReader.Close();
                        }
                        response.Close();
                    }
                    return result;
                }

                var baseURL = "https://jazz-shop.ru/tomsk/catalog/bas-gitary?in_stock=in_stock";
                var txtHTML = GetPage(baseURL.ToString());
                var doc = new HtmlDocument();
                doc.LoadHtml(txtHTML);

                //var lastPageNumber = 1;
                var lastPageNumber = doc.DocumentNode.SelectNodes("//li[@class='page-item last']//a[@class='page-link']").Select(el => el.Attributes["data-page"].Value).Last();
                lastPageNumber = (int.Parse(lastPageNumber.ToString()) + 1).ToString();

                var baseURLPage = "https://jazz-shop.ru";
                var catalogPages = new List<string>();
                catalogPages.AddRange(
                    Enumerable
                    .Range(1, Convert.ToInt32(lastPageNumber))
                    .Select(el => $"{baseURL}?in_stock=in_stock&page={el}")
                );

                var productType = new List<string>();
                var productSource = new List<string>();
                foreach (var productPage in catalogPages)
                {
                    txtHTML = GetPage(productPage);
                    doc.LoadHtml(txtHTML);
                    productSource.AddRange(doc.DocumentNode
                        .SelectNodes("//div[@class='product-trumb-name']//a")
                        .Select(el => el.Attributes["href"].Value.ToString().Trim())
                        .ToList()
                        );
                    productType.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='product-trumb-category']//span")
                    .Select(el => el.InnerText.ToString().Trim().Replace("Бас-гитара", "Бас-гитары"))
                    );
                }

                var inPage = new List<string>();
                foreach (var productPage in productSource)
                {
                    txtHTML = GetPage(productPage);
                    doc.LoadHtml(txtHTML);
                    inPage.AddRange(doc.DocumentNode
                        .SelectNodes("//div[@class='col-lg-7']")
                        .Select(el => el.InnerHtml)
                        .ToList()
                        );
                }

                var productTitle = new List<string>();
                var productPrice = new List<string>();
                var productBrand = new List<string>();
                foreach (var page in inPage)
                {
                    doc.LoadHtml(page);
                    productTitle.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='buy-btns']//a[@class='cart']")
                    .Select(el => el.Attributes["data-title"].Value.Trim())
                    .Select(el => el.ToString().Replace("Бас-гитара ", "").Replace("“", "").Replace("”", "").Trim())
                    );
                    productPrice.AddRange(doc.DocumentNode
                    .SelectNodes("//div[@class='buy-btns']//a[@class='cart']")
                    .Select(el => el.Attributes["data-price"].Value.Trim())
                    .Select(el => el.Replace(" ", "").Trim())
                    );
                    productBrand.AddRange(doc.DocumentNode
                    .SelectNodes("//a[@class='all-brand ml-10']//span[@itemprop='brand']")
                    .Select(el => el.InnerText.Trim())
                    );
                }

                for (int ind = 0; ind < productSource.Count(); ind++)
                {
                    using MuzScrapDbContext db = new MuzScrapDbContext();
                    Product product = new Product();
                    product.Title = productTitle[ind].Trim();
                    product.ProductType = productType[ind].Trim();
                    product.Price = productPrice[ind].Trim();
                    product.Brand = productBrand[ind].Trim();
                    product.Source = productSource[ind].Trim();
                    product.Store = baseURLPage.Trim();

                    var pSource = db.Products.Select(p => p.Source);
                    if (!pSource.Contains(product.Source))
                    {
                        db.Products.Add(product);
                        db.SaveChanges();
                    }
                }
            }
        }

        private void All_Click(object sender, RoutedEventArgs e)
        {
            AllProduct allProduct = new AllProduct(_userID);
            allProduct.ShowDialog();
        }

        private void Guitar_Acoustic_Click(object sender, RoutedEventArgs e)
        {
            AcousticProduct acousticProduct = new AcousticProduct(_userID);
            acousticProduct.ShowDialog();
        }

        private void Guitar_Electro_Click(object sender, RoutedEventArgs e)
        {
            ElectricProduct electricProduct = new ElectricProduct(_userID);
            electricProduct.ShowDialog();
        }

        private void Guitar_Bass_Click(object sender, RoutedEventArgs e)
        {
            BassProduct bassProduct = new BassProduct(_userID);
            bassProduct.ShowDialog();
        }

        private void Wishlist_Click(object sender, RoutedEventArgs e)
        {
            WishlistWindow wishlistWindow = new WishlistWindow(_userID);
            wishlistWindow.ShowDialog();
        }

        //private void Source_Click(object sender, RoutedEventArgs e)
        //{

        //}
    }
}
