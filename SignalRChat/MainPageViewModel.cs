using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xamarin.Forms;

namespace SignalRChat
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private HubConnection _connection;

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged();
                SendCommand.ChangeCanExecute();
            }
        }

        private bool _isConnected;
        private readonly IConfiguration _configuration;

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Message> Messages { get; set; }

        public Command ConnectCommand { get; }
        public Command SendCommand { get; }

        public MainPageViewModel(IConfiguration configuration)
        {
            _configuration = configuration;

            Messages = new ObservableCollection<Message>();

            ConnectCommand = new Command(ConnectExecute, param => !IsConnected);
            SendCommand = new Command(SendExecute, param => !string.IsNullOrEmpty(Message));
        }

        private async void SendExecute(object param)
        {
            var from = RuntimeInformation.OSDescription;

            try
            {
                await _connection.SendAsync("Send", from, Message);
                AppendMessage(from, Message);

                Message = "";
            }
            catch (Exception ex)
            {
                AppendMessage($"An error occurred while sending: {ex}");
            }
        }

        private async void ConnectExecute(object param)
        {
            try
            {
                var charHubUrl = _configuration["ChatHubUrl"];

                _connection = new HubConnectionBuilder()
                    .WithUrl(charHubUrl)
                    .Build();

                _connection.On<string, string>("broadcastMessage", (from, message) => AppendMessage(from, message));

                await _connection.StartAsync();

                AppendMessage("Connected.");
                IsConnected = true;
            }
            catch (Exception ex)
            {
                AppendMessage($"An error occurred while connecting: {ex}");
                return;
            }
        }

        private void AppendMessage(string message)
        {
            AppendMessage("-", message);
        }

        private void AppendMessage(string from, string message)
        {
            Messages.Add(new Message { From = from, Content = message });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
