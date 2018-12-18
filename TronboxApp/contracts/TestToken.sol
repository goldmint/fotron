pragma solidity ^0.4.23;

contract SafeMath {
     function safeMul(uint a, uint b) internal pure returns (uint) {
          uint c = a * b;
          assert(a == 0 || c / a == b);
          return c;
     }

     function safeSub(uint a, uint b) internal pure returns (uint) {
          assert(b <= a);
          return a - b;
     }

     function safeAdd(uint a, uint b) internal pure returns (uint) {
          uint c = a + b;
          assert(c>=a && c>=b);
          return c;
     }
}

// ERC20 standard
contract StdToken is SafeMath {
     // Fields:
     mapping(address => uint256) balances;
     mapping (address => mapping (address => uint256)) allowed;
     uint public totalSupply = 0;

     // Events:
     event Transfer(address indexed _from, address indexed _to, uint256 _value);
     event Approval(address indexed _owner, address indexed _spender, uint256 _value);

     // Functions:
     function transfer(address _to, uint256 _value) onlyPayloadSize(2 * 32) public returns(bool){
          require(balances[msg.sender] >= _value);
          require(balances[_to] + _value > balances[_to]);

          balances[msg.sender] = safeSub(balances[msg.sender],_value);
          balances[_to] = safeAdd(balances[_to],_value);

          emit Transfer(msg.sender, _to, _value);
          return true;
     }

     function transferFrom(address _from, address _to, uint256 _value) public returns(bool) {
          require(balances[_from] >= _value);
          require(allowed[_from][msg.sender] >= _value);
          require(balances[_to] + _value > balances[_to]);

          balances[_to] = safeAdd(balances[_to],_value);
          balances[_from] = safeSub(balances[_from],_value);
          allowed[_from][msg.sender] = safeSub(allowed[_from][msg.sender],_value);

          emit Transfer(_from, _to, _value);
          return true;
     }

     function balanceOf(address _owner) view public returns (uint256) {
          return balances[_owner];
     }

     function approve(address _spender, uint256 _value) public returns (bool) {
          // To change the approve amount you first have to reduce the addresses`
          //  allowance to zero by calling `approve(_spender, 0)` if it is not
          //  already 0 to mitigate the race condition described here:
          //  https://github.com/tron/EIPs/issues/20#issuecomment-263524729
          require((_value == 0) || (allowed[msg.sender][_spender] == 0));

          allowed[msg.sender][_spender] = _value;
          emit Approval(msg.sender, _spender, _value);
          return true;
     }

     function allowance(address _owner, address _spender) view public returns (uint256) {
          return allowed[_owner][_spender];
     }

     modifier onlyPayloadSize(uint _size) {
          require(msg.data.length >= _size + 4);
          _;
     }
}

contract TestToken is StdToken {
     // Fields:
     string public constant name = "Test Token";
     string public constant symbol = "MNTP";
     uint public constant decimals = 18;

     address public creator = 0x0;
     address public icoContractAddress = 0x0;
     bool public lockTransfers = false;

     // 10 mln
     uint public constant TOTAL_TOKEN_SUPPLY = 10000000 * 1 trx;

     /// Modifiers:
     modifier onlyCreator() { 
          require(msg.sender == creator); 
          _; 
     }

     function setCreator(address _creator) public {
          creator = _creator;
     }

     // Setters/Getters
     function setIcoContractAddress(address _icoContractAddress) public onlyCreator {
          icoContractAddress = _icoContractAddress;
     }

     // Functions:
     constructor() public {
          creator = msg.sender;

          assert(TOTAL_TOKEN_SUPPLY == 10000000 * 1 trx);
     }

     /// @dev Override
     function transfer(address _to, uint256 _value) public returns(bool){
          require(!lockTransfers);
          return super.transfer(_to,_value);
     }

     /// @dev Override
     function transferFrom(address _from, address _to, uint256 _value) public returns(bool){
          require(!lockTransfers);
          return super.transferFrom(_from,_to,_value);
     }

     function issueTokens(address _who, uint _tokens) public onlyCreator {

          balances[_who] = safeAdd(balances[_who],_tokens);
          totalSupply = safeAdd(totalSupply,_tokens);

          emit Transfer(0x0, _who, _tokens);
     }

     // For refunds only
     function burnTokens(address _who, uint _tokens) public onlyCreator {
          balances[_who] = safeSub(balances[_who], _tokens);
          totalSupply = safeSub(totalSupply, _tokens);
     }

     function lockTransfer(bool _lock) public onlyCreator {
          lockTransfers = _lock;
     }

     // Do not allow to send money directly to this contract
     function() public {
          revert();
     }
}
