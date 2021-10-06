extends Node2D


var resting_distance = 30
var PM_a #Point Mass
var PM_b

# Called when the node enters the scene tree for the first time.
func _ready():
	pass

# Distance between coordinate vectors
func find_distance(a,b):
	return sqrt(pow(a.x-b.x,2)+pow(a.y-b.y,2))

func constrain():
	var distance = find_distance(PM_a.position,PM_b.position)
	var distance_ratio
	if distance !=0:	#prevent division by zero
		distance_ratio = (resting_distance - distance) / distance
	else:
		distance_ratio = 1000
	
	# Move points toward each other, proportionally to how far away they are
	var translate_by = (PM_a.position - PM_b.position) * 0.5 * distance_ratio
	PM_a.position += translate_by
	PM_b.position -= translate_by

func _draw():
	draw_line(PM_a.position, PM_b.position, Color(1.0, 1.0, 1.0) , 2, false)

func _physics_process(delta):
	update()
