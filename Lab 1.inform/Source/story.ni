"Lab 1" by Cooper Mattern

[ Kinds and Properties ]
A goat-status is a kind of value.  The goat-statuses are unaware, knows-missing, knows-stable, knows-found.
A person has a goat-status called status.  The status of a person is usually unaware.

A person has a list of rooms called movement path.
A person has a number called movement delay.
A person has a number called talk delay.
A person can be moving or idle.  A person is usually idle.
The talk delay of a person is usually 0.

A person has a table name called schedule.
A person has a truth state called waiting for the player to leave.
The waiting for the player to leave of a person is usually false.

A person has a truth state called knows-Allen-is-guilty.
The knows-Allen-is-guilty of a person is usually false.

A person has an object called conversing‑with.

[ NPC Attributes for Schedules ]
Table of Joseph's Schedule
time	destination
8:00 AM	The Common Area
9:00 AM	The Fishing House
5:00 PM	The Common Area
7:00 PM	JosephHopeHouse

Table of Hope's Schedule
time	destination
8:00 AM	The Common Area
9:00 AM	The Village Shoppe
5:00 PM	The Common Area
7:00 PM	JosephHopeHouse

Table of Melia's Schedule
time	destination
7:00 AM	The Taproom
9:00 AM	The Taproom
5:00 PM	The Taproom
7:00 PM	MeliaTomHouse

Table of Tom's Schedule
time	destination
8:00 AM	The Common Area
9:00 AM	The Wheat Field
5:00 PM	The Taproom
7:00 PM	MeliaTomHouse

Table of Allen's Schedule
time	destination
8:00 AM	The Common Area
9:00 AM	The South Forest
5:00 PM	The Taproom
7:00 PM	AllenHouse

[ Helper Functions ]
After looking:
	let exit count be 0;
	repeat with way running through directions:
		let destination be the room the way from the location;
		if destination is a room:
			if exit count is 0:
				say "From here, you can go:[line break]";
			let the portal be the door the way from the location;
			if portal is a door:
				if portal is closed:
					say "    [way] through [the portal].[line break]";
				else:
					say "    [way] through the open [portal] to [destination].[line break]";
			else:
				say "    [way] to [destination].[line break]";
			now exit count is exit count + 1;
	if exit count is 0:
		say "There are no obvious exits from here.";

Before doing something other than looking, examining, or getting off when the player is on a supporter (called the perch):
	say "You climb down from [the perch].";
	silently try getting off the perch;
	continue the action.
	
[ Get destination of a path ]
To decide which room is the last entry in (L - a list of rooms):
	decide on entry the number of entries in L of L.
	
[ Breadth‑first path search ]
To decide which list of rooms is the path from (startRoom – a room) to (endRoom – a room):
	let frontier be a list of list of rooms;
	let initialPath be a list of rooms;
	add startRoom to initialPath;
	add initialPath to frontier;
	while the number of entries in frontier > 0:
		let currentPath be entry 1 of frontier;
		remove entry 1 from frontier;
		let currentRoom be the last entry in currentPath;
		if currentRoom is endRoom:
			remove entry 1 from currentPath;
			decide on currentPath;
		repeat with way running through directions:
			let neighbor be the room way from currentRoom;
			if neighbor is a room and neighbor is not listed in currentPath:
				let newPath be a list of rooms;
				repeat with R running through currentPath:
					add R to newPath;
				add neighbor to newPath;
				add newPath to frontier;
	decide on {};  [ no path found → empty list ]
	
[ NPC Movement Using Dynamic Paths ]
To set the movement path of (character - a person) from (start - a room) to (end - a room):
	let calculated path be the path from start to end;
	now the movement path of character is calculated path;
	now character is moving.

[ Story Hook ]
When play begins:
	say "You wake up to the sound of birds chirping outside your window. It's a peaceful morning in Clearhollow.";
	now the conversing‑with of the player is nothing;
	now the schedule of Joseph is the Table of Joseph's Schedule;
	now the schedule of Hope is the Table of Hope's Schedule;
	now the schedule of Melia is the Table of Melia's Schedule;
	now the schedule of Tom is the Table of Tom's Schedule;
	now the schedule of Allen is the Table of Allen's Schedule.

Instead of going north from My House for the first time:
	say "As you step outside, you notice your prized goat is missing from the pen. Determined to find it, you set off to investigate.";
	now the status of the player is knows-missing;
	continue the action;

[ Global Region ]
Clearhollow is a region.

[ The Village ]
The Village is a region in Clearhollow.

	[ Village Centre ]
	Village Centre is a room in The Village. "The heart of the village, bustling with activity during the day. A large oak tree stands in the center."

[ East Village ]
East Village is a region in The Village.

	[ East Village Centre ]
	East Village Centre is a room in East Village. "This part of the village is quieter, with a few houses and the path leading to the lake."
	East Village Centre is east of Village Centre.
	East of East Village Centre is The Lake.

	[ Fishing House ]
	The Fishing House is a room in East Village. "The scent of fish lingers in the air. Nets and fishing gear are scattered around."
		The Fishing Door is a door.
		The Fishing Door is south of The Fishing House and north of East Village Centre.
		The Fishing Door is locked.

	[ John and Hope's House ]
	JosephHopeHouse is a room in East Village. "A cozy home with a well-kept garden in front."
		The printed name of JosephHopeHouse is "Joseph and Hope's House".
		JHHDoor is a door.
			The printed name of JHHDoor is "Joseph and Hope's House's Door".
			JHHDoor is south of East Village Centre and north of JosephHopeHouse.
			JHHDoor is locked.

[ West Village ]
West Village is a region in The Village.

	[ West Village Centre ]
	West Village Centre is a room in West Village. "The western part of the village feels more rustic, with dirt paths and wooden fences."
	West Village Centre is west of Village Centre.
	West of West Village Centre is The West Forest.

	[ My House ]
	My Plot is a region in West Village.

		My House is a room in My Plot. "Your home is modest but comfortable, with a bed and a small table."
			The House Door is a door.
				The House Door is north of My House and south of My Yard.
			My Bed is a supporter in My House.

		My Yard is a room in My Plot. "The yard is fenced in, with a path leading to the goat pen."
			The Yard Door is a door.
				The Yard Door is north of My Yard and south of the West Village Centre.
			The Pen Door is a door.
				The Pen Door is west of My Yard and east of goat-pen.

		The goat-pen is a room in My Plot. "The pen is empty, with the gate slightly ajar. Your goat is missing."
			The printed name of goat-pen is "My Goat Pen".

	[ Village Tavern ]
	Village Tavern is a region in West Village.

		The Tavern Stable is a room in Village Tavern. "The stable smells of hay and animals. It's where villagers keep their livestock temporarily."
			The Tavern Door is a door.
				The Tavern Door is east of The Common Area and west of The Tavern Stable.
			The Tavern Stable is north of West Village Centre.

		MossMug is a region in Village Tavern.
			The printed name of MossMug is "Moss and Mug Tavern".
			The Common Area is a room in MossMug. "The tavern's main area, filled with tables and chairs. Villagers gather here to eat and talk."
				The Tavern Back Door is a door.
					The Tavern Back Door is north of The Common Area.
					North of The Tavern Back Door is The North Forest.
			The Taproom is a room in MossMug. "The taproom is lively, with the sound of mugs clinking and laughter echoing."
				The Taproom is west of The Common Area.
			The Lodge is a room in MossMug. "A quieter area of the tavern, reserved for overnight guests."
				The Lodge Door is a door.
					The Lodge Door is north of The Lodge and south of The Common Area.

[ North Village ]
North Village is a region in The Village.

	[ North Village Centre ]
	North Village Centre is a room in North Village. "The northern part of the village is surrounded by fields and forests."
	North Village Centre is north of Village Centre.
	North of North Village Centre is The North Forest.

	[ Melia and Tom's House ]
	MeliaTomHouse is a room in North Village. "A small but tidy home with a view of the wheat field."
		The printed name of MeliaTomHouse is "Melia and Tom's House".
		MTHDoor is a door.
			The printed name of MTHDoor is "Melia and Tom's House's Door".
			MTHDoor is west of North Village Centre and east of MeliaTomHouse.
			MTHDoor is locked.

	[ Wheat Field ]
	The Wheat Field is a room in North Village. "Golden stalks of wheat sway gently in the breeze. A hole is visible near the edge of the field."
		The Wheat Field is east of North Village Centre.
		The Wheat Field is north of The Fishing House.
		East of The Wheat Field is The Lake.
		North of The Wheat Field is The North Forest.

[ South Village ]
South Village is a region in The Village.

	[ South Village Centre ]
	South Village Centre is a room in South Village. "The southern part of the village has a few shops and houses."
		South Village Centre is south of Village Centre.
		South of South Village Centre is The South Forest.

	[ Allen's House ]
	AllenHouse is a room in South Village. "Allen's house is simple, with hunting gear propped against the walls."
		The printed name of AllenHouse is "Allen's House".
		AHDoor is a door.
			The printed name of AHDoor is "Allen's House's Door".
			AHDoor is west of South Village Centre and east of AllenHouse.
			AHDoor is locked.

	[ Village Shoppe ]
	The Village Shoppe is a room in South Village. "The shop is stocked with various goods, from fresh produce to tools."
		The printed name of The Village Shoppe is "The Common Shelf".
		VSDoor is a door.
			The printed name of VSDoor is "The Common Shelf's Door."
			VSDoor is east of South Village Centre and west of The Village Shoppe.

[ Village Boundaries ]
The Boundaries is a region in Clearhollow.
	The Lake is a room in The Boundaries. "The lake shimmers under the sunlight, its surface calm and inviting. Trees surround the area, creating a serene atmosphere."
		West of The Lake is nowhere.
	The North Forest is a room in The Boundaries. "The forest is dense and quiet, with tall trees casting long shadows on the ground."
		South of The North Forest is nowhere.
	The West Forest is a room in The Boundaries. "The forest here feels ancient, with gnarled roots and thick underbrush."
		East of The West Forest is nowhere.
	The South Forest is a room in The Boundaries. "The forest opens up slightly here, with patches of sunlight breaking through the canopy."
		North of The South Forest is nowhere.

	[ Direction Overrides ]
	Instead of
		going east when the room east of the location is The Lake
		or going west when the room west of the location is The West Forest
		or going north when the room north of the location is The North Forest
		or going south when the room south of the location is The South Forest:
		say "You dont feel like going for a swim today. Let's focus on finding our goat.";
		stop the action;

[ Persons and NPCs ]
The player is Rowan.
	Rowan is a person.
	Rowan is on My Bed.

Joseph is a person in JosephHopeHouse.
Hope is a person in JosephHopeHouse.
Melia is a person in MeliaTomHouse.
Tom is a person in MeliaTomHouse.
Allen is a person in AllenHouse.

[ NPC Movement Based on Schedules and Modular NPC Movement ]
Every turn:
	repeat with character running through people:
		if character is idle:
			say "[character] is idle [line break]";
			let best be 4 AM;
			repeat through the schedule of character:
				if the time entry is the time of day or the time entry is before the time of day:
					 if the time entry is after best:
						now best is the time entry;
			if there is a destination corresponding to a time of best in the schedule of character:
				Choose the row with time of best in the schedule of character;
				if the location of the character is not the destination entry:
					set the movement path of character from the location of character to the destination entry;
					say "character: [character][line break]    movement path:[movement path of character][line break]";
					now character is moving;
		if character is moving:
			let current room be the location of the character;
			say "[character] is moving [line break]    movement path:[the movement path of the character][line break]";
			if the number of entries in the movement path of the character > 0:
				let next room be entry 1 of the movement path of the character;
				if the character is the conversing-with of the player:
					if the talk delay of character < 15:
						say "[The character] pauses and says, 'I'm heading to [the last entry in the movement path of the character ], but I can spare a moment to talk.'";
						now the talk delay of character is the talk delay of character + 5;
					else:
						say "[The character] says, 'I really need to get going now.'";
						now the talk delay of character is 0;
						now the conversing-with of the player is nothing;
				else:
					say "    moving [character] from [current room] to [next room][line break]";
					move character to next room;
					remove entry 1 from the movement path of the character;
					say "    number of entries: [number of entries in the movement path of the character][line break]";
					if the number of entries in the movement path of the character is 0:
						now character is idle;

[ The Missing Goat ]
The Goat is an animal.
The Goat is in The Hole.
The Goat can be tethered or untethered.
The Goat can be stationary or following.
The Goat is stationary and untethered.

Instead of tying The Goat to something:
	if the player does not carry The Leash:
		say "You need something to tie the goat with.";
		stop the action;
	if the noun is The Tying Post:
		say "You tie the goat securely to the post.";
		now The Goat is tethered;
		now The Goat is fixed in place;
		stop the action;
	say "You can't tie the goat to that.";

Instead of taking The Goat:
	if the player does not carry The Leash:
		say "You can't take the goat without something to lead it.";
		stop the action;
	say "You attach the leash to the goat. The goat will now follow you.";
	now The Goat is untethered;

Every turn when Tom is in the location of the Goat and the Goat is tethered:
	now the Goat is untethered;
	now the Goat is following;
	say "Tom hands you the leash. The goat follows you now.";

Every turn when the Goat is following:
	let leader be the player;
	if Tom holds the leash, let leader be Tom;
	if the location of the Goat is not the location of leader:
		move the Goat to the location of leader;
		if the player can see the Goat:
			say "The goat follows [if the leader is the player]you[otherwise]Tom[end if] closely.";

Every turn when the Goat is untethered and the player carries the leash and the status of the player is knows-found:
	if the location of the Goat is not the location of the player:
		move the Goat to the location of the player;
		say "The goat trots after you, leash in hand.";
	otherwise if the player is in the goat-pen and the Goat is not fixed in place:
		say "The goat bleats softly, eyeing the tying post.";

The Hole is a container in The Wheat Field.
The Hole is fixed in place.

The Shovel is a thing in AllenHouse.
The Shovel is portable.

The description of The Shovel is "A sturdy shovel with a wooden handle. It looks recently used, with dirt still clinging to the blade.";

After taking the Shovel:
	say "You pick up the shovel. It might come in handy."

The Leash is a thing in My Yard. "A sturdy leather leash lies coiled on the ground."
The Leash is portable.

The Tying Post is a fixed in place thing in goat-pen. "A wooden post stands firmly in the ground, perfect for tying a goat to."

[ Time Progression Mechanic ]
The time of day is 7:01 AM.

The time rate is a number that varies.

When play begins:
	now the time rate is 5.

Every turn:
	increase time of day by 4 minutes;
	say "[time of day]";
	if the time of day is 8:00 PM:
		if the Goat is in The Tavern Stable:
			say "The sun sets, but at least your goat is safe in the stable. You can continue asking around to uncover the truth.";
		otherwise:
			end the story saying "The sun sets, and you still haven't found your goat. It's too late now.";

[ Breakfast Event ]
At 8:00 AM:
	say "You hear the breakfast bell ringing. Everyone in the village goes to the tavern across from your house for breakfast.";

[ Dinner Event ]
At 5:00 PM:
	say "You hear the dinner bell ringing from Moss and Mug. Everyone in the village gathers there for dinner.";

[ Midday Event (Hidden) ]
At 12:00 PM:
	set the movement path of Tom from The Wheat Field to The Taproom;
	now Tom is moving;
	now the Goat is tethered;
	now the Goat is following;

[ Tom’s Behavior at the Tavern Stable ]
Every turn:
	if Tom is in The Tavern Stable and the Goat is in The Tavern Stable:
		now the Goat is tethered;
		now the Goat is fixed in place;
		if the player is in The Tavern Stable:
			say "Tom ties the goat securely to a post in the Tavern Stable.";

Every turn:
	if Tom is moving and the player is in the location of Tom and the Goat is in the location of Tom:
		say "Tom notices you and hands you the leash to the goat. 'Here, you take care of it. I found it in a hole in the field,' he says.";
		now the Goat is untethered;
		now the Goat is following;
		now Tom is idle;
		now the waiting for the player to leave of Tom is true;
		stop the action;
[ Tom Resumes Movement After Player Leaves ]
Every turn:
	if the waiting for the player to leave of Tom is true and the player is not in the location of Tom:
		now the waiting for the player to leave of Tom is false;
		let best be 12:00 AM;
		repeat through the schedule of Tom:
			if the time entry is the time of day or the time entry is before the time of day:
				 if the time entry is after best:
					now best is the time entry;
		Choose the row with time of best in the schedule of Tom;
		set the movement path of Tom from the location of Tom to the destination entry;
		now Tom is moving;

[ Endings ]
Instead of examining The Goat when The Goat is in The Tavern Stable:
	if the player carries The Shovel:
		say "You realize Allen's shovel was used to dig the hole. Allen must have done it!";
		end the story saying "You found your goat and know who did it.";
	otherwise:
		say "You found your goat, but you have no idea who dug the hole.";
		now the status of the player is knows-found;

Instead of examining The Hole:
	If The Goat is in The Hole:
		move The Goat to The Wheat Field;
		now the status of the player is knows-found;
		say "You find your goat stuck in the hole. It looks scared but unharmed.";
	if the player carries The Shovel:
		say "You notice the hole was freshly dug, and Allen's shovel fits the marks perfectly.";
		now the knows-Allen-is-guilty of the player is true.

Instead of waiting when the time of day is 8:00 PM:
	if the knows-Allen-is-guilty of the player is true:
		end the story saying "You couldn't find your goat, but you know Allen was behind it.";
	otherwise if the status of the player is knows-found:
		end the story saying "You found your goat, but you have no idea who dug the hole.";
	otherwise:
		end the story saying "The sun sets, and you still haven't found your goat. It's too late now.".

[ Knocking Action ]
Knocking is an action applying to one thing.
Understand "knock on [something]" as knocking.

Check knocking:
	if the noun is not a door:
		say "You can't knock on that.";
		stop the action;
	if the noun is a door and the noun is not locked:
		say "There's no need to knock. The door is already unlocked.";
		stop the action;

Carry out knocking:
	if the noun is JHHDoor and Joseph is in JosephHopeHouse:
		move Joseph to the location;
		say "You knock on the door. Joseph steps out and says, 'What can I do for you?'";
		stop the action;
	if the noun is AHDoor and Allen is in AllenHouse:
		move Allen to the location;
		say "You knock on the door. Allen steps out, looking annoyed. 'What do you want?' he asks.";
		stop the action;
	if the noun is MTHDoor and Tom is in MeliaTomHouse:
		move Tom to the location;
		say "You knock on the door. Tom steps out, wiping his hands. 'Evening,' he says. 'What brings you here?'";
		stop the action;
	say "You knock, but no one answers.";

[ Prompt Player to Knock on Locked Doors ]
Before opening a locked door:
	if the door is JHHDoor and Joseph is in JosephHopeHouse:
		say "The door is locked. Perhaps you should try knocking.";
		stop the action;
	if the door is AHDoor and Allen is in AllenHouse:
		say "The door is locked. Perhaps you should try knocking.";
		stop the action;
	if the door is MTHDoor and Tom is in MeliaTomHouse:
		say "The door is locked. Perhaps you should try knocking.";
		stop the action;

[ NPC Response Tables ]
Table of NPC Responses About the Goat - Goat Responses
NPC (text)	Response if Seen in Stable (text)	Response if Not Aware of Goat (text)	Response if Told Found (text)
"Joseph"	"I saw your goat in the stable earlier, Joseph says. 'It looked fine to me.'"	"I haven't heard anything about your goat, maybe someone else knows something."	"That's great news! I'm glad you found it."
"Hope"	"I saw your goat in the stable earlier, Hope says. 'It seemed safe there.'"	"I haven't heard anything about your goat, Hope says. 'Sorry I can't help.'"	"I'm so glad you found your goat,' Hope says with a smile."
"Melia"	"Your goat is in the stable now, Melia says. 'But it's strange how it ended up in that hole.'"	"I haven't heard anything about your goat, Melia says. 'Maybe Tom knows something.'"	"Good to hear you found your goat,' Melia says. 'That must be a relief.'"
"Tom"	"I found your goat in the field,' Tom says. 'It was stuck in a hole. Strange, isn't it?'"	"I haven't heard anything about your goat,' Tom says. 'I'll let you know if I find anything.'"	"Glad you found your goat,' Tom says. 'I was worried about it.'"
"Allen"	"I saw your goat in the stable earlier, Allen says. 'It looked fine to me.'"	"I haven't heard anything about your goat,' Allen says curtly."	"Good for you,' Allen says. 'At least that's one less thing to worry about.'"

[ NPC Response Logic ]
Instead of telling someone about "goat":
	let name be the printed name of noun;
	now the conversing-with of the player is noun;
	Choose the row with NPC of name in the Table of Goat Responses;
	if the status of the player is knows-found:
		say "[Response if Told Found entry]";
		now the status of noun is knows-found;
	otherwise if the status of the player is knows-stable:
		say "[Response if Seen in Stable entry]";
	otherwise:
		say "[Response if Not Aware of Goat entry]";
		now the status of noun is knows-missing;

Instead of asking someone about "goat":
	let name be the printed name of noun;
	now the conversing-with of the player is noun;
	Choose the row with NPC of name in the Table of Goat Responses;
	if the status of noun is knows-found:
		say "[Response if Told Found entry]";
	otherwise if the status of noun is knows-stable:
		say "[Response if Seen in Stable entry]";
	otherwise if the status of noun is unaware:
		say "[Response if Not Aware of Goat entry]";
		
Every turn when the conversing‑with of the player is not nothing
	and the location of the conversing‑with of the player is not the location of the player:
	now the conversing‑with of the player is nothing;

[ NPC Awareness Update ]
Every turn:
	repeat with character running through people:
		if character is in The Tavern Stable and the Goat is in The Tavern Stable:
			now the status of character is knows-stable;