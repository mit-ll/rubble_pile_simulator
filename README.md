# SPROUT Sim

Unity simulation framework for validating SPROUT mapping capability

## Requirements

Unity 2022.3.62f2

Unity ROS TCP Connector
```
https://github.com/Unity-Technologies/ROS-TCP-Connector.git?path=/com.unity.robotics.ros-tcp-connector
```
URP Fog (urp 14)
```
https://github.com/meryuhi/URPFog/tree/urp14
```

Optional: Unity ROS URDF importer
```
https://github.com/Unity-Technologies/URDF-Importer.git?path=/com.unity.robotics.urdf-importer
```
ROS Melodic
```
https://wiki.ros.org/melodic/Installation/Ubuntu
```
Our DS5_ROS fork:
```
https://github.com/SPROUT-MITLL/ds5_ros
```


## Command line args
### Example usage
``` 
rubble_pile.exe -rosip 127.0.0.1 -rosport 10000 -spawnposy 25 -numlayers 4
```
### Editor Debug
To test command line arguments in editor you can add them to the text file located at: 
```
Assets/MITLL/Debug/DebugArgs.txt
```
### Ros args
* randomseed : (int) | 0 Sets the random seed for the environment
* rosip : (int) | 127.0.0.1 Sets the ros IP
* rosport : (int) | 10000 Sets the ros port

### Spawn args
#### Spawn Volume
position controls the center position of the spawn volume, scale controls the volume size from the center
* spawnposx : (float) | 0 sets the x position
* spawnposy : (float) | 10 sets the y position
* spawnposz : (float) | 0 sets the z position
* spawnboundx : (float) | 10 sets the x scale
* spawnboundy : (float) | 10 sets the y scale
* spawnboundz : (float) | 10 sets the z scale
* numlayers : (int) | 3 sets the number of layers that spawn in the pile
* numobjs : (int) | sets the number of objects per layer that spawn
* exportstl : (bool) | 1 enables export of pile stl

> [!IMPORTANT]  
> The `exportstl` is disabled for the provided release binaries due to licensing restrictions. To use the export STL function, you will need to build the project in Unity.

### Light args
* setlightrot : (bool) | 1 enables setting of light rotation otherwise it is random
* lighttype : (int) | 0 spot, 1 directional, 2 point
* lightintensity : (float) | sets intensity of light
* lightrotx : (float) | sets light x euler angle
* lightroty : (float) | sets light y euler angle
* lightrotz : (float) | sets light z euler angle
* lightposx : (float) | sets light x position
* lightposy : (float) | sets light y position
* lightposz : (float) | sets light z position
### Fog args
* fogdensity : (float) | sets the fog density
* foginstensity : (float) | sets the fog intensity

If you are using the Unity Editor there is a workaround for testing command line arguments. There is a **DebugArgs** file found in **Assets > MITLL > Debug** that you can change or add command line arguments.

## Controls
The robot camera can be controled through various means. 
### Supported Controls

* PS5 controller connected through ROS melodic
* Game controller connected to the host machine
* Keyboard/Mouse 

### PS5/Game controller defaults
- Left Stick - Rotate View
- Right stick - Grow, Retract
- Share/Select - Toggle Light
- L1/R1 - Change light intensity
- Start - Reset Pile
- Triangle - Reset position
- Touchpad Press - Display controls

### Keyboard/Mouse Defaults 
- W/S - Forward / Back
- Mouse - Rotate View
- R - reset position 
- P - regenerate pile
- L - Toggle light
- [/] - Change light intensity
- H - Display controls

## Debris Spawning
Debris is spawned in the environment based on a library that contains item scriptable objects that contain connections to prefabs and weights. This allows you to control the frequency of the objects. 

To do this you need to import 3D models or other assets and use those for the prefabs. We have provided a basic set as an example.
These prefabs need to have some kind of collider and rigidbody component.

### Generate Scriptable Objects
![Alt text](imgs/itembuilder_01.png?raw=true "Item Builder")

Using the ItemBuilder prefab in the Unity Editor you can create a list of objects and a list of weights. You can use this to generate a set of WeightedItems which you can bundle together in a library of weighted items.

* CSV Text Asset: A csv with a list of prefab names and weights that control spawning frequency for the rubble pile.
* If there is no CSV present it will generate a scriptable object based on the lists above, make sure to assign a list of weights as well.
* Prefab Path: The path of the prefabs to load when generating the weighted items scriptable objects.
* Collection Name: The name of the weighted item collection.
* Items Path: Where to save the created weighted items.

The CSV is formatted like below:
|object|weight|
|------|------|
|prefab_01|1|
|prefab_02|0.4|

### Assign library to debris spawner
![Alt text](imgs/debris_spawner_01.png?raw=true "Debris Spawner")

You can then assign the library to the debris spawner to create your very own custom rubble pile!
A component of the Managers gameobject in the MainScene

This generates a pile by spawning smaller objects first and then a series of larger objects to create a rubble pile with a more natural distribution of object sizes.

* Debris Collection: The main collection of objects to spawn.
* Small Debris Collection: The collection of smaller objects that is spawned first to ensure there are some smaller objects at the bottom of the pile.
* Spawn Volume: If none a volume is created a little above origin.
* Spawn Delay: How long to settle between spawns.
* Num Piles: Number of times the Debris Collection is spawned to create the pile.

### Export STL
When using the command line argument exportstl
```
  -exportstl 1
```
It exports a STL file of the rubble pile to this directory: 
```
AppData\LocalLow\MIT Lincoln Laboratory\RubbleSim\SceneData
```
# Distribution Statement

DISTRIBUTION STATEMENT A. Approved for public release. Distribution is unlimited.
 
This material is based upon work supported by the Department of the Air Force under Air Force Contract No. FA8702-15-D-0001. Any opinions, findings, conclusions or recommendations expressed in this material are those of the author(s) and do not necessarily reflect the views of the Department of the Air Force.
 
© 2024 Massachusetts Institute of Technology.
Subject to FAR52.227-11 Patent Rights - Ownership by the contractor (May 2014)
 
The software/firmware is provided to you on an As-Is basis
 
Delivered to the U.S. Government with Unlimited Rights, as defined in DFARS Part 252.227-7013 or 7014 (Feb 2014). Notwithstanding any copyright notice, U.S. Government rights in this work are defined by DFARS 252.227-7013 or DFARS 252.227-7014 as detailed above. Use of this work other than as specifically authorized by the U.S. Government may violate any copyrights that exist in this work.
