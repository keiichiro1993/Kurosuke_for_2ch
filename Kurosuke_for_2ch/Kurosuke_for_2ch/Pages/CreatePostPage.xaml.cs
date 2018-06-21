using Kurosuke_for_2ch.Models;
using Kurosuke_for_2ch.Utils;
using Kurosuke_for_2ch.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace Kurosuke_for_2ch.Pages
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class CreatePostPage : Page
    {
        public CreatePostPage()
        {
            this.InitializeComponent();
        }

        public CreatePostPageViewModel viewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var thread = (Thread)e.Parameter;
            viewModel = new CreatePostPageViewModel(thread);
            MainGrid.DataContext = viewModel;
        }

        private void BackButtonClicked(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private async void SendButtonClicked(object sender, RoutedEventArgs e)
        {
            var client = new My2chClient();
            try
            {
                var res = await client.PostUpdate(viewModel.Thread, viewModel.name, viewModel.mail, viewModel.message);

                if (res.IsSuccessStatusCode)
                {
                    if (Frame.CanGoBack)
                    {
                        Frame.GoBack();
                    }
                }
                else
                {
                    var dialog = new MessageDialog(res.ReasonPhrase, "書き込みに失敗しました。");
                    dialog.Commands.Add(new UICommand("リトライ"));
                    dialog.Commands.Add(new UICommand("Close"));
                    var command = await dialog.ShowAsync();
                    if (command.Label == "リトライ")
                    {
                        SendButtonClicked(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog(ex.Message, "書き込みに失敗しました。");
                dialog.Commands.Add(new UICommand("リトライ"));
                dialog.Commands.Add(new UICommand("Close"));
                var command = await dialog.ShowAsync();
                if (command.Label == "リトライ")
                {
                    SendButtonClicked(sender, e);
                }
            }
        }
    }
}
