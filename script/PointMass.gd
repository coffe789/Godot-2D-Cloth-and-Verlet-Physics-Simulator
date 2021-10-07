extends Node2D
onready var area = get_node("Area2D")
var velocity = Vector2(0,0)
var acceleration = Vector2(0,15) #gravity or other external forces
var entered_body = false;
var is_pin = false #Link checks if this is true

onready var last_position = position
var next_position

func do_verlet(delta):
	var mult = 1
	velocity = position - last_position
	last_position = position
	if (entered_body): #collision
		mult = 0
		position = last_position
		velocity *= -0.7
	velocity *= 0.98 # damping
	position = position + (velocity*delta*60) + (acceleration*delta * mult)

func _on_Area2D_body_entered(_body):
	entered_body=true;

func _on_Area2D_body_exited(_body):
	entered_body = false
