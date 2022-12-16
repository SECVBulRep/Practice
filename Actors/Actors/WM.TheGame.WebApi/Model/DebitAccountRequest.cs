namespace WM.TheGame.WebApi.Model;



public record  DebitAccountRequest(string From,string To,decimal Amount);

public record  DebitAccountResponse(decimal FromAmount,decimal ToAmount);

public record  SendMoneyRequest(string From,string To,decimal Amount);