"Lab 1" by Cooper Mattern

[ Helper Functions ]
After looking:
	let exit count be 0;
	repeat with way running through directions:
		let destination be the room the way from the location;
		if destination is a room:
			if exit count is 0:
				say "From here, you can go:[line break]";
			say " [way] to [destination].";
			now exit count is exit count + 1;
	if exit count is 0:
		say "There are no obvious exits from here.";

[ Regions ]
Clearhollow is a region.

The Village is a region in Clearhollow.
The Boundaries is a region in Clearhollow.

East Village is a region in The Village.
West Village is a region in The Village.
North Village is a region in The Village.
South Village is a region in The Village.

[ Village Boundries ]
The Lake is a room in The Boundaries.
	West of The Lake is nowhere.
The North Forest is a room in The Boundaries.
	South of The North Forest is nowhere.
The West Forest is a room in The Boundaries.
	East of The West Forest is nowhere.
The South Forest is a room in The Boundaries.
	North of The South Forest is nowhere.

[ Village Centres ]
Village Centre is a room in The Village.

East Village Centre is a room in East Village.
	East Village Centre is east of Village Centre.
	East of East Village Centre is The Lake.

West Village Centre is a room in West Village.
	West Village Centre is west of Village Centre.
	West of West Village Centre is The West Forest.

North Village Centre is a room in North Village.
	North Village Centre is north of Village Centre.
	North of North Village Centre is The North Forest.

South Village Centre is a room in South Village.	
	South Village Centre is south of Village Centre.
	South of South Village Centre is The South Forest.

[ East Village]
The Fishing House is a room in East Village.
	The Fishing House is north of East Village Centre.
JosephHopeHouse is a room in East Village.
	The printed name of JosephHopeHouse is "Joseph and Hope's House".
	JosephHopeHouse is south of East Village Centre.

[ West Village ]
My Plot is a region in West Village.

My House is a room in My Plot.
    The House Door is a door.
		The House Door is north of My House and south of My Yard.
    My Bed is a supporter in My House.

My Yard is a room in My Plot.
    The Yard Door is a door.
		The Yard Door is north of My Yard and south of the West Village Centre.
    The Pen Door is a door.
		The Pen Door is west of My Yard and east of My Goat Pen.

My Goat Pen is a room in My Plot.


[ North Village ]
The Wheat Field is a room in North Village.
	The Wheat Field is east of North Village Centre.
	The Wheat Field is north of The Fishing House.
	East of The Wheat Field is The Lake.
	North of The Wheat Field is The North Forest.

[ South Village ]

[ Direction Overrides ]
Instead of going east when the location is East Village Centre or the location is The Wheat Field:
	say "You dont feel like going for a swim today. Let's focus on finding our goat.";
	stop the action.

Instead of going west when the location is West Village Centre:
	say "If your goat went in there, it's long gone. Let's keep looking around the village.";
	stop the action.
	
Instead of going north when the location is North Village Centre or the location is The Wheat Field:
	say "If your goat went in there, it's long gone. Let's keep looking around the village.";
	stop the action.

Instead of going south when the location is South Village Centre:
	say "If your goat went in there, it's long gone. Let's keep looking around the village.";
	stop the action.
	
[ Main Loop ]
The player is Rowan.
	Rowan is a person.
	Rowan is on My Bed.