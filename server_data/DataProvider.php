<?php
error_reporting(E_ALL & ~E_NOTICE);
//error_reporting  (E_ALL);
ini_set ('display_errors', true);

$command = $_GET["command"];
$id = $_GET["id"];
$post_data = $_POST["data"];
$post_data_hash = $_POST["hash"];
$error = null;

switch ($command) {
	case 'echo':
		$data_to_show = array('data' => $_POST["data"], "hash" => $_POST["hash"], "is_valid" => md5($_POST["data"]) == $_POST["hash"], "calculated_hash" => md5($_POST["data"]));
		show_response(json_encode($data_to_show), null);
		break;
	case "get_time":
		show_response(time(), null);
		break;
	case 'get_data':
		$mysqli = init_connection();
		$player_data = get_or_create_player_data($mysqli, $id, $error);
		show_response(json_encode($player_data), $error);
		break;
	case 'save_data':
		$mysqli = init_connection();
		$save_result = false;
		if (validate_post($error)) {
			$save_result = save_data($mysqli, $id, $post_data, $error);
		}
		show_is_success_response($save_result, $error);
		break;
	case 'save_external_data':
		$mysqli = init_connection();
		$save_result = false;
		if (validate_post($error)) {
			$save_result = save_external_data($mysqli, $id, $post_data, $error);
		}
		show_is_success_response($save_result, $error);
		break;
	default:
		echo "working...";
		break;
}

function validate_post(&$error) {
	if ($post_data != null && strlen($post_data) > 0 && $post_data_hash != null && strlen($post_data_hash) > 0) {
		$hash = md5($post_data);
		if(strlen($hash) == strlen($post_data_hash) && $hash == $post_data_hash) {
			return true;
		} else {
			fill_error($error, "ERR_VALIDATION_INCORRECT_HASH", "Wrong hash given on request (or corrupted data)");
		}
	} else {		
		fill_error($error, "ERR_VALIDATION_EMPTY_DATA", "Empty data or hash is given");
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

function get_or_create_player_data($mysqli, $id, &$error) {
	$get_data_result = get_player_data($mysqli, $id, $error);
	if ($error == null) {
		if($get_data_result == null) {
			$file_content = file_get_contents("GameConfigs/DefaultUser.json");
			$reencoded_default_user = json_encode(json_decode($file_content));
			$insert_query_result = mysqli_query($mysqli,"INSERT INTO `players`(`uid`, `data`, `external_data`, `first_visit_time`, `last_visit_time`, `days_play`, `gold_b`, `cash_b`) 
														 VALUES ('$id', '$reencoded_default_user','{}', UNIX_TIMESTAMP() , 0, 0, 0, 0)");
			if ($insert_query_result) {
				mysqli_free_result($insert_query_result);
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
	$sql_result = mysqli_query($mysqli,"SELECT players.data, players.external_data, players.days_play, players.first_visit_time, players.last_visit_time
								FROM players
								WHERE players.uid = '$id'
								LIMIT 1");
	if($sql_result) {
		$fetched = mysqli_fetch_assoc($sql_result);
		if($fetched) {
			$result = array();
			foreach ($fetched as $key => $value) {
				$result[$key] = json_decode($value);
			}
		}
		mysqli_free_result($sql_result);
	} else {
		fill_error($error, "ERR_SELECT_USER", "Failed to get user from db");
	}
	return $result;
}

// SET @timestamp = UNIX_TIMESTAMP();
// UPDATE `players` 
// SET 
//     `days_play` = days_play  + (FROM_UNIXTIME(@timestamp, "%Y-%m-%d") != FROM_UNIXTIME(last_visit_time, "%Y-%m-%d")),
//     `last_visit_time`= @timestamp
// WHERE uid = 't'

function init_connection() {
	$mysqli = new mysqli("127.0.0.1", "u8446_script", "kjR6aqeP", "u8446_market_2", 3310);
	if ($mysqli->connect_errno) {
	    die ( '{"response":"-1", "reason":"Could not connect to Database: (' . $mysqli->connect_errno . ')' . $mysqli->connect_error );
	};
	return $mysqli;
}

function fill_error(&$error, $code, $msg)
{
    $error = array("code" => $code, "message" => $msg);
}

function show_response($response_str, $error) {
	$response = array('response' => $response_str, 'hash' => md5($response_str));
	if($error != null) {
		$response['error'] = json_encode($error);
	}
	echo json_encode($response);
}

function show_is_success_response($is_success, $error) {
	$response_str = json_encode(array('success' => $is_success));
	show_response($response_str, $error);
}

?>
