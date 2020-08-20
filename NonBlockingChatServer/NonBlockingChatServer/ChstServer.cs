using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

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
        Socket clientSocket = null;
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

        private void OnSend(object sender, EventArgs e)
        {
            SendMessage();
        }
        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (sender == inputPort)
                {
                    StartServer();
                }
                else if (sender == inputMsg)
                {
                    SendMessage();
                }
            }
            else if (sender == inputPort)
            {
                IntegerFiltering(sender, e);
            }
        }
        private void StartServer()
        {
            if (serverSocket != null)
            {
                MessageBox.Show("서버가 이미 실행중입니다!");
                return;
            }
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
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
                IPEndPoint endPoint = new IPEndPoint(thisAddress, port);
                serverSocket.Bind(endPoint);
                serverSocket.Listen(10);
                AppendText(outputMsg, "서버가 생성되었습니다.");
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
                AppendText(outputMsg, "클라이언트 대기중...");
            }
            catch (Exception exception)
            {
                MessageBox.Show("서버 생성 중 문제가 발생하였습니다. : " + exception.Message);
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
                clientSocket = client;
                AppendText(outputMsg, string.Format("{0}가 연결되었습니다.", clientSocket.RemoteEndPoint));
                client.BeginReceive(asyncObject.Buffer, 0, asyncObject.Buffer.Length, SocketFlags.None, ReceiveHandler, asyncObject);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
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
                    AppendText(outputMsg, string.Format("받음: {0}", Encoding.Unicode.GetString(msgByte)));
                }
                asyncObject.ClearBuffer();
                asyncObject.WorkingSocket.BeginReceive(asyncObject.Buffer, 0, asyncObject.Buffer.Length, SocketFlags.None, ReceiveHandler, asyncObject);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }
        }
        private void SendMessage()
        {
            AsyncObject asyncObject = new AsyncObject(1);
            asyncObject.Buffer = Encoding.Unicode.GetBytes(inputMsg.Text);
            asyncObject.WorkingSocket = clientSocket;
            try
            {
                clientSocket.BeginSend(asyncObject.Buffer, 0, asyncObject.Buffer.Length, SocketFlags.None, SendHandler, asyncObject);
            }
            catch (Exception exception)
            {
                MessageBox.Show("전송에 실패하였습니다. : {0}", exception.Message);
            }
        }
        void SendHandler(IAsyncResult ar)
        {
            AsyncObject asyncObject = (AsyncObject) ar.AsyncState;
            int sentBytes;
            try
            {
                sentBytes = asyncObject.WorkingSocket.EndSend(ar);
            }
            catch (Exception exception)
            {
                AppendText(outputMsg, string.Format("메세지 전송에 실패하였습니다. : ({0})", exception.Message));
                return;
            }

            if (sentBytes > 0)
            {
                Byte[] msgByte = new Byte[sentBytes];
                Array.Copy(asyncObject.Buffer, msgByte, sentBytes);

                AppendText(outputMsg, string.Format("보냄: {0}", Encoding.Unicode.GetString(msgByte)));
                inputMsg.Clear();
            }
        }
    }
}
