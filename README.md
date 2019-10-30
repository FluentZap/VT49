# VT49
> Star Wars: Combat Simulator for Edge of the Empire

## The Project

I wanted to build a space flight simulator that combined my companion app 
<a href="https://github.com/FluentZap/Holocron">Holocron</a> with physical controls.
In a Tabletop RPG, players take on different character rolls. 
A player can play any combination of Pilot, Gunner, Mechanic, Navigator, Computer Slicer and many more.

Depending on the stats of the character I wanted the simulation to scale in difficulty.
If a player was playing a very skilled pilot, the flight controls would be more responsive 
and also allow special maneuvers that a rookie pilot would not have access to.

Project VT49 includes all the software for the space simulation and the physical interface on custom control boards.

## This is the framework for the data flow of the simulation 
<hr/>
<p align="center">
  <img width="70%" src="https://i.imgur.com/mxa1Tp2.png" />  
</p>
<hr/>

Six Arduinos connect to the Raspberry pi through serial.
I used <a href="https://en.wikipedia.org/wiki/Cyclic_redundancy_check">Cyclic Redundancy Check (CRC)</a> and 
<a href="https://en.wikipedia.org/wiki/Consistent_Overhead_Byte_Stuffing">Consistent Overhead Byte Stuffing (COBS)</a>
for packet framing.

The VT49 Viewers are built in Xenko Studio for a 3d display.
Physics are build in <a href="https://github.com/bepu/bepuphysics2">BepuPhysics2</a> a native C# physic engine.
All simulaiton calculations including physics are done in the VT49 control program and run on the Raspberry Pi 3+.

<a href="https://i.imgur.com/WMQGK5T.mp4">VTViewer Asteroid Test</a>

When the VT49 Viewer connects it preforms a full update to the client and then sends dynamic updates as needed.
The viewer application is isolated from the simulation. It reads and displays data, multiple viewers and viewpoints can be connected at the same time.
One could be placed in the front of the players and one on each side to give a full surround experience.

## Measure once cut twice

> Below is the final draft of my design diagram of what I wanted the consoles to look like.

<p align="center">  
  <img width="80%" src="https://i.imgur.com/MdAbxGi.png" />  
</p>

> This is the finished product

<p align="center">  
  <img width="80%" src="https://i.imgur.com/GsrREkd.jpg" />  
</p>

## Chips and connecting the whole thing

In Star Wars computers are accessed by round cylindrical keycards called Code Cylinders.
I still needed a way to identify the user on the Holocron database with the player at the table.
So got some EEPROM chips and hooked them up to the center Arduino Mega. 
They are all codded differently and when the correct packet is sent to the center console it will activate 
power to the Code Cylinder and read the code from the EEPROM.
This code corresponds to a unique username on the Holocron database and VT49 loads the data from it's SignalR connection.

<p align="center">
  <img width="42%" src="https://i.imgur.com/Nvtq99C.jpg" />  
  <img width="42%" src="https://i.imgur.com/jphha4h.jpg" />  
</p>
