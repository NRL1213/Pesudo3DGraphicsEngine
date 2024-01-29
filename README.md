# LimGE
## A C# implmentation of a graphics engine


### A Sudo 3D graphics engine
Graphics are represented as a 2d Array that is a top down view on the 3d view
A projection is drawn outwards and when it collides with a wall it genrates a struct full of collision info
These structs are combined into a list and thrown into a renderer which turns info into scaling for distance and colors etc...
These render objects are then placed on on a queue
This queue is then put onto the screen and cleared after every frame
