using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatSocketApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new XtraForm1());
        }
    }

    /// <summary>
    /// Geliştirilmiş Chat Sunucusu
    /// - Thread-safe client yönetimi
    /// - Proper async patterns
    /// - Typing indicator desteği
    /// - Private message desteği
    /// </summary>
    public class ChatServer
    {
        private Socket serverSocket;
        private readonly Dictionary<Socket, ClientInfo> clients = new Dictionary<Socket, ClientInfo>();
        private readonly object lockObj = new object();
        
        public event Action<string> OnLog;
        public event Action<int> OnClientCountChanged;

        private class ClientInfo
        {
            public string Username { get; set; }
            public DateTime ConnectedAt { get; set; }
            public DateTime LastActivity { get; set; }
        }

        public void StartServer(int port, CancellationToken token)
        {
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                serverSocket.Listen(100);

                Log($"Sunucu başlatıldı - Port: {port}");

                while (!token.IsCancellationRequested)
                {
                    if (serverSocket.Poll(100000, SelectMode.SelectRead))
                    {
                        try
                        {
                            Socket clientSocket = serverSocket.Accept();
                            Log($"Yeni bağlantı: {clientSocket.RemoteEndPoint}");
                            
                            lock (lockObj)
                            {
                                clients[clientSocket] = new ClientInfo
                                {
                                    ConnectedAt = DateTime.Now,
                                    LastActivity = DateTime.Now
                                };
                            }
                            
                            OnClientCountChanged?.Invoke(clients.Count);
                            Task.Run(() => HandleClient(clientSocket, token));
                        }
                        catch (SocketException ex)
                        {
                            Log($"Accept hatası: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Sunucu hatası: {ex.Message}");
                throw;
            }
            finally
            {
                CleanupServer();
            }
        }

        private void HandleClient(Socket socket, CancellationToken token)
        {
            byte[] buffer = new byte[8192];
            string clientName = null;

            try
            {
                while (!token.IsCancellationRequested && socket.Connected)
                {
                    if (socket.Poll(100000, SelectMode.SelectRead))
                    {
                        int bytesRead;
                        try
                        {
                            bytesRead = socket.Receive(buffer);
                        }
                        catch
                        {
                            break;
                        }

                        if (bytesRead > 0)
                        {
                            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            
                            lock (lockObj)
                            {
                                if (clients.ContainsKey(socket))
                                    clients[socket].LastActivity = DateTime.Now;
                            }

                            // Mesaj tipini kontrol et
                            if (message.StartsWith("CONNECT:"))
                            {
                                clientName = message.Substring(8).Trim();
                                lock (lockObj)
                                {
                                    if (clients.ContainsKey(socket))
                                        clients[socket].Username = clientName;
                                }
                                Log($"{clientName} bağlandı");
                                
                                // Kullanıcı listesini gönder
                                SendUserList();
                                BroadcastMessage(message, socket);
                            }
                            else if (message.StartsWith("TYPING:"))
                            {
                                // Typing indicator - sadece diğerlerine broadcast et
                                BroadcastMessage(message, socket);
                            }
                            else if (message.StartsWith("PRIVATE:"))
                            {
                                // Private message format: PRIVATE:target:sender:message
                                HandlePrivateMessage(message, socket);
                            }
                            else
                            {
                                BroadcastMessage(message, socket);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Client handler hatası: {ex.Message}");
            }
            finally
            {
                RemoveClient(socket, clientName);
            }
        }

        private void HandlePrivateMessage(string message, Socket sender)
        {
            try
            {
                // PRIVATE:targetUser:senderUser:actualMessage
                var parts = message.Split(new[] { ':' }, 4);
                if (parts.Length >= 4)
                {
                    string targetUser = parts[1];
                    string senderUser = parts[2];
                    string actualMessage = parts[3];

                    Socket targetSocket = null;
                    lock (lockObj)
                    {
                        foreach (var kvp in clients)
                        {
                            if (kvp.Value.Username == targetUser)
                            {
                                targetSocket = kvp.Key;
                                break;
                            }
                        }
                    }

                    if (targetSocket != null)
                    {
                        try
                        {
                            byte[] data = Encoding.UTF8.GetBytes($"PRIVATE:{senderUser}:{actualMessage}");
                            targetSocket.Send(data);
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        private void SendUserList()
        {
            List<string> usernames = new List<string>();
            lock (lockObj)
            {
                foreach (var client in clients.Values)
                {
                    if (!string.IsNullOrEmpty(client.Username))
                        usernames.Add(client.Username);
                }
            }

            string userListMsg = "USERLIST:" + string.Join(",", usernames);
            byte[] data = Encoding.UTF8.GetBytes(userListMsg);

            Socket[] clientsCopy;
            lock (lockObj)
            {
                clientsCopy = new Socket[clients.Count];
                clients.Keys.CopyTo(clientsCopy, 0);
            }

            foreach (var client in clientsCopy)
            {
                try
                {
                    client.Send(data);
                }
                catch { }
            }
        }

        private void RemoveClient(Socket socket, string clientName)
        {
            lock (lockObj)
            {
                if (clients.ContainsKey(socket))
                    clients.Remove(socket);
            }

            try { socket.Shutdown(SocketShutdown.Both); } catch { }
            try { socket.Close(); } catch { }

            OnClientCountChanged?.Invoke(clients.Count);

            if (!string.IsNullOrEmpty(clientName))
            {
                Log($"{clientName} ayrıldı");
                BroadcastMessage($"DISCONNECT:{clientName}", null);
                SendUserList();
            }
        }

        private void BroadcastMessage(string message, Socket sender)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            Socket[] clientsCopy;
            lock (lockObj)
            {
                clientsCopy = new Socket[clients.Count];
                clients.Keys.CopyTo(clientsCopy, 0);
            }

            foreach (var client in clientsCopy)
            {
                if (sender != null && client == sender)
                    continue;

                try
                {
                    client.Send(data);
                }
                catch
                {
                    lock (lockObj)
                    {
                        if (clients.ContainsKey(client))
                        {
                            var info = clients[client];
                            clients.Remove(client);
                            if (!string.IsNullOrEmpty(info?.Username))
                            {
                                BroadcastMessage($"DISCONNECT:{info.Username}", null);
                            }
                        }
                    }
                    try { client.Close(); } catch { }
                }
            }
        }

        private void CleanupServer()
        {
            Log("Sunucu kapatılıyor...");
            
            Socket[] clientsCopy;
            lock (lockObj)
            {
                clientsCopy = new Socket[clients.Count];
                clients.Keys.CopyTo(clientsCopy, 0);
                clients.Clear();
            }

            foreach (var client in clientsCopy)
            {
                try { client.Shutdown(SocketShutdown.Both); } catch { }
                try { client.Close(); } catch { }
            }

            try { serverSocket?.Close(); } catch { }
            
            OnClientCountChanged?.Invoke(0);
            Log("Sunucu kapatıldı");
        }

        private void Log(string message)
        {
            OnLog?.Invoke($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        public int GetClientCount()
        {
            lock (lockObj)
            {
                return clients.Count;
            }
        }
    }
}
