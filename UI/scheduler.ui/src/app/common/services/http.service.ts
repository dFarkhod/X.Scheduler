import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';
import 'rxjs/add/observable/throw';
import { NotFoundError } from '../exceptionHandling/not-found-error';
import { BadInput } from '../exceptionHandling/bad-input';
import { AppError } from '../exceptionHandling/app-error';

@Injectable()
export class HttpService {

  constructor(private url: string, private http: HttpClient) { }

  public getAll<T>(): Observable<T[]> {
    return this.http.get<Array<T>>(this.url)
    .catch(this.handleError);
  }

  public create(item) {
    return this.http.post(this.url, item)
    .map(response=>JSON.stringify(response))
    .catch(this.handleError);
  }

  public update<T>(item) {
    return this.http.patch<T>(this.url + '/' + item.id, JSON.stringify(item.title))
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
