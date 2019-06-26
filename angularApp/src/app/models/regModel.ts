export class RegModel{
    Id: string;
    Email: string;
    Name: string;
    Surname: string;
    Address: string;
    Birthday: Date;
    Password: string;
    ConfirmPassword: string;
    Role: string;
    PassengerType: string;
    Activated: string;

    constructor(id:string,email: string, name: string, surname: string, address:string, birthday: Date,password: string, confirmPassword: string, role: string,passagerType: string, ac: string){
        this.Id = id;
        this.Email = email;
        this.Name = name;
        this.Address = address
        this.Surname = surname
        this.Birthday = birthday
        this.Password = password
        this.ConfirmPassword = confirmPassword
        this.Role = role;
        this.Activated = ac;
        // if(passagerType != null && passagerType != ""){
        //     this.PassengerType = passagerType;
        // }
        this.PassengerType = passagerType;
    }
    
}