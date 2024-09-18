namespace API.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> OnlineUsers = [];

        // Phương thức này được gọi khi một người dùng kết nối.
        public Task<bool> UserConnected(string username, string connectionId)
        {
            var isOnline = false;

            // lock Dictionary object để đảm bảo chỉ có một luồng (thread) có thể truy cập vào khối mã này cùng lúc.
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(username))
                {
                    OnlineUsers[username].Add(connectionId);
                }
                else
                {
                    //  thêm một mục mới vào Dictionary với key là tên người dùng và value là danh sách chứa ID kết nối.
                    OnlineUsers.Add(username, [connectionId]);
                    isOnline = true;
                }
            }

            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string username, string connectionId)
        {
            var isOffline = false;

            lock (OnlineUsers)
            {
                // Nếu dictionary không chứa usename này thì trả về offline = false (nghĩa là online)
                if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

                OnlineUsers[username].Remove(connectionId);

                // Nếu danh sách IDs kết nối trống, xóa nguời dùng khỏi dictionary
                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                    isOffline = true; 
                }
            }

            return Task.FromResult(isOffline);
        }

        // Lấy danh sách người dùng trực tuyến
        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;

            lock (OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }

        // kết nối với một người dùng được chỉ định
        public static Task<List<string>> GetConnectionsForUser(string username)
        {
            // Tạo một bản sao của danh sách kết nối để tránh các vấn đề khi sửa đổi đồng thời.
            List<string> connectionIds; 

            if (OnlineUsers.TryGetValue(username, out var connections)) // lấy d/s kết nối với người dùng chỉ định
            {
                lock (connections)
                {
                    connectionIds = connections.ToList();
                }
            } 
            else
            {
                connectionIds = [];
            }

            return Task.FromResult(connectionIds);
        }
    }
}
