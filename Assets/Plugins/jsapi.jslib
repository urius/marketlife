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
  }
});