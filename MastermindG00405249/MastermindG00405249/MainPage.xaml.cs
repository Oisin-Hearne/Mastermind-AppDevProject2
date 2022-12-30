using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MastermindG00405249
{
    //Empty= 411C15
    //Bg= 6b3520
    public partial class MainPage : ContentPage
    {
        //Variables
        ImageButton[] _pegs = new ImageButton[4];
        Random _rng = new Random();
        int _currentGuess = 0;
        int[] _generatedAns = new int[4];
        int[] _playerGuess = new int[4];
        Image[,] _emptyPegs = new Image[10, 4]; //A 2D array of Images is used to store the empty pegs and make them invisible from other methods.
        //I also used this to save the results of previous guesses.


        public MainPage()
        {
            InitializeComponent();
            StartGame();

        }

        private void StartGame()
        {
            SetUpGrid();
            ComputerGuess();
            MoveGuessZone();


        }

        private void MoveGuessZone()
        {
            for(int i = 0; i < 4; i++)
            {
                _pegs[i].Source = "1.png";
                _playerGuess[i] = 1;

                MainGrid.Children.Add(_pegs[i]);
                _pegs[i].SetValue(Grid.RowProperty, 12-_currentGuess);
                _pegs[i].SetValue(Grid.ColumnProperty, 3+i);               
            }
        }

        private void peg_Clicked(object sender, EventArgs e)
        {
            int y = Grid.GetColumn(((ImageButton)sender))-3;

            if (_playerGuess[y] != 6)
            {
                _playerGuess[y]++;
            }
            else
                _playerGuess[y] = 1;

            ((ImageButton)sender).Source = _playerGuess[y] + ".png";
        }

        private void ComputerGuess()
        {
            ImageButton b;
            for(int i = 0; i < 4; i ++)
            {
                b = new ImageButton();
                _generatedAns[i] = _rng.Next(1, 7);

                b.Source = _generatedAns[i] + ".png";
                b.HorizontalOptions = LayoutOptions.Center;
                b.VerticalOptions = LayoutOptions.Center;
                b.IsVisible = true;

                MainGrid.Children.Add(b);
                b.SetValue(Grid.RowProperty, 1);
                b.SetValue(Grid.ColumnProperty, 3+i);
            }
        }

        private void SetUpGrid()
        {
            Frame f;
            for(int r = 0; r < 10; r++)
            {
                //Place Images over the main area of the grid using the _emptyPegs 2D array. These images will get updated
                //as the player makes guesses.
                for (int c = 0; c < 4; c++)
                {
                    _emptyPegs[r, c] = new Image();

                    _emptyPegs[r, c].Source = "empty.png";
                    _emptyPegs[r, c].HorizontalOptions = LayoutOptions.Center;
                    _emptyPegs[r, c].VerticalOptions = LayoutOptions.Center;
                    _emptyPegs[r, c].IsVisible = true;

                    MainGrid.Children.Add(_emptyPegs[r, c]);
                    _emptyPegs[r, c].SetValue(Grid.RowProperty, r + 3);
                    _emptyPegs[r, c].SetValue(Grid.ColumnProperty, c + 3);
                }

                //Create Frames for the answer pegs to fit into.
                f = new Frame();
                f.HorizontalOptions = LayoutOptions.Center;
                f.VerticalOptions = LayoutOptions.Center;
                f.BackgroundColor = Color.FromHex("#572820");
                f.BorderColor = Color.Black;
                f.HeightRequest = 30;
                f.WidthRequest = 30;
                f.CornerRadius = 5;

                MainGrid.Children.Add(f);
                f.SetValue(Grid.RowProperty, r + 3);
                f.SetValue(Grid.ColumnProperty, 1);
            }


            //Initialize _pegs so that they can be moved when the Guess button is clicked.
            for(int i = 0; i < 4; i++)
            {
                _pegs[i] = new ImageButton();
                _pegs[i].Clicked += peg_Clicked;
                _pegs[i].HorizontalOptions = LayoutOptions.Center;
                _pegs[i].VerticalOptions = LayoutOptions.Center;
            }
        }

        private void BtnGuess_Clicked(object sender, EventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                _emptyPegs[9 - _currentGuess, i].Source = _playerGuess[i] + ".png";

            }

            MakeGuess();

            _currentGuess++;
            MoveGuessZone();
        }

        private void MakeGuess()
        {
            int[,] numColors = new int[6, 2]; //Columns: 0 for player colors, 1 for computer chosen colors.
            int fullyCorrect=0, partiallyCorrect=0, countCorrect=0; //Tracks number of pegs to place. Countcorrect is total.

            //Step 1 - Check for fully correct positions.
            for (int i = 0; i < 4; i++)
            {
                if (_generatedAns[i] == _playerGuess[i])
                {
                    fullyCorrect++;
                    countCorrect++;
                }
                else 
                {
                    //Add current colors to a list.
                    numColors[_playerGuess[i]-1, 0]++;
                    numColors[_generatedAns[i]-1, 1]++;
                }
            }

            //Step 2 - Check for partially correct positions
            for (int i = 0; i < 6; i++)
            {
                if (numColors[i, 0] > 0 && numColors[i, 1] > 0) //If both the player and the computer have picked this colour.
                {
                    //For when the number of the color does not match. Different equations are used based on whether
                    //there are more colors in the players guess, or in the computer's guess.
                    if (numColors[i, 1] != numColors[i, 0])
                    {
                        if(numColors[i,1] > numColors[i,0]) //PC > Player.
                        {
                            partiallyCorrect += numColors[i, 1] - (numColors[i, 1] - numColors[i, 0]);
                            countCorrect += numColors[i, 1] - (numColors[i, 1] - numColors[i, 0]);
                        }
                        else //Player > PC
                        {
                            partiallyCorrect += numColors[i, 0] + (numColors[i, 1] - numColors[i, 0]);
                            countCorrect += numColors[i, 0] + (numColors[i, 1] - numColors[i, 0]);
                        }
                    }
                    else //If they do match
                    {
                        partiallyCorrect += numColors[i, 0];
                        countCorrect += numColors[i, 0];
                    }
                }
            }

            placeResults(partiallyCorrect, fullyCorrect);

            LblTitle.Text = "Partial: " + partiallyCorrect + " Full: " + fullyCorrect +" All: " +countCorrect;

        }

        private void placeResults(int p, int f)
        {
            Grid results = new Grid
            {
                RowDefinitions = { new RowDefinition(), new RowDefinition()},
                ColumnDefinitions = { new ColumnDefinition(), new ColumnDefinition()}
            };

            results.Padding = 10;
            results.ColumnSpacing = 0;

            Image im;
            for(int r = 0; r < 2; r++)
            {
                for(int c = 0; c < 2; c++)
                {
                    im = new Image();
                    im.Margin = 0;

                    //Place black pegs first
                    if (f > 0)
                    {
                        im.Source = "black.png";
                        f--;
                    }
                    else if (p > 0) //Then place white pegs
                    {
                        im.Source = "white.png";
                        p--;
                    }
                    else //And if there's no white or black pegs left, place an empty slot.
                        im.Source = "empty.png";

                    //Place resulting image on grid
                    results.Children.Add(im);
                    im.SetValue(Grid.RowProperty, r);
                    im.SetValue(Grid.ColumnProperty, c);
                }
            }

            //Place results in the main grid
            MainGrid.Children.Add(results);
            results.SetValue(Grid.RowProperty, 12-_currentGuess);
            results.SetValue(Grid.ColumnProperty, 1);

        }
    }
}
