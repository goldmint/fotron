var helpers = require('../scripts/helpers.js');

var fotronCore = artifacts.require("FotronCore");
var fotronData = artifacts.require("FotronData");
var fotron = artifacts.require("Fotron");

var testToken = artifacts.require("TestToken");

var fotronCoreAddress = "TNtkjkAQm2TitkQ2WWjYebX2rbUdqrcNU9";

var tokenAddress = "TBjSkAjXwDuQj8afcPJnYViq6G3izvY3Jw";


module.exports = function(deployer) {
    if (fotronCoreAddress == '') {

      deployer.deploy(fotronCore);

    } else {

      if (tokenAddress == '') {

        deployer.deploy(testToken);

      } else {

        deployer.deploy(fotronData, fotronCoreAddress).then(() => {

          return deployer.deploy(fotron, tokenAddress, fotronData.address);

        });
      }
    }
};
