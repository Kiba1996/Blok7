export class TicketHelpModel{
    PurchaseTime: Date;
    ExparationTime: string;
    TicketType: string;
    TicketPrice: number;

    constructor( purchaseTime: Date, exparationTime: string, ticketType: string,ticketPrice: number){
        this.PurchaseTime = purchaseTime;
        this.ExparationTime = exparationTime;
        this.TicketType = ticketType;
        this.TicketPrice = ticketPrice;

    }
}