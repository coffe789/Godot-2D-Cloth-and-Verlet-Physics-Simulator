extends Node2D
onready var area = get_node("Area2D")
var velocity = Vector2(0,0)
var acceleration = Vector2(0,100) #gravity
var entered_body = false;

# Called when the node enters the scene tree for the first time.
func _ready():
	pass#area.set_monitorable(false)


onready var last_position = position
var next_position
var current_time = 0
func _process(delta):
	pass#do_verlet(delta)



func do_verlet(delta):
	var mult = 1
	velocity = position - last_position
	last_position = position
	#if (entered_body && velocity.x > 0):
	#	velocity.x *= -1
	#	mult = 0
	if (entered_body && velocity.y > 0):
		velocity.y *= -1
		mult = 0

	position = position + (velocity*delta*60) + (acceleration*delta * mult)


func get_trig_c(a):
	return sqrt(pow(a.x,2)+pow(a.y,2))


func _on_Area2D_body_entered(body):
	entered_body=true;
	print("in")
	


func _on_Area2D_body_exited(body):
	entered_body = false
