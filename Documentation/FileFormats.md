# vivid/stasis file format documentation

## General info
- Both of the binary file formats used (VSD+VSB) are based on type-value encoding
- Data is split into 'chunks', with the type of each chunk indicated by a 1-byte type ID
- Some of these are shared between the two file types
- Strings are encoded as per GML spec (null terminated)

## Data types
While GML defines its own data types for buffer reading/writing, V/S often uses its own aliases for the types.
For reference, these can be found in the `SongFieldTypeHelper` class ([SongFieldType.cs](../VividTK.VSFormatLib/VSD/SongFieldType.cs)). Bear in mind that these names were all came up with by yours truly due to being unnamed constants in the code, so might not be fully accurate.

## Binary VSD files
While referred to as VSD files internally (e.g. `gml_GlobalScript_read_binary_vsd` and the file header), they don't use the `.vsd` file extension (using `.bin` instead). This extension is actually used for other miscellanious data, like the [banned user list](#banned-user-list) and the [menu portrait data](#menu-portrait-info).

### File header
The header is made up of a 3-byte magic number (`VSD`), a 1-byte version number, and a null byte.
Only version 1 has been used currently, and is implemented by the class `gml_GlobalScript_v1_vsd_reader`.

### Song list
The header is followed by any number of song info chunks, followed by an end-of-data marker byte.
See [ObjectType.cs](../VividTK.VSFormatLib/VSD/ObjectType.cs) for a list of the chunk types (and their IDs in hex) used in VSD files.

Each song info chunk contains a number of sub-chunks, which can either be chart info chunks or song field chunks (both also defined in the ObjectType enum).

Chart info chunks are simpler, and contain:
- The display text of the difficulty (string)
- The actual number value of the difficulty (float)
- The name of the note designer (string)

in that order. They always appear in ascending difficulty order, as the only thing that ties them to the chart they represent is their index in the file.

Song fields are formatted as:
- The data type of the field (see [Data types](#data-types))
- The ID of the field
- The data (of the above type; note that reading the data type isn't necessarily needed, as the field ID does imply the type of data you need to read)

The currently used song field IDs are:
- The ID of the song (uint32)
- The song's name (string)
- The name with any special formatting (string)
- The artist name (string)
- The ID of the chart (string); this is also the folder name containing the chart files
- The BPM to display in the menu (string)
- The version of the chart (string)
- Whether the song has an encore chart (bool)
- Whether the song is an original (bool)
- The name of the jacket artist (string)
- Whether the chart is released (bool); for obvious reasons this would only be true in public builds of the game*

\* the in-game dev tools also spit out a file called `song_information_dev.bin` that contains unpublished songs too, however they are automatically removed from the regular song info file. 😔

## Binary chart files
TODO

## Text chart files
Some charts also come with `.vsc` and `.vsm` files, both of which are text-based representations of the chart data.
These are built into [binary charts](#binary-chart-files) before release, and so don't serve a purpose for players.
The VSC files contain the note data, while VSM files contain modifier data. These are both combined into one chart binary, though.

## Chart stat files
Stat files (`.stats`) are INI files that contain some metadata about the song. Some of the fields include:
- The SHA1 checksum of the associated binary chart. This is used to regenerate the file if the chart changes.
- The count of different note types
- Note densities
- Other data

## Chart modifier files
VMV files are also used to store metadata about the modifiers in a chart.
They're INI files, with a few keys, but only `weight` seems to be loaded by the game.

## Miscellaneous text VSD files
There are a few other files used by the game that have the `.vsd` extension but are actually just various text-based formats. The two I know of currently are:

### Menu portrait info
Menu portraits are stored in a file called `menuports.vsd`, which is a TSV (tab separated value) file.
The fields are as follows:
- The name of the portrait
- The type of portrait
- The border ID
- The portrait description
- The name of the sprite to load
- The key that it uses in the save file

### Banned user list
The banned user list is another text file using the `.vsd` extension, which gets fetched from a server somewhere™ when the game loads, and contains a (newline delimited) list of steam user IDs that have been banned for one reason or another. I'm being more vague about this one for reasons that should be obvious, but I'm more invested in keeping this documentation comprehensive and accurate to not include it.
