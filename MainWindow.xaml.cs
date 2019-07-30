using System.Windows;
using System.Collections.Generic;
using System.Windows.Input;
using Google.Apis.Services;
using Google.Apis.Customsearch.v1;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string searchInput;
        List<string> imageSources = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }
        private void UnfocusSearchbar(object sender, MouseButtonEventArgs e)
        {
            grid.Focus();
        }
        private void NoResults()
        {
            noResults.Visibility = Visibility.Visible;
        }
        private void HideNoResults()
        {
            noResults.Visibility = Visibility.Collapsed;
        }
        private void GetInput(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return){
                searchInput = searchBox.Text;
                SearchGoogle();
            }
        }
        private void SearchGoogle()
        {
            HideNoResults();
            string apiKey = "AIzaSyBdOQTWftGb-DFF_Y-uJCcy8wgRLLmnx3E";
            string cx = "015695191635768647286:alyjydblwfs";
            var svc = new Google.Apis.Customsearch.v1.CustomsearchService(new BaseClientService.Initializer { ApiKey = apiKey });
            var listRequest = svc.Cse.List(searchInput);
            // ListRequest num = 10 - default(Range 1-10).
            listRequest.Cx = cx;
            listRequest.SearchType = CseResource.ListRequest.SearchTypeEnum.Image;
            var search = listRequest.Execute();
            if(search != null && search.Items != null)
            {
                foreach(var result in search.Items)
                {
                    if(result.Link != null && result.Link != "")
                    {
                        imageSources.Add(result.Link);
                    }
                }
                LoadImages();
            }else{
                NoResults();
            }
        }
        private void LoadImages()
        {
            ClearImages();
            int gridX = 0;
            int gridY = 1;
            for(var i = 0; i < imageSources.Count; i++)
            {
                // Donwload image
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imageSources[i]);
                bitmap.EndInit();
                // Create image with downloaded image
                var img = new Image();
                img.Source = bitmap;
                // Set image in Grid
                if(gridX <= 3)
                {
                    Grid.SetColumn(img, gridX);
                    Grid.SetRow(img, gridY);
                    grid.Children.Add(img);
                }
                if(gridX == 3){
                    gridY++;
                    gridX = 0;
                }else{
                    gridX++;
                }
            }
            imageSources.Clear();
        }
        private void ClearImages()
        {
            int intTotalChildren = grid.Children.Count-1;
            for (int intCounter = intTotalChildren; intCounter >= 0; intCounter--)
            {
                if(grid.Children[intCounter].GetType() == typeof(Image))
                {
                    Image ucCurrentChild = (Image)grid.Children[intCounter];
                    grid.Children.Remove(ucCurrentChild);
                }
            }
        }
    }
}
