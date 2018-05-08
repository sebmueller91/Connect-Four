using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConnectFour
{
    enum Player { Red, Yellow, Empty };

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const Player AI = Player.Red;

        private Player[,] States { get; set; }
        public Button[,] Buttons { get; set; }

        private static Random rand;

        private Player m_NextMove;
        private Player NextMove
        {
            get
            {
                return m_NextMove;
            }
            set
            {
                if (value != m_NextMove)
                {
                    m_NextMove = value;
                    //if (m_NextMove == AI && GetWinner(States) == State.Empty)
                    //{
                    //    DoAIMove();
                    //}
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
            rand = new Random();

            Buttons = new Button[MyGrid.Rows, MyGrid.Columns];
            States = new Player[MyGrid.Rows, MyGrid.Columns];

            for (int i = 0; i < MyGrid.Rows; i++)
            {
                for (int j = 0; j < MyGrid.Columns; j++)
                {
                    Button button = new Button
                    {
                        Background = Brushes.Blue,
                        Foreground = Brushes.LightGray,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Height = Double.NaN,
                        Width = Double.NaN,
                        FontSize = 70,
                        BorderThickness = new Thickness(0),
                        Content = "\u26AB",
                        Name = "B" + (i * MyGrid.Columns + j).ToString()
                    };

                    button.Click += Button_Click;

                    Buttons[i, j] = button;
                    MyGrid.Children.Add(button);

                    States[i, j] = Player.Empty;
                }
            }

            NextMove = (rand.Next(0, 2) == 0) ? Player.Yellow : Player.Red;
        }

        private void Reset()
        {
            foreach (var button in Buttons)
            {
                button.Foreground = Brushes.LightGray;
            }

            for (int i = 0; i < MyGrid.Rows; i++)
            {
                for (int j = 0; j < MyGrid.Columns; j++)
                {
                    States[i, j] = Player.Empty;
                }
            }

            NextMove = (rand.Next(0, 2) == 0) ? Player.Red : Player.Yellow;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int buttonNo = Convert.ToInt32((sender as Button).Name.Replace("B", ""));
            int col = buttonNo % MyGrid.Columns;

            List<Button> buttons;
            if (GetWinner(States, out buttons) == Player.Empty && DoMove(col))
            {                
                GetWinner(States, out buttons);
                MarkWinner(buttons);
            }
        }

        public void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.R)
            {
                Reset();
            }
        }

        private bool DoMove(int col)
        {
            if (col < 0 || col >= MyGrid.Columns)
            {
                return false;
            }

            for (int i = MyGrid.Rows - 1; i >= 0; i--)
            {
                if (States[i, col] == Player.Empty)
                {
                    States[i, col] = NextMove;
                    Buttons[i, col].Foreground = (NextMove == Player.Red) ? Brushes.Red : Brushes.Yellow;
                    NextMove = GetInvPlayer(NextMove);
                    return true;
                }
            }

            return false;
        }

        private Player GetInvPlayer(Player player)
        {
            switch (player)
            {
                case Player.Red:
                    return Player.Yellow;
                case Player.Yellow:
                    return Player.Red;
                default:
                    return Player.Empty;
            }
        }

        private Player GetWinner(Player[,] states, out List<Button> buttons)
        {
            int ct = 0;
            Player lastField;
            buttons = new List<Button>();

            #region Check rows for winner
            for (int r = 0; r < MyGrid.Rows; r++)
            {
                lastField = states[r, 0];
                ct = (lastField != Player.Empty) ? 1 : 0;
                buttons.Clear();
                buttons.Add(Buttons[r, 0]);
                for (int c = 1; c < MyGrid.Columns; c++)
                {
                    if (states[r, c] == lastField && lastField != Player.Empty)
                    {
                        ct++;
                        buttons.Add(Buttons[r, c]);
                    }
                    else if (states[r, c] != Player.Empty)
                    {
                        ct = 1;
                        buttons.Clear();
                        buttons.Add(Buttons[r, c]);
                    }
                    else
                    {
                        ct = 0;
                        buttons.Clear();
                    }

                    if (ct == 4)
                    {
                        return lastField;
                    }

                    lastField = states[r, c];
                }
            }
            #endregion Check rows for winner

            #region Check columns for winner
            for (int c = 0; c < MyGrid.Columns; c++)
            {
                lastField = states[0, c];
                ct = (lastField != Player.Empty) ? 1 : 0;
                buttons.Clear();
                buttons.Add(Buttons[0, c]);
                for (int r = 1; r < MyGrid.Rows; r++)
                {
                    if (states[r, c] == lastField && lastField != Player.Empty)
                    {
                        ct++;
                        buttons.Add(Buttons[r, c]);
                    }
                    else if (states[r, c] != Player.Empty)
                    {
                        ct = 1;
                        buttons.Clear();
                        buttons.Add(Buttons[r, c]);
                    }
                    else
                    {
                        ct = 0;
                        buttons.Clear();
                    }

                    if (ct == 4)
                    {
                        return lastField;
                    }

                    lastField = states[r, c];
                }
            }
            #endregion Check columns for winner

            #region Check diagonals for winner
            #region Left top to right bottom diagonals
            for (int r = 0; r < MyGrid.Rows - 3; r++)
            {
                int c = 0;
                lastField = states[r, c];
                ct = (lastField != Player.Empty) ? 1 : 0;
                buttons.Clear();
                buttons.Add(Buttons[r, c]);
                for (int k = 1; r + k < MyGrid.Rows && c + k < MyGrid.Columns; k++)
                {
                    if (states[r + k, c + k] == lastField && lastField != Player.Empty)
                    {
                        ct++;
                        buttons.Add(Buttons[r + k, c + k]);
                    }
                    else if (states[r + k, c + k] != Player.Empty)
                    {
                        ct = 1;
                        buttons.Clear();
                        buttons.Add(Buttons[r + k, c + k]);
                    }
                    else
                    {
                        ct = 0;
                        buttons.Clear();
                    }

                    if (ct == 4)
                    {
                        return lastField;
                    }

                    lastField = states[r + k, c + k];
                }
            }

            for (int c = 1; c < MyGrid.Columns - 3; c++)
            {
                int r = 0;
                lastField = states[r, c];
                ct = (lastField != Player.Empty) ? 1 : 0;
                buttons.Clear();
                buttons.Add(Buttons[r, c]);
                for (int k = 1; r + k < MyGrid.Rows && c + k < MyGrid.Columns; k++)
                {
                    if (states[r + k, c + k] == lastField && lastField != Player.Empty)
                    {
                        ct++;
                        buttons.Add(Buttons[r + k, c + k]);
                    }
                    else if (states[r + k, c + k] != Player.Empty)
                    {
                        ct = 1;
                        buttons.Clear();
                        buttons.Add(Buttons[r + k, c + k]);
                    }
                    else
                    {
                        ct = 0;
                        buttons.Clear();
                    }

                    if (ct == 4)
                    {
                        return lastField;
                    }

                    lastField = states[r + k, c + k];
                }
            }
            #endregion Left top to right bottom diagonals

            #region Left Bottom to right top diagonals
            for (int r = 3; r < MyGrid.Rows; r++)
            {
                int c = 0;
                lastField = states[r, c];
                ct = (lastField != Player.Empty) ? 1 : 0;
                buttons.Clear();
                buttons.Add(Buttons[r, c]);
                for (int k = 1; r - k >= 0 && c + k < MyGrid.Columns; k++)
                {
                    if (states[r - k, c + k] == lastField && lastField != Player.Empty)
                    {
                        ct++;
                        buttons.Add(Buttons[r - k, c + k]);
                    }
                    else if (states[r - k, c + k] != Player.Empty)
                    {
                        ct = 1;
                        buttons.Clear();
                        buttons.Add(Buttons[r - k, c + k]);
                    }
                    else
                    {
                        ct = 0;
                        buttons.Clear();
                    }

                    if (ct == 4)
                    {
                        return lastField;
                    }

                    lastField = states[r - k, c + k];
                }
            }

            for (int c = 1; c < MyGrid.Columns - 3; c++)
            {
                int r = MyGrid.Rows - 1;
                lastField = states[r, c];
                ct = (lastField != Player.Empty) ? 1 : 0;
                buttons.Clear();
                buttons.Add(Buttons[r, c]);
                for (int k = 1; r - k >= 0 && c + k < MyGrid.Columns; k++)
                {
                    if (states[r - k, c + k] == lastField && lastField != Player.Empty)
                    {
                        ct++;
                        buttons.Add(Buttons[r - k, c + k]);
                    }
                    else if (states[r - k, c + k] != Player.Empty)
                    {
                        ct = 1;
                        buttons.Clear();
                        buttons.Add(Buttons[r - k, c + k]);
                    }
                    else
                    {
                        ct = 0;
                        buttons.Clear();
                    }

                    if (ct == 4)
                    {
                        return lastField;
                    }

                    lastField = states[r - k, c + k];
                }
            }
            #endregion Left Bottom to right top diagonals
            #endregion Check diagonals for winner

            return Player.Empty;
        }

        private void MarkWinner(List<Button> buttons)
        {

        }
    }
}
