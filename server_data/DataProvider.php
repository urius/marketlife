<?php
error_reporting(E_ALL & ~E_NOTICE);
//error_reporting  (E_ALL);
ini_set ('display_errors', true);

$command = $_GET["command"];
$id = $_GET["id"];
$ids = $_GET["ids"];
$name = $_GET["name"];
$picture_url = $_GET["picture_url"];
$post_data = $_POST["data"];
$post_data_hash = $_POST["hash"];
$error = null;

if ($id != null && strlen($id) > 0) {	
	validate_id_or_abort($id);
}

switch ($command) {
	case 'echo':
		$data_to_show = array('data' => $_POST["data"], "hash" => $_POST["hash"], "is_valid" => md5($_POST["data"]) == $_POST["hash"], "calculated_hash" => md5($_POST["data"]));
		show_response(json_encode($data_to_show), null);
		break;
	case 'test':
		echo "test";
		break;
	case "get_time":
		show_response(time(), null);
		break;
	case 'get_data':
		$mysqli = init_connection();
		$player_data = get_or_create_player_data($mysqli, $id, $error);
		show_response(json_encode($player_data), $error);
		break;
	case 'get_users_last_visit':
		validate_ids_or_abort($ids);
		$mysqli = init_connection();
		$ids_array = explode(',', $ids);
		$visit_times = get_visit_times($mysqli, $ids_array, $error);
		show_response(json_encode($visit_times), $error);
		break; 
	case 'get_leaderboards':
		$mysqli = init_connection();
		$leaderboard_data = get_leaderboards_data($mysqli, $error);
		show_response(json_encode($leaderboard_data), $error);
		break;
	case 'save_data':
		$mysqli = init_connection();
		$save_result = false;
		if (validate_post($post_data, $post_data_hash, $error)) {
			$save_result = save_data($mysqli, $id, $post_data, $error);
		}
		$response_str = json_encode(array('success' => $save_result, "saved_data" => $post_data));
		show_response($response_str, $error);
		break;
	case 'save_external_data':
		$mysqli = init_connection();
		$save_result = false;
		if (validate_post($post_data, $post_data_hash, $error)) {
			$save_result = save_external_data($mysqli, $id, $post_data, $error);
		}
		show_is_success_response($save_result, $error);
		break;
	case 'set_user_info':
		$mysqli = init_connection();
		$save_result = false;
		if ($id != null && $name != null && $picture_url != null) {
			$save_result = save_user_info($mysqli, $id, $name, $picture_url, $error);
		} else {
			fill_error($error, "ERR_MISSING_PARAMS", "Missing required parameters: id, name, or picture_url");
		}
		show_is_success_response($save_result, $error);
		break;
	case 'get_users_info':
		$mysqli = init_connection();
		if (validate_post($post_data, $post_data_hash, $error)) {
			$uids = explode(',', $post_data);
			$users_info = get_users_info($mysqli, $uids, $error);
			show_response(json_encode($users_info), $error);
		}
		break;
	case 'save_leaderboard_data':
		$mysqli = init_connection();
		$save_result = false;
		if (validate_post($post_data, $post_data_hash, $error)) {
			$save_result = save_leaderboard_data($mysqli, $id, $post_data, $error);
		}
		$response_str = json_encode(array('success' => $save_result, "saved_data" => $post_data));
		show_response($response_str, $error);
		break;
	default:
		echo "working...";
		break;
}

function validate_id_or_abort($id) {
	return;
	
	$match_result = preg_match('/\w+/', $id, $matches);
	if ($match_result == true) {
		$match_result = (strlen($matches[0]) == strlen($id));
	}

	if($match_result == false) {
		abort_with_error($error, "BAD_PARAM", "Wrong input parameter: id");
	}
}

function validate_ids_or_abort($ids) {
	return;

	$match_result = preg_match('/(?:,?\w+)+/', $ids, $matches);
	if ($match_result == true) {
		$match_result = (strlen($matches[0]) == strlen($ids));
	}

	if($match_result == false) {
		abort_with_error($error, "BAD_PARAM", "Wrong input parameter: ids");
	}
}

function validate_post($post_data, $post_data_hash, &$error) {
	if ($post_data != null && strlen($post_data) > 0 && $post_data_hash != null && strlen($post_data_hash) > 0) {
		$hash = md5($post_data);
		if(strlen($hash) == strlen($post_data_hash) && $hash == $post_data_hash) {
			return true;
		} else {
			fill_error($error, "ERR_VALIDATION_INCORRECT_HASH", "Wrong hash given on request (or corrupted data)");
		}
	} else {		
		fill_error($error, "ERR_VALIDATION_EMPTY_DATA", "Empty data or hash is given, data: $post_data hash: $post_data_hash");
	}

	return false;;
}

function save_data($mysqli, $id, $data_str, &$error) {
	$timestmap = time();
	$query_str = "UPDATE players
			   	 SET 
			   	 	 days_play = days_play + (FROM_UNIXTIME($timestmap, '%Y-%m-%d') != FROM_UNIXTIME(last_visit_time, '%Y-%m-%d')),
				 	 last_visit_time = $timestmap,
				 	 data = JSON_MERGE_PATCH(data, '$data_str')
				 WHERE uid = '$id'";

	$sql_result = mysqli_query($mysqli, $query_str);
	if ($sql_result) {
		return true;
	} else {
		fill_error($error, "ERR_SAVE_USER_DATA", "Failed to save user data");
	}

	return false;
}

function save_leaderboard_data($mysqli, $id, $data_str, &$error) {
	$timestmap = time();
	$decoded_data = json_decode($data_str);
	$cash = intval($decoded_data->cash);
	$gold = intval($decoded_data->gold);
	$exp = intval($decoded_data->exp);
	$friends = intval($decoded_data->friends);
	$query_str = "INSERT INTO `leaderboard_data`(`uid`, `cash`, `exp`, `gold`, `friends`, `timestamp`) 
							VALUES ('$id',$cash,$exp,$gold,$friends,$timestmap)
							ON DUPLICATE KEY UPDATE cash=$cash, exp=$exp, gold=$gold, friends=$friends, timestamp=$timestmap";

	$sql_result = mysqli_query($mysqli, $query_str);
	if ($sql_result) {
		return true;
	} else {
		fill_error($error, "ERR_SAVE_LEADERBOARD_DATA", "Failed to save leaderboard data");
	}

	return false;
}

function save_external_data($mysqli, $id, $data_str, &$error) {
	$timestmap = time();
	$query_str = "UPDATE players
			   	 SET 
				 	 external_data = JSON_MERGE_PATCH(external_data, '$data_str')
				 WHERE uid = '$id'";

	$sql_result = mysqli_query($mysqli, $query_str);
	if ($sql_result) {
		return true;
	} else {
		fill_error($error, "ERR_SAVE_EXTARNAL_DATA", "Failed to save external data for user");
	}

	return false;
}

function save_user_info($mysqli, $id, $name, $picture_url, &$error) {
	$query_str = "INSERT INTO `players_info`(`id`, `name`, `picture_url`) 
				  VALUES ('$id', '$name', '$picture_url')
				  ON DUPLICATE KEY UPDATE name='$name', picture_url='$picture_url'";

	$sql_result = mysqli_query($mysqli, $query_str);
	if ($sql_result) {
		return true;
	} else {
		fill_error($error, "ERR_SAVE_USER_INFO", "Failed to save user info");
	}

	return false;
}

function get_or_create_player_data($mysqli, $id, &$error) {
	$get_data_result = get_player_data($mysqli, $id, $error);
	if ($error == null) {
		if($get_data_result == null) {
			$file_content = file_get_contents("GameConfigs/DefaultUser.json");
			$reencoded_default_user = json_encode(json_decode($file_content));
			$insert_query_result = mysqli_query($mysqli,"INSERT INTO `players`(`uid`, `data`, `external_data`, `first_visit_time`, `last_visit_time`, `days_play`, `gold_b`, `cash_b`) 
														 VALUES ('$id', '$reencoded_default_user','{}', UNIX_TIMESTAMP() , 0, 0, 0, 0)");
			if ($insert_query_result) {
				$get_data_result = get_player_data($mysqli, $id, $error);
			} else {
				fill_error($error, "ERR_CREATE_USER", "Failed to create user in db");
			}
		}
	}

	return $get_data_result;
}

function get_player_data($mysqli, $id, &$error) {
	$result = null;
	$sql_result = mysqli_query($mysqli,"SELECT players.uid, players.data, players.external_data, players.days_play, players.first_visit_time, players.last_visit_time
								FROM players
								WHERE players.uid = '$id'
								LIMIT 1");
	if($sql_result) {
		$fetched = mysqli_fetch_assoc($sql_result);
		if($fetched) {
			decode_values($fetched);
			$result = $fetched;
		}
		mysqli_free_result($sql_result);
	} else {
		fill_error($error, "ERR_SELECT_USER", "Failed to get user from db");
	}
	return $result;
}

function get_visit_times($mysqli, $ids_arr, &$error) {
	$result = null;
	$query_ids_str = "'".implode("','", $ids_arr)."'";
	$query_str = "SELECT players.uid, players.last_visit_time
								FROM players
								WHERE players.uid IN ($query_ids_str)";
	$sql_result = mysqli_query($mysqli, $query_str);
	if($sql_result) {
		$result = array();
		while ($fetched = mysqli_fetch_assoc($sql_result)) {
			decode_values($fetched);
			$result[] = $fetched;
		}
		mysqli_free_result($sql_result);
	} else {
		fill_error($error, "ERR_SELECT_USERS", "Failed to get users from db");
	}
	return $result;
}

function get_leaderboards_data($mysqli, &$error) {
	$result = [];
	$timestamp_bound = time() - 7 * 24 * 3600;
	$temp_lb = get_leaderboard_for($mysqli, "cash", $timestamp_bound);
	if ($temp_lb === null) {
		fill_error($error, "ERR_SELECT_LEADERBOARD_CASH", "Failed to get cash leadeboard from db");
		return null;
	}
	$result["cash"] = $temp_lb;

	$temp_lb = get_leaderboard_for($mysqli, "gold", $timestamp_bound);
	if ($temp_lb === null) {
		fill_error($error, "ERR_SELECT_LEADERBOARD_GOLD", "Failed to get gold leadeboard from db");
		return null;
	}
	$result["gold"] = $temp_lb;

	$temp_lb = get_leaderboard_for($mysqli, "exp", $timestamp_bound);
	if ($temp_lb === null) {
		fill_error($error, "ERR_SELECT_LEADERBOARD_EXP", "Failed to get exp leadeboard from db");
		return null;
	}
	$result["exp"] = $temp_lb;

	$temp_lb = get_leaderboard_for($mysqli, "friends", $timestamp_bound);
	if ($temp_lb === null) {
		fill_error($error, "ERR_SELECT_LEADERBOARD_FRIENDS", "Failed to get friends leadeboard from db");
		return null;
	}
	$result["friends"] = $temp_lb;
	return $result;
}

function get_leaderboard_for($mysqli, $type_str, $timestamp_bound) {
	$query_str = "SELECT uid,$type_str as value FROM `leaderboard_data` WHERE timestamp > $timestamp_bound AND $type_str >= 0 ORDER BY $type_str DESC LIMIT 100";
	$sql_result = mysqli_query($mysqli, $query_str);

	if ($sql_result) {
		$result = array();
		while ($fetched = mysqli_fetch_assoc($sql_result)) {
			decode_values($fetched);
			$result[] = $fetched["uid"].":".$fetched["value"];
		}
		mysqli_free_result($sql_result);

		return $result;
	}

	return null;
}

function decode_values(&$values_arr) {
	foreach ($values_arr as $key => $value) {
		$decoded_value = json_decode($value);
		$values_arr[$key] = $decoded_value === null ? $value : $decoded_value;
	}
}

// SET @timestamp = UNIX_TIMESTAMP();
// UPDATE `players` 
// SET 
//     `days_play` = days_play  + (FROM_UNIXTIME(@timestamp, "%Y-%m-%d") != FROM_UNIXTIME(last_visit_time, "%Y-%m-%d")),
//     `last_visit_time`= @timestamp
// WHERE uid = 't'

function init_connection() {
	$mysqli = new mysqli("127.0.0.1", "i116296_script_cg", "kjR6aqeP1", "i116296_market_crazy_games", 3306);
	if ($mysqli->connect_errno) {
	    die ( '{"response":"-1", "reason":"Could not connect to Database: (' . $mysqli->connect_errno . ')' . $mysqli->connect_error );
	};
	return $mysqli;
}

function abort_with_error(&$error, $code, $msg) {
	fill_error($error, "BAD_PARAM", "Wrong input parameter: id");
	show_response(null, $error);
	die();
}

function fill_error(&$error, $code, $msg)
{
    $error = array("code" => $code, "message" => $msg);
}

function show_response($response_str, $error) {
	$response = array('response' => $response_str, 'hash' => md5($response_str), "v" => 1);
	if($error != null) {
		$response['error'] = json_encode($error);
	}
	echo json_encode($response);
}

function show_is_success_response($is_success, $error) {
	$response_str = json_encode(array('success' => $is_success));
	show_response($response_str, $error);
}

function get_users_info($mysqli, $uids, &$error) {
	$result = array();
	$query_ids_str = "'".implode("','", $uids)."'";
	$query_str = "SELECT id, name, picture_url 
				  FROM players_info 
				  WHERE id IN ($query_ids_str)";
	
	$sql_result = mysqli_query($mysqli, $query_str);
	if($sql_result) {
		while ($fetched = mysqli_fetch_assoc($sql_result)) {
			$result[] = $fetched;
		}
		mysqli_free_result($sql_result);
	} else {
		fill_error($error, "ERR_SELECT_USERS_INFO", "Failed to get users info from db");
	}
	return array("users" => $result);
}

?>
