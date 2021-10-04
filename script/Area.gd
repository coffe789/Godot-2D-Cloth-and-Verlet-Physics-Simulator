extends Node2D
onready var particle_scene = preload("res://scene/Particle.tscn")
onready var spring_scene = preload("res://scene/DampedSpringJoint2D.tscn")
var grid_offset = Vector2(100,100)

var space_between = 10
var particle_list = []
var spring_list = []

func _ready():
	define_particles()
	define_springs()
	for i in spring_list.size(): #attach springs to particles
		spring_list[i].set_node_a(particle_list[i].get_path())
		spring_list[i].set_node_b(particle_list[i+1].get_path())


# Creates particles in a grid, stores them in an array
func define_particles():
	for i in 40:
		var particle_instance = particle_scene.instance()
		particle_instance.position = grid_offset + Vector2((i*space_between),0)
		particle_instance.name = "particle" + str(i)
		particle_list.append(particle_instance)
		add_child(particle_list[i])

func define_springs():
	for i in 40-1:
		var spring_instance = spring_scene.instance()
		spring_list.append(spring_instance)
		particle_list[i].add_child(spring_list[i])
		

var t = 0
func _physics_process(delta):
	t+=delta
	var mousepos = get_viewport().get_mouse_position()
	particle_list[0].position = particle_list[0].position.linear_interpolate(mousepos, delta * 20)
	

	#particle_list[0].position = grid_offset + Vector2((0*space_between),0)


func sinwave(t):
	for i in 40:
		particle_list[i].position.y = 50 + 30*sin(particle_list[i].position.x/20 - 30*t)

func apply_gravity(node,max_speed,delta_acc):
	node.velocity.y = approach(node.velocity.y, max_speed, delta_acc)

func approach(to_change, maximum, change_by):
	assert(change_by>=0)
	var approach_direction = 0;
	if (maximum > to_change):
		approach_direction = 1
	elif (maximum < to_change):
		approach_direction = -1
	to_change += change_by * approach_direction;
	if (approach_direction == -1 && to_change < maximum):
		to_change = maximum
	elif (approach_direction == 1 && to_change > maximum):
		to_change = maximum
	return to_change
