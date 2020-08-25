using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

namespace NonBlockingChatServer
{
    public partial class ChatServer : Form
    {
        public class AsyncObject
        {
            public Byte[] Buffer;
            public Socket WorkingSocket;
            public readonly int BufferSize;
            public AsyncObject(int bufferSize)
            {
                BufferSize = bufferSize;
                Buffer = new byte[BufferSize];
            }
            public void ClearBuffer()
            {
                Array.Clear(Buffer, 0, BufferSize);
            }
        }
        List<Socket> clients = new List<Socket>();
        Socket serverSocket = null;
        IPAddress thisAddress;
        public ChatServer()
        {
            InitializeComponent();
        }
        public void AppendText(Control ctrl, string s)
        {
            if (ctrl.InvokeRequired)
            {
                ctrl.Invoke(new MethodInvoker(delegate ()
                {
                    string source = ctrl.Text;
                    ctrl.Text = source + s + Environment.NewLine;
                }));
            }
            else
            {
                string source = ctrl.Text;
                ctrl.Text = source + s + Environment.NewLine;
            }
        }

        private void IntegerFiltering(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsControl(e.KeyChar) || char.IsDigit(e.KeyChar));
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress addr in hostEntry.AddressList)
                {
                    if (addr.AddressFamily == AddressFamily.InterNetwork)
                    {
                        thisAddress = addr;
                        break;
                    }
                }
                if (thisAddress == null)
                {
                    thisAddress = IPAddress.Loopback;
                }
                inputAddr.Text = thisAddress.ToString();
            } 
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void OnStart(object sender, EventArgs e)
        {
            StartServer();
        }
        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if(sender == inputPort)
            {
                if (e.KeyChar == (char)13)
                {
                    StartServer();
                }
                else
                {
                    IntegerFiltering(sender, e);
                }
            }
        }
        private void StartServer()
        {
            if (serverSocket != null)
            {
                MessageBox.Show("서버가 이미 실행중입니다!");
                return;
            }
            int port;
            if (!int.TryParse(inputPort.Text, out port))
            {
                MessageBox.Show("포트번호를 확인해주세요.");
                inputPort.Focus();
                inputPort.SelectAll();
                return;
            }
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                IPEndPoint endPoint = new IPEndPoint(thisAddress, port);
                serverSocket.Bind(endPoint);
                serverSocket.Listen(10);
                AppendText(outputMsg, "서버가 생성되었습니다.");
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
                AppendText(outputMsg, "클라이언트 대기중...");
            }
            catch (Exception exception)
            {
                MessageBox.Show("서버 생성 중 문제가 발생하였습니다.\n" + exception.Message);
                serverSocket = null;
                return;
            }
        }
        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = serverSocket.EndAccept(ar);
                AsyncObject asyncObject = new AsyncObject(4096);
                asyncObject.WorkingSocket = client;
                clients.Add(client);
                AppendText(outputMsg, string.Format("{0}와 연결되었습니다.", client.RemoteEndPoint));
                if(clients.Count < 2)
                {
                    serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
                }
                else
                {
                    AppendText(outputMsg, "모든 클라이언트가 입장하였습니다.");
                }
                client.BeginReceive(asyncObject.Buffer, 0, asyncObject.Buffer.Length, SocketFlags.None, ReceiveHandler, asyncObject);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Accept 중 문제가 발생하였습니다.\n" + exception.Message);
            }
        }
        void ReceiveHandler(IAsyncResult ar)
        {
            AsyncObject asyncObject = (AsyncObject) ar.AsyncState;
            int recvBytes = 0;
            try
            {
                recvBytes = asyncObject.WorkingSocket.EndReceive(ar);
                if (recvBytes > 0)
                {
                    Byte[] msgByte = new Byte[recvBytes];
                    Array.Copy(asyncObject.Buffer, msgByte, recvBytes);
                    AppendText(outputMsg, string.Format("{0} : {1}", asyncObject.WorkingSocket.RemoteEndPoint, Encoding.Unicode.GetString(msgByte)));
                    foreach(Socket socket in clients)
                    {
                        if(socket != asyncObject.WorkingSocket)
                        {
                            socket.Send(msgByte);
                        }
                    }
                }
                asyncObject.ClearBuffer();
                asyncObject.WorkingSocket.BeginReceive(asyncObject.Buffer, 0, asyncObject.Buffer.Length, SocketFlags.None, ReceiveHandler, asyncObject);
            }
            catch (SocketException exception)
            {
                if (exception.ErrorCode == 10054)
                {
                    AppendText(outputMsg, string.Format("{0}와의 연결이 끊겼습니다.", asyncObject.WorkingSocket.RemoteEndPoint));
                    foreach (Socket client in clients)
                    {
                        if (client == asyncObject.WorkingSocket)
                        {
                            clients.Remove(client);
                            break;
                        }
                    }
                    serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
                    AppendText(outputMsg, "다른 클라이언트를 기다립니다.");
                }
                else
                {
                    AppendText(outputMsg, string.Format("수신에 실패하였습니다.\n{0}", exception.Message));
                    return;
                }
            }
        }
    }
}