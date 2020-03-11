# Khaios Core Mod
 Khaios Core is a mod for Terraria that implements "Command Blocks" into the game. These are designed to be attached to wires in Terraria in order to execute commands. There are currently seven commands that can be executed which do a variety of things that may help map makers create adventure maps. 
 
# Commands

There are a few ways to write a command. Please use this key to help understand what symbols are.
|	- This means or. for example, each command has a long and short way. For example weather has weather or just w.
[]	- These are optional. These will mostly be at the end of commands.
""	- Anything inside quotations requires quotations around it. For example if getting an item, if you do "Iron Shortsword" it will correctly find it
~	- This means relative. This is related to X and Y coords. So if a command says ~X then the number to enter will be relative to the x coord. For example, if the tile is at X: 2000 and the command is -20 the relative position would be 1980.
()	- This means the command is a number. For example (Radius) you need to replace that with a number.

Time Command:
	t|time set (value)|day|night|midday|midnight
	t|time add (value)

	Examples:
		t set midday
		time add 1000

Weather Command:
	w|weather set|toggle clear|rain|storm|slime
	w|weather clear

	Examples:
		weather set slime
		w clear

Item Command:
	i|item give|take "name"|(id) [quantity]

	Examples:
		item give "iron shortsword"
		item give 1 99

		item take "iron shortsword"
		item take 1 99

Say Command:
	s|say "message"

	Examples:
		s "Hello, World!"
		say "Broadcast Message!"

Kill Command
	k|kill player|npc all|(radius)

	Examples:
		k npc all
		kill player 16

Event Command
	e|event start bloodmoon|goblin|legion|solar|pirate|pumpkin|frost|martian

	bloodmoon - Blood Moon
	goblin - Goblin Invasion
	legion - Frost Legion
	solar - Solar Eclipse
	pirate - Pirate Invasion
	pumpkin - Pumpkin Moon
	frost - Frost Moon
	martian - Martian Invasion

	Examples:
		e start goblin
		event start pumpkin


Summon Command
	su|summon (id)|"name" ~x ~y

	Examples:
		summon "blue slime" 0 -5
		summon 93 10 5
		su "red slime" 0 0
