import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpService } from './http.service';

@Injectable()
export class ScheduleService  extends HttpService{

  constructor(http: HttpClient) { 
  super('http://localhost:49955/api/Schedule', http);
  }

}
