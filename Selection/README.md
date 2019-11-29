# Selection

Features:
-Support for single selection
-Support for drag selection
-Support for removing items while selection occures
-Support for button interaction

To use this:

Put the script "Selection_Behviour" on the object that you want to select.
- this contains a variable "DragSelection", for selection from selection box. false if not wanted

Put the script "User_Script" on an empty gameobject for system code
Notes:
- Objects in "All_Selectable" list allows user to select these objects
- "Selecting_Objects" are objects selected before release the selection box
- "Selected_Objects" are object that are selected
- When wanting to remove game objects, that are in "All_Selectable", call "Destroying_Objects(remove this game object)".
- "HideButtons()" can be called to hid all buttons
