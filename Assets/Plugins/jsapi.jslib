mergeInto(LibraryManager.library, {
  Hello: function () {
    console.log("function Hello");
    window.alert("Hello, world!");
  },
  HelloString: function (str) {
    window.alert(Pointer_stringify(str));
  },
  SendToJs: function (str) {  
    console.log("calling window.receiveDataFromUnity: " + Pointer_stringify(str));
    window.receiveDataFromUnity(Pointer_stringify(str));
  },
  GetYGPlayerId: function(callback) {
     console.log("calling GetYGPlayerId");
     
     yandexGamesWrapper.resolvePlayer()
        .then(player => {
            const uniqueId = player.getUniqueID();
            
            const uniqueIdCStr = stringToNewUTF8(uniqueId);
            {{{ makeDynCall('vi', 'callback') }}} (uniqueIdCStr);
        });
  },
  GetCGUser: function(callback) {
     console.log("calling GetCGUser...");
     
     window.CrazyGames.SDK.user
         .getUser()
         .then(user => {
            const userNameStr = user.username;
            const userPictureUrlStr = user.profilePictureUrl;
            
            console.log(userNameStr);        
            console.log(userPictureUrlStr); 
                     
            const userNameStrCs = stringToNewUTF8(userNameStr);
            const userPictureUrlStrCs = stringToNewUTF8(userPictureUrlStr);
            {{{ makeDynCall('vii', 'callback') }}} (userNameStrCs, userPictureUrlStrCs);   
         });
  }
});