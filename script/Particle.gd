extends RigidBody2D
var velocity = Vector2(0,0);

func _ready():
	pass 



func _physics_process(delta):
	position += velocity
