using MuzScrap.BaseContext;
using MuzScrap.Domain.Models;
using System.Linq;
using System.Windows;

namespace MuzScrap.WPF.Category
{
    /// <summary>
    /// Логика взаимодействия для ElectricProduct.xaml
    /// </summary>
    public partial class ElectricProduct : Window
    {
        private readonly UserIdNow _userID = new(default);
        public ElectricProduct(UserIdNow userID)
        {
            InitializeComponent();
            _userID = userID;
            ListProduct.ItemsSource = MuzScrapBdContext.GetInstance().Products.Where(x => x.ProductType.Contains("Элект")).ToList();
            TotalStrings.Text = MuzScrapBdContext.GetInstance().Products.Where(x => x.ProductType.Contains("Элект")).Count().ToString();
        }
        private void SearchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var filtered = MuzScrapBdContext.GetInstance().Products.ToList().Where(search =>
                (search.ProductType.Contains("Элект"))
                & (search.Title.ToLower().Contains(SearchBox.Text.ToLower())
                | search.Price.ToLower().Contains(SearchBox.Text.ToLower())));
            ListProduct.ItemsSource = filtered;
        }
        private void Button_AddWishlist_Click(object sender, RoutedEventArgs e)
        {
            Wishlist wishlist = new Wishlist();
            if (ListProduct.SelectedItem is Product gridListProduct)
            {
                wishlist.ProductId = gridListProduct.Id;
                wishlist.UserId = _userID.UserID;
            }

            using MuzScrapDbContext db = new MuzScrapDbContext();
            var wishlistGet = db.Wishlists.Where(x => x.UserId == _userID.UserID).Select(x => x.ProductId);
            if (!wishlistGet.Contains(wishlist.ProductId))
            {
                db.Wishlists.Add(wishlist);
                db.SaveChanges();
                string index = "Добавлено в избранное!";
                MessageBox.Show(index);
            }
            else
            {
                string index = "Товар уже есть в избранном!";
                MessageBox.Show(index);
            }
        }
        private void ListProduct_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ListProduct.SelectedItem == null) return;
            var selectedProduct = (ListProduct.SelectedItem as Product);

            using MuzScrapDbContext db = new MuzScrapDbContext();
            ProductCard productCard = new ProductCard();

            foreach (var product in db.Products.Where(x => x.Store == "https://www.muztorg.ru"))
            {
                if (product.Title.ToLower() == selectedProduct.Title.ToLower())
                {
                    productCard.Title = selectedProduct.Title.Trim();
                    productCard.Brand = selectedProduct.Brand.Trim();
                    productCard.ProductType = selectedProduct.ProductType.Trim();

                    productCard.Price = product.Price.Trim();
                    productCard.Source = product.Source.Trim();
                    productCard.Store = product.Store.Trim();
                }
            }
            foreach (var product in db.Products.Where(x => x.Store == "https://jazz-shop.ru"))
            {
                if (product.Title.ToLower() == selectedProduct.Title.ToLower())
                {
                    productCard.Title = selectedProduct.Title.Trim();
                    productCard.Brand = selectedProduct.Brand.Trim();
                    productCard.ProductType = selectedProduct.ProductType;

                    productCard.Price2 = product.Price.Trim();
                    productCard.Source2 = product.Source.Trim();
                    productCard.Store2 = product.Store.Trim();
                }
            }
            ProductMore productMore = new ProductMore(productCard);
            productMore.ShowDialog();
        }
    }
}
