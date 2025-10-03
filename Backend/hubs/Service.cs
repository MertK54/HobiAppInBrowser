using Microsoft.AspNetCore.SignalR.Client;
using System.Text.Json;

namespace Backend.Hubs
{
    public class WebRtcService
    {
        private HubConnection _hubConnection;
        private string _currentUserId;
        
        // STUN sunucuları (NAT traversal için)
        private readonly string[] _iceServers = new[]
        {
            "stun:stun.l.google.com:19302",
            "stun:stun1.l.google.com:19302"
        };

        public event Action<string> OnIncomingCall;
        public event Action<string> OnCallAnswered;
        public event Action<string, string> OnOfferReceived;
        public event Action<string, string> OnAnswerReceived;
        public event Action<string, string> OnIceCandidateReceived;
        public event Action<string> OnCallEnded;
        public event Action<string> OnUserConnected;
        public event Action<string> OnUserDisconnected;

        public async Task ConnectAsync(string serverUrl)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(serverUrl)
                .WithAutomaticReconnect()
                .Build();

            // Event handler'ları kaydet
            RegisterHandlers();

            await _hubConnection.StartAsync();
            _currentUserId = _hubConnection.ConnectionId;
        }

        private void RegisterHandlers()
        {
            _hubConnection.On<string>("UserConnected", userId =>
            {
                OnUserConnected?.Invoke(userId);
            });

            _hubConnection.On<string>("UserDisconnected", userId =>
            {
                OnUserDisconnected?.Invoke(userId);
            });

            _hubConnection.On<string>("IncomingCall", callerId =>
            {
                OnIncomingCall?.Invoke(callerId);
            });

            _hubConnection.On<string>("CallAnswered", userId =>
            {
                OnCallAnswered?.Invoke(userId);
            });

            _hubConnection.On<string, string>("ReceiveOffer", (userId, offer) =>
            {
                OnOfferReceived?.Invoke(userId, offer);
            });

            _hubConnection.On<string, string>("ReceiveAnswer", (userId, answer) =>
            {
                OnAnswerReceived?.Invoke(userId, answer);
            });

            _hubConnection.On<string, string>("ReceiveIceCandidate", (userId, candidate) =>
            {
                OnIceCandidateReceived?.Invoke(userId, candidate);
            });

            _hubConnection.On<string>("CallEnded", userId =>
            {
                OnCallEnded?.Invoke(userId);
            });
        }

        public async Task CallUserAsync(string targetUserId)
        {
            await _hubConnection.InvokeAsync("CallUser", targetUserId);
        }

        public async Task AnswerCallAsync(string callerId)
        {
            await _hubConnection.InvokeAsync("AnswerCall", callerId);
        }

        public async Task SendOfferAsync(string targetUserId, string offer)
        {
            await _hubConnection.InvokeAsync("SendOffer", targetUserId, offer);
        }

        public async Task SendAnswerAsync(string targetUserId, string answer)
        {
            await _hubConnection.InvokeAsync("SendAnswer", targetUserId, answer);
        }

        public async Task SendIceCandidateAsync(string targetUserId, string candidate)
        {
            await _hubConnection.InvokeAsync("SendIceCandidate", targetUserId, candidate);
        }

        public async Task EndCallAsync(string targetUserId)
        {
            await _hubConnection.InvokeAsync("EndCall", targetUserId);
        }

        public string GetCurrentUserId() => _currentUserId;

        public async Task DisconnectAsync()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }
        }
    }
}