using System;
using System.Collections.Generic;
using System.Text;

namespace MastermindG00405249
{
    public class GameState
    {
        public int[,] guesses;
        public int[] answer;
        public int[,] results;
        public int currentTurn;


        public GameState()
        {
            guesses = new int[10, 4];
            results = new int[10, 4];
            answer = new int[4];
            currentTurn = 0;
        }
    }
}
