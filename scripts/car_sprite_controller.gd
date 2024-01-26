extends Node2D


func _ready():
	$Top/Engine.play("default")
	$Fills/TireFrontLeft.play("default")
	$Fills/TireFrontRight.play("default")
	$Fills/TireBackLeft.play("default")
	$Fills/TireBackRight.play("default")
	set_zone_damaged(0,false)
	set_zone_damaged(1,false)
	set_zone_damaged(2,false)
	set_zone_damaged(3,false)
	set_zone_damaged(4,false)
	set_zone_damaged(5,false)



## angle in degrees, 0 deg = wheels straight forward
## a positive angle is a right turn relative to the front of the car
## a negavtive angle is a left turn relative to the front of the car
func set_wheel_angle(angle: float) -> void:
	$Outlines/TireFrontLeft.set_rotation_degrees(angle)
	$Outlines/TireFrontRight.set_rotation_degrees(angle)
	$Fills/TireFrontLeft.set_rotation_degrees(angle)
	$Fills/TireFrontRight.set_rotation_degrees(angle)

func is_zone_damaged(zone_index: int) -> bool:
	match zone_index:
			0: #front right
				return $Fills/FrontRight.frame_coords.y == 0
			1: #front left
				return $Fills/FrontLeft.frame_coords.y == 0
			2: #side right
				return $Fills/SideRight.frame_coords.y == 0
			3: #side left
				return $Fills/SideLeft.frame_coords.y == 0
			4: #back right
				return $Fills/BackRight.frame_coords.y == 0
			5: #back left
				return $Fills/BackLeft.frame_coords.y == 0
	return false

func set_zone_damaged(zone_index: int, damaged: bool) -> void:
	var frame = 0 if damaged else 1
	match zone_index:
		0: #front right
			$Outlines/FrontRight.frame_coords.y = frame
			$Fills/FrontRight.frame_coords.y = frame
		1: #front left
			$Outlines/FrontLeft.frame_coords.y = frame
			$Fills/FrontLeft.frame_coords.y = frame
		2: #side right
			$Outlines/SideRight.frame_coords.y = frame
			$Fills/SideRight.frame_coords.y = frame
		3: #side left
			$Outlines/SideLeft.frame_coords.y = frame
			$Fills/SideLeft.frame_coords.y = frame
		4: #back right
			$Outlines/BackRight.frame_coords.y = frame
			$Fills/BackRight.frame_coords.y = frame
		5: #back left
			$Outlines/BackLeft.frame_coords.y = frame
			$Fills/BackLeft.frame_coords.y = frame

func set_car_color_index(color_index: int) -> void:
	$Fills/FrontRight.frame_coords.x = color_index
	$Fills/FrontLeft.frame_coords.x = color_index
	$Fills/SideRight.frame_coords.x = color_index
	$Fills/SideLeft.frame_coords.x = color_index
	$Fills/BackRight.frame_coords.x = color_index
	$Fills/BackLeft.frame_coords.x = color_index

func set_car_speed_scale(speed_scale: float) -> void:
	$Fills/TireFrontLeft.set_speed_scale(speed_scale)
	$Fills/TireFrontRight.set_speed_scale(speed_scale)
	$Fills/TireBackLeft.set_speed_scale(speed_scale)
	$Fills/TireBackRight.set_speed_scale(speed_scale)
	$Top/Engine.set_speed_scale(speed_scale)

func set_engine_scale(s: float) -> void:
	pass
