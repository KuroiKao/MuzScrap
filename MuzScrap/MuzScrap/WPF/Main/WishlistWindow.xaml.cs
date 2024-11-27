using Microsoft.EntityFrameworkCore;
using MuzScrap.BaseContext;
using MuzScrap.Domain.Models;
using MuzScrap.WPF.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MuzScrap.WPF.Main
{
    /// <summary>
    /// Логика взаимодействия для WishlistWindow.xaml
    /// </summary>
    public partial class WishlistWindow : Window
    {
        private readonly UserIdNow _userID = new(default);
        public WishlistWindow(UserIdNow userID)
        {
            InitializeComponent();
            _userID = userID;
            Load_Wishlist();
            List<string> comboProductType = new List<string>
            {
                "Все",
                "Акустические гитары",
                "Электрогитары",
                "Бас-гитары"
            };
            ComboBoxView.ItemsSource = comboProductType;
        }
        void Load_Wishlist()
        {
            ListProduct.ItemsSource = MuzScrapBdContext.GetInstance().Wishlists
                .Include(x => x.Product)
                .Where(x => x.ProductId == x.Product.Id && x.UserId == _userID.UserID)
                .ToList();
        }

        private void SearchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (ComboBoxView.SelectedItem == "Все")
            {
                var filteredAll = MuzScrapBdContext.GetInstance().Wishlists
                    .Include(x => x.Product)
                    .Where(x => x.ProductId == x.Product.Id && x.UserId == _userID.UserID)
                    .ToList()
                    .Where(x => x.Product.Title.ToLower().Contains(SearchBox.Text.ToLower())
                            | x.Product.Price.ToLower().Contains(SearchBox.Text.ToLower())
                            | x.Product.ProductType.ToLower().Contains(SearchBox.Text.ToLower())).ToList();
                ListProduct.ItemsSource = filteredAll;
            }
            else
            {
                var filtered = MuzScrapBdContext.GetInstance().Wishlists
                    .Include(x => x.Product)
                    .Where(x => x.ProductId == x.Product.Id && x.UserId == _userID.UserID && x.Product.ProductType == ComboBoxView.SelectedItem)
                    .ToList()
                    .Where(x => x.Product.Title.ToLower().Contains(SearchBox.Text.ToLower())
                                | x.Product.Price.ToLower().Contains(SearchBox.Text.ToLower())
                                | x.Product.ProductType.ToLower().Contains(SearchBox.Text.ToLower())).ToList();
                ListProduct.ItemsSource = filtered;
            }
        }
        private void Button_DelWishlist_Click(object sender, RoutedEventArgs e)
        {
            if (ListProduct.SelectedItem is Wishlist product)
            {
                using MuzScrapDbContext db = new MuzScrapDbContext();

                string index = "Удалено из избранного!";
                MessageBox.Show(index);

                db.Wishlists.Remove(product);
                db.SaveChanges();
                Load_Wishlist();
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
                    productCard.Title = selectedProduct.Title;
                    productCard.Brand = selectedProduct.Brand;
                    productCard.ProductType = selectedProduct.ProductType;

                    productCard.Price = product.Price.Trim();
                    productCard.Source = product.Source;
                    productCard.Store = product.Store;
                }
            }
            foreach (var product in db.Products.Where(x => x.Store == "https://jazz-shop.ru"))
            {
                if (product.Title.ToLower() == selectedProduct.Title.ToLower())
                {
                    productCard.Title = selectedProduct.Title;
                    productCard.Brand = selectedProduct.Brand;
                    productCard.ProductType = selectedProduct.ProductType;

                    productCard.Price2 = product.Price.Trim();
                    productCard.Source2 = product.Source;
                    productCard.Store2 = product.Store;
                }
            }
            ProductMore productMore = new ProductMore(productCard);
            productMore.ShowDialog();
        }

        private void ComboBoxView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectLabel = ComboBoxView.SelectedValue;
            ListProduct.ItemsSource = MuzScrapBdContext.GetInstance().Wishlists
                                                        .Include(x => x.Product)
                                                        .Where(x => x.ProductId == x.Product.Id && x.UserId == _userID.UserID && x.Product.ProductType == selectLabel)
                                                        .ToList();
            if (selectLabel == "Все")
            {
                Load_Wishlist();
            }
        }
    }
}
