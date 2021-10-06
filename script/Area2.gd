extends Node2D
onready var PM = preload("res://scene/PointMass.tscn")
onready var Link = preload("res://scene/Link.tscn")
var PM_list = []
var link_list = [] # Not a linked list lol
var spawn_offset = Vector2(600,100)
var PM_spacing = 30

# Called when the node enters the scene tree for the first time.
func _ready():
	randomize()
	spawn_PM()
	link_PM()


func spawn_PM():
	for i in 5:
		var new_PM = PM.instance()
		new_PM.name = "PointMass" + str(i)
		new_PM.position = spawn_offset + Vector2((i*PM_spacing),rand_range(-10,10))
		PM_list.append(new_PM)
		add_child(new_PM)

func link_PM():
	assert(PM_list.size()>0)
	for i in PM_list.size()-1:
		var new_link = Link.instance()
		new_link.PM_a = PM_list[i]
		new_link.PM_b = PM_list[i+1]
		link_list.append(new_link)
		add_child(new_link)

func _physics_process(delta):
	suck_to_mouse(delta)
	for i in link_list.size():
		link_list[i].constrain()
		link_list[i].constrain()
		link_list[i].constrain()
	for i in PM_list.size():
		PM_list[i].do_verlet(delta)
	

func suck_to_mouse(delta):
	var mousepos = get_viewport().get_mouse_position()
	PM_list[0].position = PM_list[0].position.linear_interpolate(mousepos, delta * 40)

