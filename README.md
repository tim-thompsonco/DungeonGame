<h2>Dungeon Game</h2>

<p>This is my fun project to finish what I started with C when I was 14. My goal at the time was to build a simple text dungeon game. I didn't really understand OOP at the time, so that didn't go too far, but now I have learned OOP with C# from some courses on Udemy. The intent of this game was to provide as much coding practice as possible in C# and OOP, hence why I have not placed much of an emphasis on a very fancy GUI, since the goal is to build code rather than work with a game engine such as Unity. Everything in this game, from the combat engine, to the GUI, was built from scratch by me for this reason.</p>

<p>Update 5/7/21: This was my first project which I started in November 2019. The majority of the code does not currently reflect how I write code, but I am focusing on this project again, for practice in refactoring old legacy code. The game itself runs fine, but for current code quality examples, please review either recent commits or the Effects class files since those have already been refactored.</p>

<h2>Demo</h2>

https://user-images.githubusercontent.com/45145621/117673103-b452b880-b167-11eb-818a-765c4c73cfdc.mp4

<h2>Game Commands</h2>

<p>Whenever possible, I tried to include game commands in the game itself. However, here is a list of the most common commands that as a player you will need to remember.</p>

<p>i - This will display the player's inventory</p>
<p>ability [name] - This will show details on an ability in the player's ability list</p>
<p>spell [name] - This will show details on a spell in the player's spellbook</p>
<p>quest [name] - This will show details on a quest in the player's quest log</p>
<p>drop [item] - This will drop an item from the player's inventory onto the ground</p>
<p>pickup [item] - This will pick up an item on the ground and deposit it in the player's inventory</p>
<p>cast [spell] - This will cast a spell if the player is a Mage</p>
<p>use [ability] - This will use an ability if the player is a Warrior or Archer</p>
<p>kill [monster] / attack <monster> - This will initiate combat against the monster</p>
<p>loot [monster] - This will loot a monster that has been defeated</p>
<p>equip/unequip [item] - This will equip or unequip an item in the player's inventory if the item is wearable</p>
<p>drink [potion] - This will consume an item such as a potion if it is in the player's inventory</p>
<p>show forsale - This will show available items for sale at a vendor</p>
<p>buy/sell [item] - This will buy or sell an item if the player is in the same room as a vendor</p>
<p>buy [potion] [quantity] - This will buy the desired quantity of potions at the healer. IE "buy health 5" will buy 5 health potions.
<p>sell [item] [count] - This will sell an item with a duplicate name at the desired count. IE player has two "sturdy daggers" in inventory. "sell dagger 2" will sell the 2nd sturdy dagger.
<p>show upgrades - This will show available abilities/spells that can be upgraded at the appropriate class trainer</p>
<p>train [spell/ability] - This will train a new spell or ability at the appropriate class trainer</p>
<p>upgrade [spell/ability] - This will upgrade an existing spell or ability at the appropriate class trainer</p>
<p>repair [item] - This will repair an item if the player is in the same room as a vendor who can repair that type of item</p>
<p>repair all - This will attempt to repair all items equipped by a player if the player is in the same room as a vendor</p>
<p>enhance [armor/weapon] [kit] - This will upgrade a piece of armor or a weapon with an upgrade kit</p>
<p>restore - This will restore all player stats if the player is in the same room as a healer</p>
<p>e/w/s/n/u/d/ne/se/nw/sw - This will move the player in the specified direction</p>

<h2>Prerequisites</h2>

<p>A Windows PC.</p>

<h2>Installing</h2>

<p>To install the game, simply download the zip file for the desired release and unzip to a directory of your choice. The executable program can then be ran.</p>

<h2>Deployment</h2>

<p>All releases are built as an executable console application using .NET Framework 4.7.2.</p>

<h2>Built With</h2>

<p>JetBrains Rider</p>
<p>Jira</p>

<h2>Versioning</h2>

<p>Every major version release is an increment of 1.0. Every minor version release is an increment of 0.1.</p>

<h2>Author</h2>

<p>Tim Thompson</p>

<h2>Acknowledgments</h2>

<p>Made possible by the C# classes taught on Udemy by Mosh Hamedani</p>
