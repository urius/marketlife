function getActiveBuildMetaData() {
	var buildName = "beta_13";

    var buildUrl = "../Build";
    var loaderUrl = buildUrl + "/" + buildName + ".loader.js";
    var config = {
      dataUrl: buildUrl + "/" + buildName + ".data.unityweb",
      frameworkUrl: buildUrl + "/" + buildName + ".framework.js.unityweb",
      codeUrl: buildUrl + "/" + buildName + ".wasm.unityweb",
      streamingAssetsUrl: "StreamingAssets",
      companyName: "isupgames",
      productName: "market life",
      productVersion: "1.02",
    };

    return {
    	loaderUrl: loaderUrl,
    	config: config
    };
}
