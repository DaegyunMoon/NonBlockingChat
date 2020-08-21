using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace NonBlockingChatClient
{
    public partial class ChatClient : Form
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

        public ChatClient()
        {
            InitializeComponent();
        }
        void AppendText(Control ctrl, string s)
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
        private void OnConnect(object sender, EventArgs e)
        {
            ConnectToServer();
        }
        private void OnSend(object sender, EventArgs e)
        {
            SendMessage();
        }
        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (sender == inputAddr)
                {
                    SendKeys.Send("{TAB}");
                }
                else if (sender == inputPort)
                {
                    ConnectToServer();
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
        private void ConnectToServer()
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            try
            {
                int port;
                if (!int.TryParse(inputPort.Text, out port))
                {
                    MessageBox.Show("포트번호를 확인해주세요.");
                    inputPort.Focus();
                    inputPort.SelectAll();
                    return;
                }
                clientSocket.BeginConnect(inputAddr.Text, port, ConnectCallback, clientSocket);
                AppendText(outputMsg, string.Format("{0}에 연결중...", inputAddr.Text));
            }
            catch (Exception exception)
            {
                MessageBox.Show("연결에 실패하였습니다.\n" + exception.Message);
                return;
            }
        }
        void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                AsyncObject asyncObject = new AsyncObject(4096);
                asyncObject.WorkingSocket = client;
                clientSocket.BeginReceive(asyncObject.Buffer, 0, asyncObject.Buffer.Length, SocketFlags.None, ReceiveHandler, asyncObject);
                AppendText(outputMsg, string.Format("{0}에 연결되었습니다.", inputAddr.Text.ToString()));
            }
            catch (Exception exception)
            {
                MessageBox.Show("Connect 중 문제가 발생하였습니다.\n" + exception.Message);
            }
        }
        void ReceiveHandler(IAsyncResult ar)
        {
            AsyncObject asyncObject = (AsyncObject)ar.AsyncState;
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
                MessageBox.Show("Receive 중 문제가 발생하였습니다.\n" + exception.Message);
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
                MessageBox.Show("전송에 실패하였습니다.\n" + exception.Message);
            }
        }
        void SendHandler(IAsyncResult ar)
        {
            AsyncObject asyncObject = (AsyncObject)ar.AsyncState;
            int sentBytes;
            try
            {
                sentBytes = asyncObject.WorkingSocket.EndSend(ar);
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("메세지 전송에 실패하였습니다.\n" + exception.Message));
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
