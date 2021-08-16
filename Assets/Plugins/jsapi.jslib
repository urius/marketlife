mergeInto(LibraryManager.library, {
  Hello: function () {
    console.log("function Hello");
    window.alert("Hello, world!");
  },

  HelloString: function (str) {
    window.alert(Pointer_stringify(str));
  }
});