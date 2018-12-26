var helpers = require('../scripts/helpers.js');

var fotronCore = artifacts.require("FotronCore");
var fotronData = artifacts.require("FotronData");
var fotron = artifacts.require("Fotron");

var testToken = artifacts.require("TestToken");

var fotronCoreAddress = "TYoCsd8nSYkkZSaFNwepVpxn4FDxwdDE7a";

var tokenAddress = "TQY2hQDXuNVB1s1b16PP9K8gS3gi5RmwFj";

var dataAddress = "";


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
