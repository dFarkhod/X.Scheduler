export class Schedule{


private _date : Date;
public get date() : Date {
    return this._date;
}
public set date(v : Date) {
    this._date = v;
}


private _shift : number;
public get shift() : number {
    return this._shift;
}
public set shift(v : number) {
    this._shift = v;
}


private _staff : string;
public get staff() : string {
    return this._staff;
}
public set staff(v : string) {
    this._staff = v;
}

private _columns : string;
public get columns() : string {
    return this._columns;
}
public set columns(v : string) {
    this._columns = v;
}

    
}