export class PayPalModel{
    Id: number;
    PayementId: string;
    CreateTime: Date;
    PayerEmail: string;
    PayerName: string;
    PayerSurname: string;
    CurrencyCode: string;
    Value: number;



    constructor(id: number,pyId?: string, ct?: Date, payerEmai? : string, payerName? : string, payerSurname?: string, cc? : string, value?: number){

      this.Id = id;
      this.PayementId = pyId;
      this.CreateTime = ct;
      this.PayerEmail = payerEmai;
      this.PayerName = payerName;
      this.PayerSurname = payerSurname;
      this.CurrencyCode = cc;
      this.Value = value;
    }
} 