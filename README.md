# Amarath
StMU project for Files and Databases 2019
## Inventory
Opening up the Inventory will query the database for all the items that the player has. Armor and Weapons, such as Chestplates, Helmets, Wands, and Daggers, can be Equipped and Unequpped. When the player Equipps an item, the button will change the Uneqiup and vice versa. Other items can be used, such as potions. These items will be removed from the inventory once they have been used.

## Battle Sequence and Spawning
Enemies are generated when a random condition is met on a particular level. This will then spawn a single Enemy, which is dependent on the level of the location they are at. 

The battle sequence is started when the player types "attack". In all battles, the player will start first and then the computer will attack. For all attacks, both from the computer and the player, there is a chance for a dodge, a critical, or a normal attack. A dodge will simply negate any damage taken. A critical will take the maximum damage the attacking entity can do and then add on to that. And a normal hit is a random value between the minimum and maximum attack damage of an enemy. 
Changes are not saved to the database until the battle sequence is over. Luckily, battles happen so fast that this is not an issue.

## Leveling
A character gains a level once they gain a total of 100 experience. If the character has more than the required experience, it will carry over to their next level.

For example: A Character who is lever 2 with 50 experience and then gains 100 experience, will then be level 3 with 50 expeprience.

Leveling grants the following:
 - Healed 30 points
 - 10 bonus points to the Max health
 - 1 point bonus to all stats (Strength, Intelligence, Dexterity)
 
 Leveling will immediatly save everything to the database
 
 ## Locations
 There are currently 13 different Locations in the game. All of these locations are stored inside the database
#### Level 0: The Crypt Clearing
Welcome, you are an adventurer for hire and you have been hired to investigate an old crypt. The crypt was built into the side of a mountain, and was once covered with an elaborate carved stone facade but it was later partially covered by landslides. You see what looks like a natural cave entrance, big enough for a single man to enter comfortably.

#### Level 1: Inside the Crypt
Only when you get inside do they notice that the walls are carved stone, covered with hauntingly beautiful patterns of interwoven lines and strange writing in a language no living man knows. You enter a large chamber. The floor is wet from the seepage of groundwater through the ceiling. There is an elaborate set of double brass doors, slightly open, leading into an area with sealed stone doors in all directions. Each door is marked with the name of the deceased in elaborate writing. You see a broken door.

#### Level 2: The Antechamber
You enter a large chamber. The floor is wet from seepage of groundwater through the ceiling. There is an elaborate set of double brass doors, open slightly, leading into area. There are stone doors in all directions. Each door is marked with the name of the deceased in elaborate writing.

#### Level 3: The Pit
After investigating the 3 rooms a fourth door opens. The door leads to a rickety wooden bridge spanning a 15 foot wide pit. On other side there is a door with a magic seal.
You enter and look around.

#### Level 4: Accross the Bridge
You throw some rubble and the magic seal and it breaks, shattering the wall and scattering rubble around. When you look inside there is a small room with a fountain in the middle. There is a single, beautifully carved door with a sign that says Vault.

#### Level 5: Vault
This portion of the crypt was remodeled into a treasure vault by Nognor, an ambitious evil sorcerer who is now long dead. These passages have not been trodden in hundreds of years. As you enter into the vault you soon realize it is fake. After further investigating we find a draft leading to a hidden door.

#### Level 6: The Swamp
As you enter the next floor you see cracked trees with black trunks, rotting vegetation, scummy water, reeds, leeches all over. Soon after you are hit with the stench of rotting flesh and murky water. You are unsure of how you got here but all you know is that you must push forward.

#### Level 7: Goblin Camp
You enter a goblin camp. You see more than 20 bonfires scattered throughout the floor. The floor is dark other than the bonfires. There is a putrid stench of burring human flesh that makes your eyes water. You try to count the number of goblins but you lose count after 100. The ritualistic scream of the goblins resound through your ears.
You enter and look around.

#### Level 8: Large Clearing
Past the goblin camp, you find yourself in a clearing. This area looks remarkably well kept, as if the swamp fears to invade this area. In the center of the clearing you see a marble shrine emblazoned with runes.

#### Level 9: Shrine
You enter the shrine and you see a massive, glistening sword firmly implanted in a pedestal. This must be what the Wizard Nagnor wanted. But what is it exactly? You see a plaque embedded into the pedestal, surrounded by lattice.
You enter and look around.

#### Level 10: Inner Shrine
On the plaque are the words THE SWORD LOGIC. Remembering your studies, the Sword Logic demands fighting until everything weaker is dead so the world becomes its "perfect" form. Nognor defies the Sword Logic by reviving things that aren't strong enough to live on their own. It's blasphemy.” How can only the strongest survive if nothing can die” By making a pact directly with Xol to obtain his powers, he sidestepped the rigid hierarchy of the Hive and betrayed the Sword Logic. Obtaining power via a gift instead of taking it is of the Sky, not the Deep, and this hearsay is what he was exiled for. When the Council of Wizards exiled him, they enchanted the sword and hid it so that it could never again be wielded.

#### Level 11: Large Clearing
You exit the Shrine and find yourself back in the clearing. There he was. The infamous Nognor. He must have sensed your presence and decided to investigate. But he seems even more powerful now than ever before. Maybe even powerful enough to break the enchantment.

#### Level 12: Final Battle
Finally. After all this time you have come face to face with an enemy to humanity. This may have seemed like an ordinary job, but now you are what stands between Nognor and the Logic Sword. Now you must not fail.
 
