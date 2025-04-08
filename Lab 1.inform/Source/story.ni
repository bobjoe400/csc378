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

East Village is a region in Clearhollow. 
West Village is a region in Clearhollow. 
North Village is a region in Clearhollow. 
South Village is a region in Clearhollow. 

[ Village Boundries ]
The Lake is a room. 
	West of the Lake is nowhere.
The North Forest is a room.
	South of The North Forest is nowhere.
The West Forest is a room.
	East of the West Forest is nowhere.
The South Forest is a room.
	North of The South Forest is nowhere.

[ Village Centres ]
Village Centre is a room in Clearhollow. 

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
My House is a room in West Village.
	My House is south of West Village Centre.

[ North Village ]
The Wheat Field is a room in North Village.
	The Wheat Field is east of North Village Centre.
	The Wheat Field is north of The Fishing House.
	North of The Wheat Field is The North Forest.
	East of The Wheat Field is The Lake.

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
	Rowan is a person in My House.