function init(callback) {
	vkBridge
	  .send('VKWebAppInit')
	  .then(data => {
	  	var queryDict = {};
	  	location.search.substr(1).split("&").forEach(function(item) {queryDict[item.split("=")[0]] = item.split("=")[1]});
	 	window.vkParams = queryDict;

	  	callback();
	  })
	  .catch(error => {
		console.log(JSON.stringify(error));
	  });
}
function callVKApi(methodName, params, callback) {
	if (params == null) params = {};

	var accessToken = window.access_token != null ? window.access_token : window.vkParams.access_token;
	params.v = "5.131";
	params.access_token = accessToken;
	vkBridge.send("VKWebAppCallAPIMethod", {"method": methodName, "request_id": new Date().toISOString(), "params": params})
	.then(data => {
		if(callback != null) {
			callback(data);
		}
	})
	.catch(error => {
		console.log(JSON.stringify(error));
	});
}
function showInviteBox() {
	vkBridge.send("VKWebAppShowInviteBox", {})
         //.then(data => alert(data.success))
		.catch(error => {
			console.log(JSON.stringify(error));
		});
}
function showOrderBox(item, callback) {
	vkBridge.send("VKWebAppShowOrderBox", {type:"item",item:item})
        .then(data => callback({ is_success:true }))
        .catch(error => {
        	var isUserCancelled = (error != null && error.error_data != null && error.error_data.error_code == 4);
        	callback({ is_success:false, is_user_cancelled:isUserCancelled });
        });
}
function wallPost(message, successCallback) {
	proceedWithPermissions("wall",() => wallPostInternal(message, successCallback));
}
function wallPostInternal(message, successCallback) {
	vkBridge.send("VKWebAppShowWallPostBox", {"message": message, "attachments":"https://vk.com/app4995114_48982", "v":"5.131"})
		.then(data => {
				if (data != null && data.post_id != null) {					
					successCallback();		
				}
			})
		.catch(error => {
			console.log(JSON.stringify(error));
		});
}
function proceedWithPermissions(permissionsStr, callback) {
	var appId = parseInt(window.vkParams.api_id);
	var splittedPermissions = permissionsStr.split(",");
	vkBridge.send("VKWebAppGetAuthToken", {"app_id": appId, "scope": permissionsStr})
		.then(data => {
			var allPermissions = true;
			if(data.scope != null) {				
				for (var i = 0; i < splittedPermissions.length; i++) {
					allPermissions &= data.scope.indexOf(splittedPermissions[i]) >= 0;
				}
			}

			if (allPermissions == true) {
				window.access_token = data.access_token;
				callback();
			}
		})
		.catch(error => {
			console.log(JSON.stringify(error));
		});;
}
function showNativeAds(callback) {
	vkBridge.send("VKWebAppShowNativeAds", {ad_format:"reward"})
	.then(data => (data != null && data.result === true) ? callback(true) : callback(false))
	.catch(error => callback(false));
}