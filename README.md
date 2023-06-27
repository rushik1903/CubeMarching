still working on GPU version

# CubeMarching

{a small prototype=> unzip > run cubeMarching . controls [WASD, mouse]}

https://drive.google.com/drive/folders/18gqevYUB-05uW0rUMyhJIgLhWqKmeWoC

Terrain generation with cube marching in unity

cubemarching is widely known for creating 3d surfaces from 3d scalar fields.
Input is a grid of points with different weights assigned to them 
Output is a mesh generated using these weights
The best part is this generated mesh is complete and will not have any holes in it.

we can generate a world using this mesh genration tool
The difference in the input data can really give some unique worlds

In this project I implemented cubemarching using both CPU and GPU

CPU - more cubeMarching time - less mesh render time
GPU - less cubemarching time - more mesh render time

This is because in GPU there are vertices that repeat multiple times.
We can eliminate this in CPU as as it is single threaded.
Still working on multi threading cpu's.
