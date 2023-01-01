using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        Image[,] _emptyPegs = new Image[10, 4]; //A 2D array of Images is used to store the empty pegs and make them invisible from other methods.
        Random _rng = new Random();

        int _currentGuess = 0;

        int[] _generatedAns = new int[4];
        int[] _playerGuess = new int[4];

        int[,] _pastGuesses = new int[10, 4]; //Store past guesses & results for saving and loading them later.
        int[,] _pastResults = new int[10, 4];
        const String FILE_NAME = "MastermindSaveData.json";

        public MainPage()
        {
            InitializeComponent();
        }

        private void StartGame()
        {
            SetUpGrid();
            ComputerGuess();
            MoveGuessZone();
        }

        private void MoveGuessZone()
        {
            for (int i = 0; i < 4; i++)
            {
                _pegs[i].Source = "1.png";
                _playerGuess[i] = 1;

                MainGrid.Children.Add(_pegs[i]);
                _pegs[i].SetValue(Grid.RowProperty, 12 - _currentGuess);
                _pegs[i].SetValue(Grid.ColumnProperty, 3 + i);
            }
        }

        private void peg_Clicked(object sender, EventArgs e)
        {
            int y = Grid.GetColumn(((ImageButton)sender)) - 3;

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
            for (int i = 0; i < 4; i++)
            {
                b = new ImageButton();
                _generatedAns[i] = _rng.Next(1, 7);

                b.Source = _generatedAns[i] + ".png";
                b.HorizontalOptions = LayoutOptions.Center;
                b.VerticalOptions = LayoutOptions.Center;
                b.IsVisible = true;

                MainGrid.Children.Add(b);
                b.SetValue(Grid.RowProperty, 1);
                b.SetValue(Grid.ColumnProperty, 3 + i);
            }
        }

        private void SetUpGrid()
        {
            Frame f;
            for (int r = 0; r < 10; r++)
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
                    _emptyPegs[r, c].SetValue(Grid.RowProperty, r+3);
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
            for (int i = 0; i < 4; i++)
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
                _pastGuesses[_currentGuess, i] = _playerGuess[i]; //Save guess to past guesses
            }

            MakeGuess();

            _currentGuess++;
            MoveGuessZone();
        }

        private void MakeGuess()
        {
            int[,] numColors = new int[6, 2]; //Columns: 0 for player colors, 1 for computer chosen colors.
            int fullyCorrect = 0, partiallyCorrect = 0, countCorrect = 0; //Tracks number of pegs to place. Countcorrect is total.

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
                    numColors[_playerGuess[i] - 1, 0]++;
                    numColors[_generatedAns[i] - 1, 1]++;
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
                        if (numColors[i, 1] > numColors[i, 0]) //PC > Player.
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

            placeResults(partiallyCorrect, fullyCorrect, _currentGuess);

            LblTitle.Text = "Partial: " + partiallyCorrect + " Full: " + fullyCorrect + " All: " + countCorrect;

        }

        private void placeResults(int p, int f, int row)
        {
            int fullyCorrect = f;
            int partiallyCorrect = p;
            Grid results = new Grid
            {
                RowDefinitions = { new RowDefinition(), new RowDefinition() },
                ColumnDefinitions = { new ColumnDefinition(), new ColumnDefinition() }
            };

            results.Padding = 10;
            results.ColumnSpacing = 0;

            //Create an image of a peg and set its colour to black, white or empty based on the partially/fully correct answers.
            Image im;
            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 2; c++)
                {
                    im = new Image();
                    im.Margin = 0;

                    //Place black pegs first
                    if (fullyCorrect > 0)
                    {
                        im.Source = "black.png";
                        fullyCorrect--;
                    }
                    else if (partiallyCorrect > 0) //Then place white pegs
                    {
                        im.Source = "white.png";
                        partiallyCorrect--;
                    }
                    else //And if there's no white or black pegs left, place an empty slot.
                        im.Source = "empty.png";

                    //Place resulting image on grid
                    results.Children.Add(im);
                    im.SetValue(Grid.RowProperty, r);
                    im.SetValue(Grid.ColumnProperty, c);
                }
            }

            //Save values to an array for later loading/saving
            for (int i = 0; i < 4; i++)
            {
                //Same order as earlier.
                if (f > 0)
                {
                    _pastResults[row, i] = 2;
                    f--;
                }
                else if (p > 0)
                {
                    _pastResults[row, i] = 1;
                    p--;
                }
                else
                    _pastResults[row, i] = 0;
            }

            //Place results in the main grid
            MainGrid.Children.Add(results);
            results.SetValue(Grid.RowProperty, 12 - row);
            results.SetValue(Grid.ColumnProperty, 1);

        }

        private void SaveGame()
        {
            GameState gs = new GameState();
            String filepath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            String filename = Path.Combine(filepath, FILE_NAME);
            String text;

            //Save all the values used in the game: The current turn, the answer, and the past guesses & results.
            gs.currentTurn = _currentGuess;

            for (int i = 0; i < 4; i++)
                gs.answer[i] = _generatedAns[i];

            for (int i = 0; i < 10; i++)
            {
                for (int x = 0; x < 4; x++)
                {
                    gs.guesses[i, x] = _pastGuesses[i, x];
                    gs.results[i, x] = _pastResults[i, x];
                }
            }

            //Write to a file in the local app data.
            using (var w = new StreamWriter(filename, false))
            {
                text = JsonConvert.SerializeObject(gs);
                w.WriteLine(text);
            }
        }

        //Load a saved game
        private void LoadGame()
        {
            GameState gl = new GameState();
            String filepath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            String filename = Path.Combine(filepath, FILE_NAME);
            String text;

            try
            {
                using (var r = new StreamReader(filename))
                {
                    text = r.ReadToEnd();
                }
            }
            catch
            {
                text = "";
            }

            if (text != "") //Checks that it's not empty, load game
            {
                gl = JsonConvert.DeserializeObject<GameState>(text);

                //Load values
                _currentGuess = gl.currentTurn;
                for (int i = 0; i < 4; i++)
                    _generatedAns[i] = gl.answer[i];

                for (int i = 0; i < 10; i++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        _pastGuesses[i, x] = gl.guesses[i, x];
                        _pastResults[i, x] = gl.results[i, x];
                    }
                }

                //Initialize Game w/ loaded data.
                PlaceLoadedData();
                MoveGuessZone();
            }
            else //Tells the user that a saved file does not exist.
            {
                StartGame();
            }
        }

        //When loading a game, take the past guesses and past results and place them on the grid.
        private void PlaceLoadedData()
        {
            int partial, full;
            SetUpGrid();

            for(int r = 0; r < _currentGuess; r++)
            {
                //Place guess pegs
                for (int c = 0; c < 4; c++)
                {
                    if(_pastGuesses[r,c] != 0)
                      _emptyPegs[9-r, c].Source = _pastGuesses[r,c]+".png";
                }

                //Place result pegs
                partial = 0;
                full = 0;
                for (int c = 0; c < 4; c++)
                {
                    if (_pastResults[r, c] == 2)
                        full++;
                    if (_pastResults[r, c] == 1)
                        partial++;
                }
                placeResults(partial, full, r);

            }
        }

        private void BtnSave_Clicked(object sender, EventArgs e)
        {
            SaveGame();
            BtnSave.Text = "Game Saved!";
        }

        private void BtnLoad_Clicked(object sender, EventArgs e)
        {
            SLStart.IsVisible = false;
            SLGame.IsVisible = true;
            LoadGame();
        }

        private void BtnStart_Clicked(object sender, EventArgs e)
        {
            SLStart.IsVisible = false;
            SLGame.IsVisible = true;
            StartGame();
        }
    }
}
