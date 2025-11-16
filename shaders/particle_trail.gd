@tool
extends Node2D

# For GPUParticles2D setup
func _ready():
	if get_node("../RedTrail"):
		setup_trail(get_node("../RedTrail"), Color(1, 0.4, 0), Vector2(0, 0), Vector2(1280, 720))
	if get_node("../WhiteTrail"):
		setup_trail(get_node("../WhiteTrail"), Color(1, 1, 1), Vector2(1280, 0), Vector2(0, 720))

func setup_trail(particles, color, start, end):
	particles.emitting = true
	particles.process_material = preload("res://shaders/particle_material.tres")
	particles.draw_order = GPUParticles2D.DRAW_ORDER_LIFETIME
	particles.one_shot = true
	particles.speed_scale = 0.8
	particles.amount = 500
	particles.lifetime = 5.0
	particles.gravity = Vector2(0, 0)
	particles.explosiveness = 0.0
	particles.direction = (end - start).normalized()
	particles.spread = 10.0
	particles.color = color
	particles.scale = 0.5
	particles.emission_shape = GPUParticles2D.EMISSION_SHAPE_DIRECTED_POINTS
	particles.emission_points = [start, end]
	particles.emission_point_count = 2
	particles.restart()

# Optional: Grid Shader Setup
func _process(delta):
	pass