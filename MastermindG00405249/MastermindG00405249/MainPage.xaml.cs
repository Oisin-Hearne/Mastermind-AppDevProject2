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
        ImageButton _peg;
        Random _rng = new Random();
        int _currentGuess = 1;
        int[] _generatedGuess = new int[4];


        public MainPage()
        {
            InitializeComponent();
            StartGame();

            _peg = new ImageButton();
            _peg.HeightRequest = 76;
            _peg.WidthRequest = 76;
            _peg.CornerRadius = 38;
        }

        private void StartGame()
        {
            SetUpGrid();
            ComputerGuess();


        }

        private void ComputerGuess()
        {
            ImageButton b;
            for(int i = 0; i < 4; i ++)
            {
                b = new ImageButton();
                _generatedGuess[i] = _rng.Next(1, 6);

                b.Source = _generatedGuess[i] + ".png";
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
            Image b;
            for(int r = 3; r < 13; r++)
                for(int c = 3; c < 7; c++)
                {
                    b = new Image();

                    b.Source = "empty.png";
                    b.HorizontalOptions = LayoutOptions.Center;
                    b.VerticalOptions = LayoutOptions.Center;
                    b.IsEnabled = true;

                    MainGrid.Children.Add(b);
                    b.SetValue(Grid.RowProperty, r);
                    b.SetValue(Grid.ColumnProperty, c);
                }
        }
    }
}
