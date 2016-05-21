BurgerBurgerBurger is a personal project to recreate the Dreamcast game "Chu Chu Rocket" as a Universal Windows Platform app.

Game Rules / Requirements
	• Game is local multiplayer up to 4 players (played with controllers).
	• Games are timed.  Player with the highest score after a given time wins.
	• Game is played on a 2d grid board.
	• Placed on the grid board are "spawners".  Spawners constantly produce mice (good) and cats (bad).
	• Mice and Cats spawn a fixed direction from a spawner (up, down, left, right).  They walk straight until they hit a wall, then they turn and walk straight again.
	• Players have "bases" placed on the board.
	• Players get points when mice go into their base.
	• Players lose points when cats go into their base.
	• Players can direct mice and cats by placing arrows on the board.  
		• Arrows can be in any of the 4 directions: up, down, left, and right.
		• Whenever a mouse or cat walks over the arrow, they change direction in to the way the arrow is pointing.
		• Players have 3 arrows to place.  When 3 arrows are placed, when the player places another arrow, the first arrow placed is removed.
		• Arrows stay in the same place until they are moved by the player by the mechanic above.
		• Players cannot overwrite each others' arrows.  If an arrow is on a space, another player cannot place a different arrow on that space.
