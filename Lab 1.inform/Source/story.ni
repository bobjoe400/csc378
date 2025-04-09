"Lab 1" by Cooper Mattern

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
				say "     [way] to [destination].[line break]";
			now exit count is exit count + 1;
	if exit count is 0:
		say "There are no obvious exits from here.";

Before doing something other than
	looking, 
	examining,
	or getting off 
	when the player is on a supporter (called the perch):
	say "You climb down from [the perch].";
	silently try getting off the perch;
	continue the action.
	
[ Global Region ]
Clearhollow is a region.

[ Village Boundries ]
The Boundaries is a region in Clearhollow.
	The Lake is a room in The Boundaries.
		West of The Lake is nowhere.
	The North Forest is a room in The Boundaries.
		South of The North Forest is nowhere.
	The West Forest is a room in The Boundaries.
		East of The West Forest is nowhere.
	The South Forest is a room in The Boundaries.
		North of The South Forest is nowhere.

	[ Direction Overrides ]
	Instead of going east when the room east of the location is The Lake:
		say "You dont feel like going for a swim today. Let's focus on finding our goat.";
		stop the action.

	Instead of going west when the room west of the location is The West Forest:
		say "If your goat went in there, it's long gone. Let's keep looking around the village.";
		stop the action.
		
	Instead of going north when the room north of the location is The North Forest:
		say "If your goat went in there, it's long gone. Let's keep looking around the village.";
		stop the action.

	Instead of going south when the room south of the location is The South Forest:
		say "If your goat went in there, it's long gone. Let's keep looking around the village.";
		stop the action.

[ The Village ]
The Village is a region in Clearhollow.

	[ Village Centre ]
	Village Centre is a room in The Village.

[ East Village]
East Village is a region in The Village.

	[ East Village Centre ]
	East Village Centre is a room in East Village.
	East Village Centre is east of Village Centre.
	East of East Village Centre is The Lake.

	[ Fishing House ]
	The Fishing House is a room in East Village.
		The Fishing House is north of East Village Centre.

	[ John and Hope's House ]
	JosephHopeHouse is a room in East Village.
		The printed name of JosephHopeHouse is "Joseph and Hope's House".
		JHHDoor is a door.
			The printed name of JHHDoor is "Joseph and Hope's House's Door".
			JHHDoor is south of East Village Centre and north of JosephHopeHouse.
			JHHDoor is locked.

[ West Village ]
West Village is a region in The Village.

	[ West Village Centre ]
	West Village Centre is a room in West Village.
	West Village Centre is west of Village Centre.
	West of West Village Centre is The West Forest.

	[ My House]
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

	[ Village Tavern ]
	Village Tavern is a region in West Village.

		The Tavern Stable is a room in Village Tavern.
			The Tavern Door is a door.
				The Tavern Door is east of The Common Area and west of The Tavern Stable.
			The Tavern Stable is north of West Village Centre.

		MossMug is a region in Village Tavern.
			The printed name of MossMug is "Moss and Mug Tavern".
			The Common Area is a room in MossMug.
				The Tavern Back Door is a door.
					The Tavern Back Door is north of The Common Area.
					North of The Tavern Back Door is The North Forest.
			The Taproom is a room in MossMug.
				The Taproom is west of The Common Area.
			The Lodge is a room in MossMug.
				The Lodge Door is a door.
					The Lodge Door is north of The Lodge and south of The Common Area.

[ North Village ]
North Village is a region in The Village.

	[ North Village Centre ]
	North Village Centre is a room in North Village.
	North Village Centre is north of Village Centre.
	North of North Village Centre is The North Forest.
	
	[ Melia and Tom's house]
	MeliaTomHouse is a room in North Village.
		The printed name of MeliaTomHouse is "Melia and Tom's House". 
		MTHDoor is a door.
			The printed name of MTHDoor is "Melia and Tom's House's Door".
			MTHDoor is west of North Village Centre and east of MeliaTomHouse.
			MTHDoor is locked.

	[ Wheat Field ]
	The Wheat Field is a room in North Village.
		The Wheat Field is east of North Village Centre.
		The Wheat Field is north of The Fishing House.
		East of The Wheat Field is The Lake.
		North of The Wheat Field is The North Forest.

[ South Village ]
South Village is a region in The Village.

	[ South Village Centre ]
	South Village Centre is a room in South Village.	
		South Village Centre is south of Village Centre.
		South of South Village Centre is The South Forest.
	
	[ Allen's House]
	AllenHouse is a room in South Village.
		The printed name of AllenHouse is "Allen's House".
		AHDoor is a door.
			The printed name of AHDoor is "Allen's House's Door".
			AHDoor is west of South Village Centre and east of AllenHouse.
			AHDoor is locked.
	
	[ Village Shoppe ]
	The Village Shoppe is a room in South Village.
		The printed name of The Village Shoppe is "The Common Shelf".
		VSDoor is a door.
			The printed name of VSDoor is "The Common Shelf's Door."
			VSDoor is east of South Village Centre and west of The Village Shoppe.
	
[ Persons ]
The player is Rowan.
	Rowan is a person.
	Rowan is on My Bed.