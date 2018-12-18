require('dotenv').config();


module.exports = {
  networks: {

     development: {
      // For trontools/quickstart docker image
      privateKey: 'da146374a75310b9666e834ee4ad0866d6f4035967bfc76217c5a495fff9f0d0',
      consume_user_resource_percent: 30,
      fee_limit: 100000000000,
      fullHost: "http://127.0.0.1:9090",
      network_id: "9090"
    },

    shasta: {
      privateKey: process.env.PK,
      consume_user_resource_percent: 30,
      fee_limit: 1000000000,
      fullHost: "https://api.shasta.trongrid.io",
      network_id: "*",
      from: "TFDdwNhPcHQHNq9UE8qQt8YTfeeMRV6yxG"
    },

    mainnet: {
      privateKey: process.env.PK,
      consume_user_resource_percent: 30,
      fee_limit: 1000000000,
      fullHost: "https://api.trongrid.io",
      network_id: "1"
    }
  }
}
