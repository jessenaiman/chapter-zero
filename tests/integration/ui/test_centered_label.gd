extends GdUnitTestSuite

func test_label_is_centered():
    var runner = scene_runner("res://source/ui/menus/test_centered_label.tscn")
    var root = runner.root()
    var label = root.get_node("Label")
    var viewport_size = root.get_viewport_rect().size
    var label_rect = label.get_global_rect()
    var viewport_center = viewport_size / 2
    var label_center = label_rect.position + label_rect.size / 2
    var distance = (label_center - viewport_center).length()
    assert_float(distance).is_less_or_equal(5.0, "Label is not centered. Actual: %s, Expected: %s" % [label_center, viewport_center])
