export class TicketTypeModel {
    Id: number;
    Name: string;
   
  
    constructor(name: string, id: number) {
        this.Name = name;
        this.Id = id;
    }
  }