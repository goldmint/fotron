var helpers = require('../scripts/helpers.js');

var fotronCore = artifacts.require("FotronCore");
var fotronData = artifacts.require("FotronData");
var fotron = artifacts.require("Fotron");

var testToken = artifacts.require("TestToken");

var fotronCoreAddress = "TXPnqbPSxfUjyo9K6HvrdWYzVUo1uUiUd7";

var tokenAddress = "TJiSPzUkWDGJQqA2MsQL6YyNnYYwT6kJ7p";

var dataAddress = "TWH36nNq9dF8DLAT44ZRP7otS6L9uZZMvr";


module.exports = function(deployer) {

    if (fotronCoreAddress == '') {

      deployer.deploy(fotronCore);

    } else {

      if (tokenAddress == '') {

        deployer.deploy(testToken);

      } else {

        if (dataAddress == '') {
          deployer.deploy(fotronData, fotronCoreAddress).then(() => {
            return deployer.deploy(fotron, tokenAddress, fotronData.address);
          });
        } else {
          deployer.deploy(fotron, tokenAddress, dataAddress);
        }
      }
    }
};
