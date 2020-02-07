# XMLtoUE4JsonDataTable-Dialogue
Converts yEd's .graphml xml files to UE4-importable Json

## In yEd Graph Editor:
![Example](example.png)

## In the program:
![Example2](example2.png)

  You drag-drop files from explorer
  
  Doubleclick to remove entry

## Result:
![Example3](example3.png)

  The converted Json can be imported to ue4 (drag-drop) as a data-table asset of sDialogue structure
  
## The sDialogue and sResponse structures:
![Example4](example4.png)

## Explicit start functionality

  If you make your start node a down-pointing triangle, the program will try to make that node the first statement in the json file and also switch its name to n*::n0 where n* is its original group id
  
  This is useful if for some ungodly reason yEd decides that it wants to shuffle the order of nodes or if you didn't start building your graph with the root
