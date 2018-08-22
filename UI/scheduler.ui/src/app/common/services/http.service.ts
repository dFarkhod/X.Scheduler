import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import 'rxjs/add/observable/throw';
import { AppError } from '../exceptionHandling/app-error';
import { NotFoundError } from '../exceptionHandling/not-found-error';
import { BadInput } from '../exceptionHandling/bad-input';

@Injectable()
export class HttpService {

  constructor(private url: string, private http: HttpClient) { }

  public getAll() {
    return this.http.get(this.url).map(response => response)
    .catch(this.handleError);
  }

  public create(rename) {
    return this.http.post(this.url, rename)
      .catch(this.handleError);
  }

  public update(item) {
    return this.http.patch(this.url + '/' + item.id, JSON.stringify(item.title))
    .catch(this.handleError);
  }

  public delete(item){
    return this.http.delete(this.url + '/' + item.id)
      .catch(this.handleError)
  }

  private handleError(error: Response){
    if (error.status === 404) {
      return Observable.throw(new NotFoundError(error));
    }

    if (error.status === 400) {
      return Observable.throw(new BadInput(error.json()));
    }

    return Observable.throw(new AppError(error));
  }

}
