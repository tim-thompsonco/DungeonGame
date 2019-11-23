using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGame
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var playerOne = new Player();
            var zombie = new Monster();
            Console.WriteLine("You encountered a monster. What do you do? 'Fight' would be a good idea.");
            var input = Console.ReadLine();
            var inputL = input.ToLower();
            if (inputL == "fight")
            {
                playerOne.Combat(zombie);
            }
            if (playerOne.CheckHealth() <= 0)
            {
                Console.WriteLine("You have died.");
            }
        }
    }
}