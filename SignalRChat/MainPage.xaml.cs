using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Xamarin.Forms;

namespace SignalRChat
{
    public partial class MainPage : ContentPage
    {
        private const string SIGNALR_CHAT_SERVER_URL = "https://signalrchatweb.azurewebsites.net/chat";
        private HubConnection _connection;

        public ObservableCollection<Message> Messages { get; set; }

        public MainPage()
        {
            InitializeComponent();

            Messages = new ObservableCollection<Message>();
            BindingContext = this;
        }

        private void AppendMessage(string message)
        {
            AppendMessage(new Message { From = "-", Content = message });
        }

        private void AppendMessage(Message message)
        {
            Messages.Add(message);
            Device.BeginInvokeOnMainThread(() =>
            {
                MessagesListView.ScrollTo(message, ScrollToPosition.End, animated: true);
            });
        }

        private async void SendButton_Clicked(object sender, EventArgs e)
        {
            var from = RuntimeInformation.OSDescription;
            var message = MessageEntry.Text;

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            try
            {
                await _connection.SendAsync("Send", from, message);
                MessageEntry.Text = "";

                AppendMessage(new Message { From = from, Content = message });
            }
            catch (Exception ex)
            {
                AppendMessage($"An error occurred while sending: {ex}");
            }
        }

        private async void ConnectButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                _connection = new HubConnectionBuilder()
                    .WithUrl(SIGNALR_CHAT_SERVER_URL)
                    .Build();

                _connection.On<string, string>("broadcastMessage", (name, message) =>
                {
                    AppendMessage(new Message { From = name, Content = message });
                });

                await _connection.StartAsync();

                AppendMessage("Connected.");
                ConnectButton.IsVisible = false;
                MessageEntryGrid.IsVisible = true;
            }
            catch (Exception ex)
            {
                AppendMessage($"An error occurred while connecting: {ex}");
                return;
            }
        }

        bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
