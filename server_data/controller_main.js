init(tryEstablishConnection);

function HandleGetFriendsResponse(responseObj) {
	console.log("friends.get response.items: " + JSON.stringify(responseObj));
	if (responseObj.response != null && responseObj.response.items != null) {
		var items = responseObj.response.items;
		window.friends = items.filter(f => f.deactivated == null).sort(function (f1,f2) { return (f2.last_seen != null ? f2.last_seen.time : 0) - (f1.last_seen != null ? f1.last_seen.time : 0); });
		callVKApi("friends.getAppUsers", null, HandleGetAppFriendsResponse);
	}
}

function HandleGetAppFriendsResponse(responseObj) {
	console.log("friends.getAppUsers response: " + JSON.stringify(responseObj));
	if (responseObj.response != null && responseObj.response.length > 0) {
		var appFriendsIds = responseObj.response;
		var friends = window.friends;
		var appFriends = [];
		var invitableFriends = [];
		for (var i = 0; i < friends.length; i++) {
			var friendItem = friends[i];
			if (appFriendsIds.indexOf(friendItem.id) >= 0) {
				friendItem.is_app = true;
				appFriends.push(friendItem);
				//console.log("friend " + friends[i].first_name + " is_app");
			} else {
				invitableFriends.push(friendItem);
			}
		}

		console.log("sending SetVkFriendsData: " + window.friends.length);
		sendCommandToUnity('SetVkFriendsData', appFriends.concat(invitableFriends));
	}
}

function onUnityInstanceLoaded(unityInstance) {
	window.receiveDataFromUnity = receiveDataFromUnityHandler;
    window.unityInstance = unityInstance;
	tryEstablishConnection();
}

function tryEstablishConnection() {
	if (window.vkParams != null && window.unityInstance != null) {
		sendCommandToUnity('SetVkPlatformData', window.vkParams);

		proceedWithPermissions("friends", getAppFriendsSequence);
	}
}

function getAppFriendsSequence() {
	callVKApi("friends.get", { fields: "first_name,last_name,photo_50,photo_100,online,last_seen", order:"random" }, HandleGetFriendsResponse);
}

function sendCommandToUnity(command, data) {
    window.unityInstance.SendMessage('JsBridge', 'JsCommandMessage', JSON.stringify({command:command, data:data}));
}

function receiveDataFromUnityHandler(data) {
	console.log("OnReceiveDataFromUnity!, data:" + data);
	var parsedData = JSON.parse(data);
	processCommandFromUnity(parsedData.command, parsedData.payload);
}

function processCommandFromUnity(command, payload) {
	switch(command) {
		case "InviteFriend":
			//VK.callMethod("showRequestBox", parseInt(payload.uid), "Открывай свой магазин и присоединяйся! Жлём только тебя! :)", "invite_revenue_5g");
		break;
		case "InviteFriends":
			showInviteBox();
		break;
		case "BuyMoney":
			window.chargedProductId = payload.product;
			showOrderBox(payload.product, onOrderBoxCallback);
		break;
		case "LevelUp":
			sendRequest(
				"https://devman.ru/marketVK/unity/vk/VKDataReceiver.php?command=push_level&id=" + window.vkParams.viewer_id + "&level=" + payload.level + "&time=" + (new Date()).getTime());
		break;
		case "PostNewLevel":
			postNewLevel(payload.level);
		break;
		case "PostOfflineRevenue":
			postOfflineRevenue(payload.hours, payload.minutes, payload.revenue);
		break;
		case "ShowAds":
			showNativeAds(result =>	sendCommandToUnity('ShowAdsResult', { is_success: result }));
		break;
	}
}

function onOrderBoxCallback(result) {
	sendCommandToUnity('BuyVkMoneyResult', { is_success:result.is_success, is_user_cancelled:result.is_user_cancelled, order_id:window.chargedProductId });
}

function sendRequest(url, callback = null) {
	const http = new XMLHttpRequest();
	http.open("GET", url);
	http.send();
	if (callback != null) {		
		http.onload = () => callback(http.responseText);
	}
}

function postNewLevel(level) {
	var str = "Прокачал свой магазин до " + level + "-го уровня!\nДогоняйте!";
	switch(level) {
	    case 2:
			str = "Еще чуть-чуть, и стану супермаркетом!\nВторой уровень - это вам не палатка с шаурмой :)";
		break;
		case 3:
			str = "Бизнес растет как на дрожжах!\nТретий уровень - уже появляются постоянные клиенты!";
		break;
		case 4:
			str = "Конкуренты? Не, не слышали :)\nМой магазин уже на четвертом уровне!";
		break;
		case 5:
			str = "Магазинчик пятого уровня!\nКогда-нибудь видели что-то подобное? :)";
		break;
		case 6:
			str = "Это уже серьёзный бизнес!\nМой магазин уже на шестом уровне!";
		break;
		case 7:
			str = "Спасибо друзьям, клиентам и бизнес партнерам!\nМы сделали это! Седьмой уровень!";
		break;
		case 8:
			str = "Мне всегда говорили, что я талантливый руководитель!\nМой магазин развился уже до восьмого уровня!";
		break;
		case 9:
			str = "Этот супермаркет знают все вокруг!\nМой магазинчик настолько популярен, что добрался до девятого уровня!";
		break;
		case 10:
			str = "Это давно уже торговый центр, а не магазинчик :)\nПриглашаю всех посетить мой магазин десятого уровня!";
		break;
	}
	wallPost(str, () => sendCommandToUnity('VkWallPostSuccess'));
}

function postOfflineRevenue(hours, minutes, revenue) {
	var timePassedStr = ((hours >= 1) ? (parseInt(hours) + " ч.") : (minutes +" мин."));
	wallPost("Мой магазинчик принес мне " + revenue + "$ всего за " + timePassedStr + "!\nА ваш магазинчик так сможет? :)", () => sendCommandToUnity('VkWallPostSuccess'));
}

//{"api_url":"https://api.vk.com/api.php","api_id":"4995114","api_settings":"8455","viewer_id":"48982",
//"viewer_type":"0","sid":"2dcd4aafa1575def2e4c9d3c5629e00b824b7e113482d224f0eb22746b7431dea9ef606faee73e269c9ea",
//"secret":"94836c36ab","access_token":"e1c0e3e7ad550705f2325d465a0c2e22448fd174a38ca546dc89881787d0f717e53eb19c267925144fed0",
//"user_id":"0","is_app_user":"1","language":"0","parent_language":"0","is_secure":"1","stats_hash":"0f6038c9a92a05c47d",
//"group_id":"0","ads_app_id":"4995114_6d2e2a13c4ce8e81d7","access_token_settings":"notify,friends,photos,menu,wall",
//"referrer":"unknown","lc_name":"2d629194","platform":"web","is_widescreen":"0",
//"whitelist_scopes":"friends,photos,video,stories,pages,status,notes,wall,docs,groups,stats,market,ads,notifications",
//"group_whitelist_scopes":"stories,photos,app_widget,messages,wall,docs,manage",
//"auth_key":"fbe8f7b5f4668d430a71b6608b1d4306","timestamp":"1629136547","sign":"ybAJ4yRuKLawG4c2TWOu3KJQFUM1LdFNpZT5KRUyXcM",
//"sign_keys":"access_token,access_token_settings,ads_app_id,api_id,api_settings,api_url,auth_key,group_id,group_whitelist_scopes,is_app_user,is_secure,is_widescreen,language,lc_name,parent_language,platform,referrer,secret,sid,stats_hash,timestamp,user_id,viewer_id,viewer_type,whitelist_scopes",
//"hash":""}