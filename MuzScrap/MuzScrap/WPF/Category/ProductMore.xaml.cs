using MuzScrap.BaseContext;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MuzScrap.WPF.Category
{
    /// <summary>
    /// Логика взаимодействия для ProductMore.xaml
    /// </summary>
    public partial class ProductMore : Window
    {
        private string Image1 { get; set; } = "https://www.muztorg.ru/img/no_photo.png";
        private string Image2 { get; set; } = "https://www.muztorg.ru/img/no_photo.png";

        public ProductMore(ProductCard productCard)
        {
            InitializeComponent();
            this.DataContext = productCard;
            if(productCard.Image2 == "https://www.muztorg.ru/img/no_photo.png" || productCard.Image2 == null)
            {
                Left.Visibility = Visibility.Hidden;
                Right.Visibility = Visibility.Hidden;
            }

            Image1 = productCard.Image;
            Image2 = productCard.Image2;

            if (productCard.Image != null)
            {
                WebClient webClient = new WebClient();
                byte[] imageBytes = webClient.DownloadData(productCard.Image);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(imageBytes);
                bitmapImage.EndInit();
                Image_Card.Source = bitmapImage;
            }
            else if (productCard.Image2 != null)
            {
                WebClient webClient = new WebClient();
                byte[] imageBytes = webClient.DownloadData(productCard.Image2);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(imageBytes);
                bitmapImage.EndInit();
                Image_Card.Source = bitmapImage;
            }

        }        

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            if (Image2 != null)
            {
                WebClient webClient = new WebClient();
                byte[] imageBytes = webClient.DownloadData(Image2);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(imageBytes);
                bitmapImage.EndInit();
                Image_Card.Source = bitmapImage;
            }
        }

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            if (Image1 != null)
            {
                WebClient webClient = new WebClient();
                byte[] imageBytes = webClient.DownloadData(Image1);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(imageBytes);
                bitmapImage.EndInit();
                Image_Card.Source = bitmapImage;
            }
        }
    }
}
