extends Node2D
onready var PM = preload("res://scene/PointMass.tscn")
onready var Link = preload("res://scene/Link.tscn")
var PM_list = []
var link_list = [] # Not a linked list lol
var spawn_offset = Vector2(600,100)
var PM_spacing = 10
var grid_size = 10

# Called when the node enters the scene tree for the first time.
func _ready():
	randomize()
	spawn_PM()
	link_PM()


func spawn_PM():
	for i in grid_size:
		PM_list.append([])
		for j in grid_size:
			var new_PM = PM.instance()
			new_PM.name = "PointMass" + str(i) + str(j)
			new_PM.position = spawn_offset + Vector2(i,j)*PM_spacing
			PM_list[i].append(new_PM)
			add_child(new_PM)

func link_PM():
	assert(PM_list.size()>0)
	for i in PM_list.size():
		for j in PM_list.size()-1:
			var new_link = Link.instance() #horizontal
			new_link.PM_a = PM_list[i][j]
			new_link.PM_b = PM_list[i][j+1]
			new_link.resting_distance = PM_spacing
			link_list.append(new_link)
			add_child(new_link)
			
	for i in PM_list.size()-1:
		for j in PM_list.size():
			var new_link = Link.instance()
			new_link.PM_a = PM_list[i][j]
			new_link.PM_b = PM_list[i+1][j]
			new_link.resting_distance = PM_spacing
			link_list.append(new_link)
			add_child(new_link)

var count = 0
func _physics_process(delta):
	count+=1;
	if count % 3 == 0:
		suck_to_mouse(delta)
		for i in link_list.size():
			link_list[i].constrain()
			link_list[i].constrain()
			link_list[i].constrain()
		for i in PM_list.size():
			for j in PM_list.size():
				PM_list[i][j].do_verlet(delta)
	

func suck_to_mouse(delta):
	var mousepos = get_viewport().get_mouse_position()
	PM_list[0][0].position = PM_list[0][0].position.linear_interpolate(mousepos, delta * 80)
	
	PM_list[grid_size-1][0].position = PM_list[0][0].position + Vector2((grid_size+1)*PM_spacing,0)

