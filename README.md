# Godot-2D-Cloth-and-Verlet-Physics-Simulator
This is a customisable 2D cloth, simulated using verlet integration physics that can be added to a game in the Godot engine.
I have also added support for creating other objects that obey verlet physics.

Usage:
Download the project and import the scenes and scripts appropriately. Delete the static bodies in Area, as they are just there for demonstration.
Place area in the root of your scene. Read the documentation and at how I spawn the cloth in spawn_PM() & link_PM() to figure out how to use it for your purposes.


  Supported Customisability In Area:
	
    - grid_size: determines the number of point masses on the cloth
		
    - PM_spacing: determines the space between each point mass
		
  Supported Customisability In Link:
	
    Note that a link can connect any two Godot nodes, though only my point masses will experience verlet physics (have continued momentum).
		
    - color: determines the colour of the link
		
    - is_rigid: if a link is rigid it will resist compression. The cloth links do not resist compression, but this will allow you to create rigid objects
		
    - is_PM_a_pin/is_PM_b_pin: A pin node will not be pulled by a link. By default any node you connect to a link will be a pin. This can be overriden by adding a variable to your node called 'is_pin'
   
  Supported Customisability In PointMass:
   
	 - acceleration: "accelerates" the mass constantly in this direction. By default this is used as gravity, but it could also implement wind and other forces.
   
	 - dampen_factor: multiplies velocity by this factor each frame
  
	- collision_factor: This is a value I had to tweak to make collision feel right. Increase it and the mass will become more bouncy and experience more friction. If it is set too low, high speed masses will stick to walls instead of bouncing.



  Performance:
  
    - I don't recommend you make a cloth much bigger than 7x7 if you want a consistent 60fps on low-end machines. This should be plenty enough imo, you can still make the links longer if you want a bigger cloth.
	
    - You can improve performance by quite a bit by making the constrain() function run less times per frame. By default it runs 3 times per frame. In short, running it several times stops the cloth from 'shivering.' The shivering isn't super noticeable so it wouldn't hurt to reduce this.
	
    - You could also just lower the rate at which Area updates its physics, which in theory would make it twice as performant.
    
  TODO & Contributions:
	
    I will likely add support for texturing the cloth, and I may add support for adjusting the rigidity of the links.
    Feel free to make suggestions, or add your own features to this repo.
