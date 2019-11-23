using System;

namespace DungeonGame
{
  public static class Helper {
    public static string GetFormattedInput() {
      var input = Console.ReadLine();
      var inputFormatted = input.ToLower();
      return inputFormatted;
    }
    public static void RequestCommand() {
      Console.Write("Your command: ");
    }
    public static void PlayerDeath() {
      Console.WriteLine("You have died.");
    }
  }
}