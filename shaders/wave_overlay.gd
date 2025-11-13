extends CanvasLayer

# Parameters for wave animation
var wave_speed = 0.1
var wave_amplitude = 20
var wave_frequency = 5
var wave_color = Color(1, 1, 1, 0.5)  # White with transparency

# Parameters for drawing
var border_margin = 10
var line_thickness = 2

func _ready():
    set_process(true)

func _process(delta):
    update()

func _draw():
    var time = OS.get_ticks_msec() / 1000.0 * wave_speed
    var width = get_viewport_rect().size.x - border_margin * 2
    var height = get_viewport_rect().size.y - border_margin * 2

    # Draw waves along the border
    for i in range(border_margin, width + border_margin, 10):
        var y_offset = sin(i / wave_frequency + time) * wave_amplitude
        draw_line(Vector2(i, border_margin), Vector2(i, border_margin + y_offset), wave_color, line_thickness)

    for i in range(border_margin, height + border_margin, 10):
        var x_offset = sin(i / wave_frequency + time) * wave_amplitude
        draw_line(Vector2(border_margin, i), Vector2(border_margin + x_offset, i), wave_color, line_thickness)