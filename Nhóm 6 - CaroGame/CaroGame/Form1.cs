using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace CaroGame
{
    public partial class Form1 : Form
    {
        #region Properties
        ChessBoardManager ChessBoard;
        SocketManager socket;
        #endregion

        public Form1()
        {

            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            ChessBoard = new ChessBoardManager(panelChessBoard, textBoxPlayerName, pictureBoxPlayerMark);
            ChessBoard.EndedGame += ChessBoard_EndedGame;
            ChessBoard.PlayerMarked += ChessBoard_PlayerMarked;

            progressBar.Maximum = Const.coolDownTime;
            progressBar.Step = Const.coolDownStep;
            progressBar.Value = 0;

            timerCoolDown.Interval = Const.coolDownInterval;

            socket = new SocketManager();

            newGame();
        }

        #region Methods
        private void ChessBoard_PlayerMarked(object sender, ButtonClickEvent e)
        {
            timerCoolDown.Start();
            panelChessBoard.Enabled = false;
            progressBar.Value = 0;

            socket.Send(new SocketData((int)SocketCommand.SEND_POINT, "", e.ClickedPoint));

            undoToolStripMenuItem.Enabled = false;
            Listen();
        }

        void EndGame()
        {
            timerCoolDown.Stop();
            panelChessBoard.Enabled = false;//stop match
            undoToolStripMenuItem.Enabled = false;
        }

        void newGame()
        {
            progressBar.Value = 0;
            timerCoolDown.Stop();
            undoToolStripMenuItem.Enabled = true;
            ChessBoard.drawChessBoard();
        }

        void Quit()
        {
            if (MessageBox.Show("Do you want to quit?", "Notification", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                Application.Exit();
        }

        void Undo()
        {
            ChessBoard.Undo();
            progressBar.Value = 0;
        }

        private void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame();
            socket.Send(new SocketData((int)SocketCommand.END_GAME, "", new Point()));
        }

        private void timerCoolDown_Tick(object sender, EventArgs e)
        {
            progressBar.PerformStep();
            if (progressBar.Value >= Const.coolDownTime)
            {
                EndGame();
                socket.Send(new SocketData((int)SocketCommand.TIME_OUT, "", new Point()));
            }
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newGame();
            socket.Send(new SocketData((int)SocketCommand.NEW_GAME, "", new Point()));
            panelChessBoard.Enabled = true;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Do you want to quit?", "Notification", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                e.Cancel = true;
            else
            {
                try
                {
                    socket.Send(new SocketData((int)SocketCommand.QUIT, "", new Point()));
                }
                catch { }
            }
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            socket.IP = textBoxIP.Text;

            if (!socket.connectServer())
            {
                socket.isServer = true;
                panelChessBoard.Enabled = true;
                socket.createServer();
            }
            else
            {
                socket.isServer = false;
                panelChessBoard.Enabled = false;
                Listen();
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            textBoxIP.Text = socket.getLocalIpV4(System.Net.NetworkInformation.NetworkInterfaceType.Wireless80211);

            if (string.IsNullOrEmpty(textBoxIP.Text))
            {
                textBoxIP.Text = socket.getLocalIpV4(System.Net.NetworkInformation.NetworkInterfaceType.Ethernet);
            }

        }

        void Listen()
        {

            Thread listenThread = new Thread(() =>
             {
                 try
                 {
                     SocketData data = (SocketData)socket.Receive();
                     ProcessData(data);
                 }
                 catch
                 {
                 }
             });
            listenThread.IsBackground = true;
            listenThread.Start();

        }

        private void ProcessData(SocketData data)
        {
            switch (data.Command)
            {
                case (int)SocketCommand.NOTIFY:
                    MessageBox.Show(data.Message);
                    break;
                case (int)SocketCommand.NEW_GAME:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        newGame();
                        panelChessBoard.Enabled = false;
                    }));
                    break;
                case (int)SocketCommand.UNDO:
                    Undo();
                    progressBar.Value = 0;
                    break;
                case (int)SocketCommand.SEND_POINT:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        progressBar.Value = 0;
                        panelChessBoard.Enabled = true;
                        timerCoolDown.Start();
                        ChessBoard.OtherPlayerMark(data.Point);
                        undoToolStripMenuItem.Enabled = true;
                    }));
                    break;
                case (int)SocketCommand.END_GAME:
                    MessageBox.Show("5 in a line!");
                    break;
                case (int)SocketCommand.TIME_OUT:
                    MessageBox.Show("Time out!");
                    break;
                case (int)SocketCommand.QUIT:
                    timerCoolDown.Stop();
                    MessageBox.Show("Player is out!");
                    break;
                default:
                    break;
            }
            Listen();
        }
    }
}
