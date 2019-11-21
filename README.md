# Amarath
StMU project for Files and Databases 2019

## Battle Sequence and Spawning
Enemies are generated when a random condition is met on a particular level. This will then spawn a single Enemy, which is dependent on the level of the location they are at. 

The battle sequence is started when the player types "attack". In all battles, the player will start first and then the computer will attack. For all attacks, both from the computer and the player, there is a chance for a dodge, a critical, or a normal attack. A dodge will simply negate any damage taken. A critical will take the maximum damage the attacking entity can do and then add on to that. And a normal hit is a random value between the minimum and maximum attack damage of an enemy. 

## Leveling
A character gains a level once they gain a total of 100 experience. If the character has more than the required experience, it will carry over to their next level.

For example: A Character who is lever 2 with 50 experience and then gains 100 experience, will then be level 3 with 50 expeprience.

Leveling grants the following:
 - Healed 30 points
 - 10 bonus points to the Max health
 - 1 point bonus to all stats (Strength, Intelligence, Dexterity)
 
