require('dotenv').config();
const TronWeb = require('tronweb');
const BigNumber = require('big-number');


const TRX = 1000000;

const HttpProvider = TronWeb.providers.HttpProvider;
/*
const fullNode = new HttpProvider('https://api.shasta.trongrid.io'); // Full node http endpoint
const solidityNode = new HttpProvider('https://api.shasta.trongrid.io'); // Solidity node http endpoint
const eventServer = 'https://api.shasta.trongrid.io'; // Contract events http endpoint
*/

const fullNode = new HttpProvider('https://api.trongrid.io'); // Full node http endpoint
const solidityNode = new HttpProvider('https://api.trongrid.io'); // Solidity node http endpoint
const eventServer = 'https://api.trongrid.io'; // Contract events http endpoint


const tronWeb = new TronWeb(
    fullNode,
    solidityNode,
    eventServer,
    process.env.PK
);

const tokenContractAddress = 'TQY2hQDXuNVB1s1b16PP9K8gS3gi5RmwFj';
const tokenContractAddressHex = tronWeb.address.toHex(tokenContractAddress);

const tokenContractAbi = '[{"constant":true,"inputs":[],"name":"creator","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"name","outputs":[{"name":"","type":"string"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"_spender","type":"address"},{"name":"_value","type":"uint256"}],"name":"approve","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"_who","type":"address"},{"name":"_tokens","type":"uint256"}],"name":"burnTokens","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"_icoContractAddress","type":"address"}],"name":"setIcoContractAddress","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"totalSupply","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"_lock","type":"bool"}],"name":"lockTransfer","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"_from","type":"address"},{"name":"_to","type":"address"},{"name":"_value","type":"uint256"}],"name":"transferFrom","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"decimals","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"_creator","type":"address"}],"name":"setCreator","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"_who","type":"address"},{"name":"_tokens","type":"uint256"}],"name":"issueTokens","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"name":"_owner","type":"address"}],"name":"balanceOf","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"lockTransfers","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"symbol","outputs":[{"name":"","type":"string"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"icoContractAddress","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"_to","type":"address"},{"name":"_value","type":"uint256"}],"name":"transfer","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"TOTAL_TOKEN_SUPPLY","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"_owner","type":"address"},{"name":"_spender","type":"address"}],"name":"allowance","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"inputs":[],"payable":false,"stateMutability":"nonpayable","type":"constructor"},{"payable":false,"stateMutability":"nonpayable","type":"fallback"},{"anonymous":false,"inputs":[{"indexed":true,"name":"_from","type":"address"},{"indexed":true,"name":"_to","type":"address"},{"indexed":false,"name":"_value","type":"uint256"}],"name":"Transfer","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"name":"_owner","type":"address"},{"indexed":true,"name":"_spender","type":"address"},{"indexed":false,"name":"_value","type":"uint256"}],"name":"Approval","type":"event"}]';
var tokenContract = null;

var fotronCoreContractAddress = 'TYoCsd8nSYkkZSaFNwepVpxn4FDxwdDE7a';
const fotronCoreContractAddressHex = tronWeb.address.toHex(fotronCoreContractAddress);
var fotronCoreContractAbi = '[{"constant":true,"inputs":[],"name":"MAX_TRX_DEAL_VAL","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"addr","type":"address"}],"name":"isAdministrator","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"addBonusPerShare","outputs":[],"payable":true,"stateMutability":"payable","type":"function"},{"constant":true,"inputs":[],"name":"_refBonusPercent","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getBlockNumSinceInit","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"amount","type":"uint256"},{"name":"percent","type":"uint256"}],"name":"calcPercent","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"pure","type":"function"},{"constant":false,"inputs":[],"name":"addBigPromoBonus","outputs":[],"payable":true,"stateMutability":"payable","type":"function"},{"constant":true,"inputs":[],"name":"_shareRewardPercent","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"val","type":"uint256"}],"name":"convert256ToReal","outputs":[{"name":"","type":"int128"}],"payable":false,"stateMutability":"pure","type":"function"},{"constant":true,"inputs":[],"name":"_initBlockNum","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getTotalBuyCount","outputs":[{"name":"res","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getInitBlockNum","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"userAddress","type":"address"}],"name":"payoutBigBonus","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"addr","type":"address"}],"name":"addManager","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"addr","type":"address"}],"name":"addControllerContract","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"_devReward","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"userAddress","type":"address"}],"name":"payoutQuickBonus","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"name":"dataContractAddress","type":"address"},{"name":"userAddress","type":"address"}],"name":"getUserTotalPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"dataContractAddress","type":"address"},{"name":"userAddress","type":"address"}],"name":"getUserTokenLocalBalance","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"addr","type":"address"}],"name":"addAdministator","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"getCurrentUserPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"_tokenOwnerRewardPercent","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getBigPromoRemainingBlocks","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"withdrawRemainingTrxAfterAll","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[],"name":"withdrawUserReward","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"getCurrentUserShareBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"_devRewardPercent","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"_promoMinPurchaseTrx","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"val","type":"uint128"}],"name":"setQuickPromoInterval","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"MIN_TOKEN_DEAL_VAL","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"dataContractAddress","type":"address"},{"name":"userAddress","type":"address"}],"name":"getUserRewardPayouts","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getTotalCollectedPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"_currentBigPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"isRefAvailable","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getTotalSellCount","outputs":[{"name":"res","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"_quickPromoPercent","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"MAGNITUDE","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"userAddress","type":"address"}],"name":"resetUserRefBalance","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"_bigPromoBlockInterval","outputs":[{"name":"","type":"uint128"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"dataContractAddress","type":"address"}],"name":"getTotalVolumeTrx","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"userAddress","type":"address"},{"name":"val","type":"uint256"}],"name":"subUserTokenLocalBalance","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"name":"dataContractAddress","type":"address"},{"name":"userAddress","type":"address"}],"name":"getUserQuickPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"userAddress","type":"address"},{"name":"volTrx","type":"uint256"},{"name":"volToken","type":"uint256"}],"name":"trackBuy","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"MIN_TRX_DEAL_VAL","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"userAddress","type":"address"},{"name":"incShareBonus","type":"bool"},{"name":"incRefBonus","type":"bool"},{"name":"incPromoBonus","type":"bool"}],"name":"getUserTotalReward","outputs":[{"name":"res","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"addQuickPromoBonus","outputs":[],"payable":true,"stateMutability":"payable","type":"function"},{"constant":false,"inputs":[{"name":"addr","type":"address"}],"name":"removeAdministator","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"_totalIncomeFeePercent","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"val","type":"uint128"}],"name":"setBigPromoInterval","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"userAddress","type":"address"}],"name":"addUserRefBalance","outputs":[],"payable":true,"stateMutability":"payable","type":"function"},{"constant":true,"inputs":[{"name":"dataContractAddress","type":"address"}],"name":"getTotalVolumeToken","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"userAddress","type":"address"}],"name":"getUserTotalTrxVolumeSaldo","outputs":[{"name":"res","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"oldAddr","type":"address"},{"name":"newAddress","type":"address"}],"name":"changeControllerContract","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"val","type":"uint256"}],"name":"setMinRefTrxPurchase","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"_bigPromoPercent","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"addDevReward","outputs":[],"payable":true,"stateMutability":"payable","type":"function"},{"constant":true,"inputs":[],"name":"MAX_TOKEN_DEAL_VAL","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"userAddress","type":"address"},{"name":"val","type":"uint256"}],"name":"addUserTokenLocalBalance","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"tokenOwnerRewardPercent","type":"uint256"},{"name":"shareRewardPercent","type":"uint256"},{"name":"refBonusPercent","type":"uint256"},{"name":"bigPromoPercent","type":"uint256"},{"name":"quickPromoPercent","type":"uint256"}],"name":"setRewardPercentages","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"addr","type":"address"}],"name":"removeManager","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"_minRefTrxPurchase","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"userAddress","type":"address"},{"name":"volTrx","type":"uint256"},{"name":"volToken","type":"uint256"}],"name":"trackSell","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"getCurrentUserRefBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getTotalBonusPerShare","outputs":[{"name":"res","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getCurrentUserTotalReward","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"dataContractAddress","type":"address"}],"name":"getSellCount","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"dataContractAddress","type":"address"},{"name":"userAddress","type":"address"},{"name":"incShareBonus","type":"bool"},{"name":"incRefBonus","type":"bool"},{"name":"incPromoBonus","type":"bool"}],"name":"getUserReward","outputs":[{"name":"reward","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"val","type":"uint256"}],"name":"setPromoMinPurchaseTrx","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"userAddress","type":"address"}],"name":"resetUserPromoBonus","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"name":"dataContractAddress","type":"address"}],"name":"getBuyCount","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"dataContractAddress","type":"address"}],"name":"getBonusPerShare","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"realVal","type":"int128"}],"name":"convertRealTo256","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"pure","type":"function"},{"constant":true,"inputs":[],"name":"_quickPromoBlockInterval","outputs":[{"name":"","type":"uint128"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"incRefBonus","type":"bool"},{"name":"incPromoBonus","type":"bool"}],"name":"getCurrentUserReward","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"addr","type":"address"}],"name":"removeControllerContract","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"name":"dataContractAddress","type":"address"},{"name":"userAddress","type":"address"}],"name":"getUserTrxVolumeSaldo","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"_currentQuickPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"withdrawDevReward","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"getQuickPromoRemainingBlocks","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"addr","type":"address"}],"name":"isManager","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"val","type":"uint256"}],"name":"setTotalIncomeFeePercent","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"userAddress","type":"address"},{"name":"val","type":"uint256"}],"name":"addUserRewardPayouts","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"name":"refAddress","type":"address"}],"name":"isRefAvailable","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"dataContractAddress","type":"address"},{"name":"userAddress","type":"address"}],"name":"getUserRefBalance","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"dataContractAddress","type":"address"},{"name":"userAddress","type":"address"}],"name":"getUserBigPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"inputs":[],"payable":false,"stateMutability":"nonpayable","type":"constructor"},{"anonymous":false,"inputs":[{"indexed":true,"name":"userAddress","type":"address"},{"indexed":false,"name":"trxWithdrawn","type":"uint256"}],"name":"onWithdrawUserBonus","type":"event"}]';
var fotronCoreContract = null;

var fotronDataContractAddress = 'THZELpsiGBen9pHAijdk4jX1cA7jm5zmed';
const fotronDataContractAddressHex = tronWeb.address.toHex(fotronDataContractAddress);
var fotronDataContractAbi = '[{"constant":true,"inputs":[],"name":"_realTokenPrice","outputs":[{"name":"","type":"int128"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"userAddress","type":"address"}],"name":"getUserQuickPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"addr","type":"address"}],"name":"isAdministrator","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"_core","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"tokenContractAddress","type":"address"}],"name":"init","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"_hasMaxPurchaseLimit","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getCoreAddress","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"userAddress","type":"address"}],"name":"getUserTrxVolumeSaldo","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"_initBlockNum","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"userAddress","type":"address"}],"name":"getUserRefBalance","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"userAddress","type":"address"}],"name":"getUserRewardPayouts","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getTotalTokenSold","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"userAddress","type":"address"},{"name":"incRefBonus","type":"bool"},{"name":"incPromoBonus","type":"bool"}],"name":"getUserReward","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"addr","type":"address"}],"name":"addAdministator","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"_totalSupply","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"val","type":"bool"}],"name":"setHasMaxPurchaseLimit","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"name":"userAddress","type":"address"}],"name":"getActualUserTokenBalance","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"EXP_PERIOD_DAYS","outputs":[{"name":"","type":"uint64"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"_tokenContractAddress","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"_controllerAddress","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"userAddress","type":"address"}],"name":"getUserTotalPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getTotalCollectedPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"PRICE_SPEED_PERCENT","outputs":[{"name":"","type":"uint64"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"newAddress","type":"address"}],"name":"setNewControllerAddress","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"_expirationTime","outputs":[{"name":"","type":"uint64"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"val","type":"uint256"}],"name":"addTokenOwnerReward","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"_initTime","outputs":[{"name":"","type":"uint64"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getCommonInitBlockNum","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"_tokenOwnerReward","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getRemainingTokenAmount","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"addr","type":"address"}],"name":"removeAdministator","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"getAdministratorCount","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"resetTokenOwnerReward","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"getBonusPerShare","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getCurrentQuickPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getCurrentBigPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"PRICE_SPEED_INTERVAL","outputs":[{"name":"","type":"uint64"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"userAddress","type":"address"}],"name":"getUserBigPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"_token","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"val","type":"int128"}],"name":"setRealTokenPrice","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"val","type":"uint256"}],"name":"setTotalSupply","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"TOKEN_PRICE_INITIAL","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"userAddress","type":"address"}],"name":"getUserTokenLocalBalance","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"isInited","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getPromoMinPurchaseTrx","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"inputs":[{"name":"coreAddress","type":"address"}],"payable":false,"stateMutability":"nonpayable","type":"constructor"}]';
var fotronDataContract = null;

var fotronContractAddress = 'TRgqLFf6oaEiNeMKZPG4W9TB6iioiJi8t2';
const fotronContractAddressHex = tronWeb.address.toHex(fotronContractAddress);
var fotronContractAbi = '[{"constant":true,"inputs":[],"name":"getCurrentUserMaxPurchase","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getTotalTokenSupply","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getPriceSpeedTokenBlock","outputs":[{"name":"","type":"uint64"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"activate","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"getCurrentUserQuickPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getTokenAddress","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getBlockNumSinceInit","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"realTokenAmount","type":"int128"}],"name":"updateTokenPrice","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"_core","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"isActive","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getMinRefTrxPurchase","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"amount","type":"uint256"},{"name":"fromTrx","type":"bool"}],"name":"estimateBuyOrder","outputs":[{"name":"","type":"uint256"},{"name":"","type":"uint256"},{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"amount","type":"uint256"},{"name":"fromToken","type":"bool"}],"name":"estimateSellOrder","outputs":[{"name":"","type":"uint256"},{"name":"","type":"uint256"},{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getCurrentUserLocalTokenBalance","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getTokenOwnerReward","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"migrateToNewNewControllerContract","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"getTokenInitialPrice","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getTotalTokenSold","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getSellCount","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"isMigrationToNewControllerInProgress","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"addr","type":"address"}],"name":"addAdministator","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"getCurrentUserPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getBigPromoRemainingBlocks","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"val","type":"bool"}],"name":"setHasMaxPurchaseLimit","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"newControllerAddr","type":"address"}],"name":"requestControllerContractMigration","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"isCurrentUserRefAvailable","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"isActualContractVer","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"name":"tokenAmount","type":"uint256"}],"name":"calcReward","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getDataContractAddress","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"migrationContractAddress","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getTotalCollectedPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"_data","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getBigPromoPercent","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getCurrentUserTotalPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getPriceSpeedPercent","outputs":[{"name":"","type":"uint64"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"prepareForMigration","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[],"name":"acceptOwnership","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"name":"userAddress","type":"address"},{"name":"isTotal","type":"bool"}],"name":"getUserReward","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getRemainingTokenAmount","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"addr","type":"address"}],"name":"removeAdministator","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"name":"userAddress","type":"address"}],"name":"getUserMaxPurchase","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getBuyCount","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getCurrentUserBigPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getShareRewardPercent","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getDevRewardPercent","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"migrateFunds","outputs":[],"payable":true,"stateMutability":"payable","type":"function"},{"constant":true,"inputs":[],"name":"getTrxDealRange","outputs":[{"name":"","type":"uint256"},{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getTokenDealRange","outputs":[{"name":"","type":"uint256"},{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getRemainingTimeTillExpiration","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"addr","type":"address"}],"name":"transferOwnershipRequest","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"getBonusPerShare","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getRefBonusPercent","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"get1TokenBuyPrice","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"approveControllerContractMigration","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"getTotalVolumeToken","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getQuickPromoBlockInterval","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"isCurrentUserAdministrator","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getTokenOwnerRewardPercent","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getCurrentUserRefBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getCurrentQuickPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getCurrentBigPromoBonus","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"name":"refAddress","type":"address"},{"name":"minReturn","type":"uint256"}],"name":"buy","outputs":[{"name":"","type":"uint256"}],"payable":true,"stateMutability":"payable","type":"function"},{"constant":true,"inputs":[{"name":"userAddress","type":"address"}],"name":"getUserLocalTokenBalance","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"finish","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"name":"tokenAmount","type":"uint256"},{"name":"minReturn","type":"uint256"}],"name":"sell","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"getTotalTrxBalance","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getExpirationTime","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getTotalVolumeTrx","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"_token","outputs":[{"name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"withdrawTokenOwnerReward","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"getBigPromoBlockInterval","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getQuickPromoPercent","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getQuickPromoRemainingBlocks","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"isMigrationApproved","outputs":[{"name":"","type":"bool"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getCurrentTokenPrice","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getPromoMinPurchaseTrx","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"get1TokenSellPrice","outputs":[{"name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"inputs":[{"name":"tokenContractAddress","type":"address"},{"name":"dataContractAddress","type":"address"}],"payable":false,"stateMutability":"nonpayable","type":"constructor"},{"payable":true,"stateMutability":"payable","type":"fallback"},{"anonymous":false,"inputs":[{"indexed":true,"name":"userAddress","type":"address"},{"indexed":false,"name":"incomingTrx","type":"uint256"},{"indexed":false,"name":"tokensMinted","type":"uint256"},{"indexed":true,"name":"referredBy","type":"address"}],"name":"onTokenPurchase","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"name":"userAddress","type":"address"},{"indexed":false,"name":"tokensBurned","type":"uint256"},{"indexed":false,"name":"trxEarned","type":"uint256"}],"name":"onTokenSell","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"name":"userAddress","type":"address"},{"indexed":false,"name":"trxReinvested","type":"uint256"},{"indexed":false,"name":"tokensMinted","type":"uint256"}],"name":"onReinvestment","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"name":"toAddress","type":"address"},{"indexed":false,"name":"trxWithdrawn","type":"uint256"}],"name":"onWithdrawTokenOwnerReward","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"name":"userAddress","type":"address"},{"indexed":false,"name":"trxWon","type":"uint256"}],"name":"onWinQuickPromo","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"name":"userAddress","type":"address"},{"indexed":false,"name":"trxWon","type":"uint256"}],"name":"onWinBigPromo","type":"event"}]';
var fotronContract = null;


const creatorPublicKey = tronWeb.address.fromPrivateKey(process.env.PK);
const creatorPublicKeyHex = tronWeb.address.toHex(creatorPublicKey);
 

async function init() {
    tokenContract = await tronWeb.contract(JSON.parse(tokenContractAbi)).at(tokenContractAddress);
    fotronCoreContract = await tronWeb.contract(JSON.parse(fotronCoreContractAbi)).at(fotronCoreContractAddress);
    fotronContract = await tronWeb.contract(JSON.parse(fotronContractAbi)).at(fotronContractAddress);
    fotronDataContract = await tronWeb.contract(JSON.parse(fotronDataContractAbi)).at(fotronDataContractAddress);

}

function convertNumResult2Trx(num) {
    var num = parseInt(num, 10);
    return num / TRX;

    //return new BigNumber(num.toString(10)).div(new BigNumber(TRX.toString(10))).toString(10);
}


async function getBalance() {
 
    const address = 'TAYfaozXhQYV4KMATmZMHtda1gttY8oGFA';
 
    // The majority of the function calls are asynchronus,
    // meaning that they cannot return the result instantly.
    // These methods therefore return a promise, which you can await.
    const balance = await tronWeb.trx.getBalance(address);
    console.log({balance});
    
    // You can also bind a `then` and `catch` method.
    tronWeb.trx.getBalance(address).then(balance => {
        console.log({balance});
    }).catch(err => console.error(err));
 
    // If you'd like to use a similar API to Web3, provide a callback function.
    tronWeb.trx.getBalance(address, (err, balance) => {
        if (err)
            return console.error(err);
 
        console.log({balance});
    });
}


async function issueTokensToController() {
    console.log("-- ISSUE TOKENS TO CONTROLLER --");

    var fotronContractTokenBalance1 = await tokenContract.balanceOf(fotronContractAddressHex).call();

    if (convertNumResult2Trx(fotronContractTokenBalance1) == '0') await tokenContract.issueTokens(fotronContractAddressHex, 2 * 1e6 * TRX).send();

    var fotronContractTokenBalance2 = await fotronContract.getRemainingTokenAmount().call();// await tokenContract.balanceOf(fotronContractAddressHex).call();

    var totalSupply = await (await tokenContract.TOTAL_TOKEN_SUPPLY.call()).call(); 
    console.log("Token Total Supply: " + convertNumResult2Trx(totalSupply));


    console.log("fotronContractTokenBalance: " + convertNumResult2Trx(fotronContractTokenBalance2));
}

async function addControllerToCore() {
    await fotronCoreContract.addControllerContract(fotronContractAddressHex).send();
}

async function activateController() {
    console.log("-- ACTIVATE THE CONTROLLER --");

    var isActive = await (await fotronContract.isActive.call()).call();
    
    console.log("controller is active: " + isActive);

    if (isActive) return;

    await fotronContract.activate().send();

    var isActive = await (await fotronContract.isActive.call()).call();

    console.log("controller is active: " + isActive);

}

async function testPurchase() {
    console.log("-- TEST PURCHASE --");

    var val = 10 * TRX;
    var estBuy = await fotronContract.estimateBuyOrder(val, true).call();

    console.log("est buy " + convertNumResult2Trx(val) + " trx. Receive " + convertNumResult2Trx(estBuy[0]) + " tokens by price " + convertNumResult2Trx(estBuy[2]) + " trx/token; fee: " + convertNumResult2Trx(estBuy[1]) + " trx");
    console.log("--");

    var buyerTokenBalance1 = await fotronContract.getCurrentUserLocalTokenBalance().call();
    var buyerTrxBalance1 = await tronWeb.trx.getBalance(creatorPublicKeyHex);
    var controllerTrxBalance1 = await tronWeb.trx.getBalance(fotronContractAddressHex);

    console.log("buyerTokenBalance1: " + convertNumResult2Trx(buyerTokenBalance1));
    console.log("buyerTrxBalance1: " + convertNumResult2Trx(buyerTrxBalance1));
    console.log("controllerTrxBalance1: " + convertNumResult2Trx(controllerTrxBalance1));

    await fotronContract.buy(creatorPublicKeyHex, 1).send({ callValue: val });


    var buyerTokenBalance2 = await fotronContract.getCurrentUserLocalTokenBalance().call();
    var buyerTrxBalance2 = await tronWeb.trx.getBalance(creatorPublicKeyHex);
    var controllerTrxBalance2 = await tronWeb.trx.getBalance(fotronContractAddressHex);

    console.log("buyerTokenBalance2: " + convertNumResult2Trx(buyerTokenBalance2));
    console.log("buyerTrxBalance2: " + convertNumResult2Trx(buyerTrxBalance2));
    console.log("controllerTrxBalance2: " + convertNumResult2Trx(controllerTrxBalance2));
}

async function testSelling() {
    console.log("-- TEST SELLING --");


    var val = 1000 * TRX;
    var estSell = await fotronContract.estimateSellOrder(val, true).call();
    console.log("est sell " + convertNumResult2Trx(val) + " token: Receive " + convertNumResult2Trx(estSell[0]) + " trx by price " + convertNumResult2Trx(estSell[2]) + " trx/token; fee: " + convertNumResult2Trx(estSell[1]) + " trx");
        
    console.log("--");

    var buyerTokenBalance1 = await fotronContract.getCurrentUserLocalTokenBalance().call();
    var buyerTrxBalance1 = await tronWeb.trx.getBalance(creatorPublicKeyHex);
    var controllerTrxBalance1 = await tronWeb.trx.getBalance(fotronContractAddressHex);

    console.log("buyerTokenBalance1: " + convertNumResult2Trx(buyerTokenBalance1));
    console.log("buyerTrxBalance1: " + convertNumResult2Trx(buyerTrxBalance1));
    console.log("controllerTrxBalance1: " + convertNumResult2Trx(controllerTrxBalance1));



    await tokenContract.approve(fotronContractAddressHex, val).send();
    await fotronContract.sell(val, 1).send();

    var buyerTokenBalance2 = await fotronContract.getCurrentUserLocalTokenBalance().call();
    var buyerTrxBalance2 = await tronWeb.trx.getBalance(creatorPublicKeyHex);
    var controllerTrxBalance2 = await tronWeb.trx.getBalance(fotronContractAddressHex);

    console.log("buyerTokenBalance2: " + convertNumResult2Trx(buyerTokenBalance2));
    console.log("buyerTrxBalance2: " + convertNumResult2Trx(buyerTrxBalance2));
    console.log("controllerTrxBalance2: " + convertNumResult2Trx(controllerTrxBalance2));

}

async function showStat() {
    console.log("-- STAT --");

    var curTokenPrice = await fotronContract.getCurrentTokenPrice().call();
    console.log("curTokenPrice: " + (curTokenPrice));

    var buyCount = await fotronContract.getBuyCount().call();
    console.log("buy count: " + buyCount.toString(10));

    var sellCount = await fotronContract.getSellCount().call();
    console.log("sell count: " + sellCount.toString(10));

    var buyerTokenBalance1 = await fotronContract.getCurrentUserLocalTokenBalance().call();
    var buyerTrxBalance1 = await tronWeb.trx.getBalance(creatorPublicKeyHex);
    var controllerTrxBalance1 = await tronWeb.trx.getBalance(fotronContractAddressHex);
    var controllerTokenBalance = await tokenContract.balanceOf(fotronContractAddressHex).call();

    console.log("buyerTokenBalance: " + convertNumResult2Trx(buyerTokenBalance1));
    console.log("buyerTrxBalance: " + convertNumResult2Trx(buyerTrxBalance1));
    console.log("controllerTrxBalance: " + convertNumResult2Trx(controllerTrxBalance1));
    console.log("controllerTokenBalance: " + convertNumResult2Trx(controllerTokenBalance));

    var tokenBuyPrice = await fotronContract.get1TokenBuyPrice().call();
    console.log("tokenBuyPrice: " + convertNumResult2Trx(tokenBuyPrice));

    var tokenSellPrice = await fotronContract.get1TokenSellPrice().call();
    console.log("tokenSellPrice: " + convertNumResult2Trx(tokenSellPrice));

    var userTotalReward = await fotronCoreContract.getCurrentUserTotalReward().call();
    console.log("userTotalReward: " + userTotalReward.toString(10));

    var volumeTrx = await fotronCoreContract.getUserTotalTrxVolumeSaldo(creatorPublicKeyHex).call();
    console.log(volumeTrx.toString(10));
    console.log("volumeTrx: " + convertNumResult2Trx(volumeTrx));

}

async function testEsimations() {

    console.log("-- TEST ESTIMATIONS --");

    var val = 10;
    var estBuy = await fotronContract.estimateBuyOrder(val * TRX, true).call();

    console.log("est buy " + val + " trx. Receive " + convertNumResult2Trx(estBuy[0]) + " tokens by price " + convertNumResult2Trx(estBuy[2]) + " trx/token; fee: " + convertNumResult2Trx(estBuy[1]) + " trx");


    var val = 10000;
    var estSell = await fotronContract.estimateSellOrder(val * TRX, true).call();
    console.log("est sell " + val + " tokens: Receive " + convertNumResult2Trx(estSell[0]) + " trx by price " + convertNumResult2Trx(estSell[2]) + " trx/token; fee: " + convertNumResult2Trx(estSell[1]) + " trx");
        
}



async function run() {
    //await getBalance();

    await init();

    await testEsimations();


    //await issueTokensToController();

    //await addControllerToCore();

    //await activateController();

    //await testPurchase();

    //await testSelling();

    await showStat();
}
 
run();