using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CaroGame
{
    public class ChessBoardManager
    {

        #region Properties
        private Panel chessBoard;
        public Panel ChessBoard
        {
            get { return chessBoard; }
            set { chessBoard = value; }
        }

        public List<Player> Player
        {
            get
            {
                return player;
            }

            set
            {
                player = value;
            }
        }

        public int CurrentPlayer
        {
            get
            {
                return currentPlayer;
            }

            set
            {
                currentPlayer = value;
            }
        }

        public TextBox PlayerName
        {
            get
            {
                return playerName;
            }

            set
            {
                playerName = value;
            }
        }

        public PictureBox PlayerMark
        {
            get
            {
                return playerMark;
            }

            set
            {
                playerMark = value;
            }
        }

        public List<List<Button>> Matrix
        {
            get
            {
                return matrix;
            }

            set
            {
                matrix = value;
            }
        }

        public Stack<PlayInfo> PlayTimeline
        {
            get
            {
                return playTimeline;
            }

            set
            {
                playTimeline = value;
            }
        }

        private List<Player> player;
        private int currentPlayer = 0;
        private TextBox playerName;
        private PictureBox playerMark;
        private List<List<Button>> matrix;

        private event EventHandler <ButtonClickEvent> playerMarked;
        public event EventHandler<ButtonClickEvent> PlayerMarked
        {
            add
            {
                playerMarked += value;
            }
            remove
            {
                playerMarked -= value;
            }
        }

        private event EventHandler endedGame;
        public event EventHandler EndedGame
        {
            add
            {
                endedGame += value;
            }
            remove
            {
                endedGame -= value;
            }
        }

        private Stack<PlayInfo> playTimeline;

        #endregion

        #region Initialize
        public ChessBoardManager(Panel chessBoard, TextBox playerName, PictureBox playerMark)
        {

            this.ChessBoard = chessBoard;
            this.PlayerName = playerName;
            this.PlayerMark = playerMark;
            this.Player = new List<Player>()
            {
                new Player("X-Player", Image.FromFile(Application.StartupPath +"\\Resources\\X.jpg")),
                new Player("O-Player", Image.FromFile(Application.StartupPath +"\\Resources\\O.jpg"))
            };
            
        }
        #endregion

        #region Methods
        public void drawChessBoard()
        {
            ChessBoard.Enabled = true;
            ChessBoard.Controls.Clear();

            playTimeline = new Stack<PlayInfo>();

            CurrentPlayer = 0;
            changePlayer();

            Matrix = new List<List<Button>>();
            //chess board from button so that when click
            //activate colision!
            //draw by button
            Button oldButton = new Button()
            {
                Width = 0,
                Location = new Point(0, 0)
            };
            for (int i = 0; i < Const.chessBoardHeight; ++i)
            {

                Matrix.Add(new List<Button>());
                for (int j = 0; j < Const.chessBoardWidth; j++)
                {
                    Button btn = new Button()
                    {
                        Width = Const.chessWidth,
                        Height = Const.chessHeight,
                        Location = new Point(oldButton.Location.X + oldButton.Width, oldButton.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i.ToString()
                    };

                    btn.Click += Btn_Click;

                    ChessBoard.Controls.Add(btn);
                    //save button to matrix
                    Matrix[i].Add(btn);

                    //save to reuse
                    oldButton = btn;
                }
                //new location
                oldButton.Location = new Point(0, oldButton.Location.Y + Const.chessHeight);
                oldButton.Width = 0;
                oldButton.Height = 0;
            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn.BackgroundImage != null) return;
            Mark(btn);

            PlayTimeline.Push(new PlayInfo(getChessPoint(btn), CurrentPlayer));

            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;

            changePlayer();

            if (playerMarked != null)
                playerMarked(this, new ButtonClickEvent(getChessPoint(btn)));

            if (isEndGame(btn))
                endGame();
        }

        public void OtherPlayerMark(Point point)
        {
            Button btn = Matrix[point.Y][point.X];
            if (btn.BackgroundImage != null) return;

            Mark(btn);

            PlayTimeline.Push(new PlayInfo(getChessPoint(btn), CurrentPlayer));

            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;

            changePlayer();

            if (isEndGame(btn))
                endGame();
        }
        public void endGame()
        {
            if (endedGame != null)
                endedGame(this, new EventArgs());
            
        }

        public bool Undo()
        {
            if (PlayTimeline.Count <= 0)
                return false;
            bool isUndo1 = UndoAStep();
            bool isUndo2 = UndoAStep();

            PlayInfo oldPoint = playTimeline.Peek();
            CurrentPlayer = oldPoint.CurrentPlayer == 1 ? 0 : 1;

            return isUndo1 && isUndo2;
        }

        private bool UndoAStep()
        {
            if (PlayTimeline.Count <= 0)
                return false;
            PlayInfo oldPoint = playTimeline.Pop();
            Button btn = Matrix[oldPoint.Point.Y][oldPoint.Point.X];

            btn.BackgroundImage = null;

            if (PlayTimeline.Count <= 0)
                CurrentPlayer = 0;
            else
            {
                oldPoint = PlayTimeline.Peek();
            }

            changePlayer();

            return false;
        }

        private bool isEndGame(Button btn)
        {
            return isEndHorizontal(btn) || isEndVertical(btn) || isEndPrimaryDiagonal(btn) || isEndSubDiagonal(btn);
        }

        private Point getChessPoint(Button btn)
        {
           
            int vertical = Convert.ToInt32(btn.Tag);
            int horizontal = Matrix[vertical].IndexOf(btn);
            Point point = new Point(horizontal, vertical);
            return point;
        }

        private bool isEndHorizontal(Button btn)
        {
            Point point = getChessPoint(btn);

            int countLeft = 0;
            for (int i = point.X; i >= 0; i--)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                    countLeft++;
                else break;
            }

            int countRight = 0;
            for (int i = point.X + 1; i < Const.chessBoardWidth; i++)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                    countRight++;
                else break;
            }


            return countLeft + countRight == 5;
        }

        private bool isEndVertical(Button btn)
        {
            Point point = getChessPoint(btn);

            int countTop = 0;
            for (int i = point.Y; i >= 0; i--)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                    countTop++;
                else break;
            }

            int countBottom = 0;
            for (int i = point.Y + 1; i < Const.chessBoardHeight; i++)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                    countBottom++;
                else break;
            }


            return countTop + countBottom == 5;
        }

        private bool isEndPrimaryDiagonal(Button btn)
        {
            Point point = getChessPoint(btn);

            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.Y - i < 0 || point.X - i < 0)
                    break;

                if (Matrix[point.Y - i][point.X - i].BackgroundImage == btn.BackgroundImage)
                    countTop++;
                else break;
            }

            int countBottom = 0;
            for (int i = 1; i <= Const.chessBoardWidth - point.X; i++)
            {
                if (point.Y + i >= Const.chessBoardHeight || point.X + i >= Const.chessBoardWidth)
                    break;

                if (Matrix[point.Y + i][point.X + i].BackgroundImage == btn.BackgroundImage)
                    countBottom++;
                else break;
            }


            return countTop + countBottom == 5;
        }

        private bool isEndSubDiagonal(Button btn)
        {
            Point point = getChessPoint(btn);

            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.X + i > Const.chessBoardWidth || point.Y- i < 0)
                    break;

                if (Matrix[point.Y - i][point.X + i].BackgroundImage == btn.BackgroundImage)
                    countTop++;
                else break;
            }

            int countBottom = 0;
            for (int i = 1; i <= Const.chessBoardWidth - point.X; i++)
            {
                if (point.Y + i >= Const.chessBoardHeight || point.X - i < 0)
                    break;

                if (Matrix[point.Y + i][point.X - i].BackgroundImage == btn.BackgroundImage)
                    countBottom++;
                else break;
            }


            return countTop + countBottom == 5;
        }

        private void Mark(Button btn)
        {
            btn.BackgroundImage = Player[CurrentPlayer].Mark;
        }

        private void changePlayer()
        {
            PlayerName.Text = Player[CurrentPlayer].Name;
            PlayerMark.Image = Player[CurrentPlayer].Mark;
        }
        #endregion

    }

    public class ButtonClickEvent : EventArgs
    {
        private Point clickedPoint;

        public Point ClickedPoint
        {
            get
            {
                return clickedPoint;
            }

            set
            {
                clickedPoint = value;
            }
        }
        public ButtonClickEvent(Point point)
        {
            this.ClickedPoint = point;
        }
    }
}
