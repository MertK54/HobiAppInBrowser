using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs
{
    public class VideoChatHub : Hub
    {
        // Kullanıcı bağlandığında
        public override async Task OnConnectedAsync()
        {
            await Clients.Others.SendAsync("UserConnected", Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        // Kullanıcı ayrıldığında
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.Others.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        // Arama başlatma
        public async Task CallUser(string targetUserId)
        {
            await Clients.Client(targetUserId).SendAsync("IncomingCall", Context.ConnectionId);
        }

        // Aramayı kabul etme
        public async Task AnswerCall(string callerId)
        {
            await Clients.Client(callerId).SendAsync("CallAnswered", Context.ConnectionId);
        }

        // WebRTC Offer gönderme
        public async Task SendOffer(string targetUserId, string offer)
        {
            Console.WriteLine($"[SendOffer] {Context.ConnectionId} -> {targetUserId}");
            Console.WriteLine($"[SendOffer] Offer uzunluğu: {offer?.Length ?? 0} karakter");
            await Clients.Client(targetUserId).SendAsync("ReceiveOffer", Context.ConnectionId, offer);
        }

        // WebRTC Answer gönderme
        public async Task SendAnswer(string targetUserId, string answer)
        {
            Console.WriteLine($"[SendAnswer] {Context.ConnectionId} -> {targetUserId}");
            Console.WriteLine($"[SendAnswer] Answer uzunluğu: {answer?.Length ?? 0} karakter");
            await Clients.Client(targetUserId).SendAsync("ReceiveAnswer", Context.ConnectionId, answer);
        }

        // ICE Candidate gönderme
        public async Task SendIceCandidate(string targetUserId, string candidate)
        {
            await Clients.Client(targetUserId).SendAsync("ReceiveIceCandidate", Context.ConnectionId, candidate);
        }

        // Aramayı sonlandırma
        public async Task EndCall(string targetUserId)
        {
            await Clients.Client(targetUserId).SendAsync("CallEnded", Context.ConnectionId);
        }
    }
}