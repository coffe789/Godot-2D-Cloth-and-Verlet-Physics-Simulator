extends Node2D


var resting_distance = 30 #default value, can be overriden when instanced
var PM_a #Point Mass
var PM_b
var is_PM_a_pin #A pinned node's position is not affected by the constraint
var is_PM_b_pin
var is_rigid = false #A rigid link has a minimum length
var color = Color(1.0, 1.0, 1.0)

# Called when the node enters the scene tree for the first time.
func _ready():
	if (PM_a.get("is_pin") == null):
		print("makin it true")
		is_PM_a_pin = true # True by default if not otherwise specified by node
	else:
		is_PM_a_pin = PM_a.is_pin # Can set value manually by giving your node an is_pin variable
	
	if (PM_b.get("is_pin") == null):
		is_PM_b_pin = true
	else:
		is_PM_b_pin = PM_b.is_pin

# Distance between coordinate vectors
func find_distance(a,b):
	return sqrt(pow(a.x-b.x,2)+pow(a.y-b.y,2))

func constrain():
	var distance = find_distance(PM_a.position,PM_b.position)
	var distance_ratio
	if distance !=0:	#prevent division by zero, which is basically impossible but hey
		distance_ratio = (resting_distance - distance) / distance
	else:
		distance_ratio = 1
	if distance_ratio <= 0 || is_rigid:
		# Move points proportionally to how far away they are
		var translate_by = (PM_a.position - PM_b.position) * 0.5 * distance_ratio
		if !is_PM_a_pin:
			PM_a.position += translate_by
		if !is_PM_b_pin:
				PM_b.position -= translate_by

func _draw():
	draw_line(PM_a.position, PM_b.position, color , 2, false)

func _process(_delta):
	update()
