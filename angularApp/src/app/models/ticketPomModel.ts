import { TicketModel } from './ticketModel';

export class TicketPomModel {
    Ticket: TicketModel;
    PayementId: string;

    constructor(t: TicketModel,pid: string)
    {
        this.Ticket = t;
        this.PayementId = pid;
    }
} 