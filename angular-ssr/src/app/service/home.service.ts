import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {User} from '../model/user';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HomeService {

  constructor(private httpClient: HttpClient) {
  }

  getUserInfo(): Observable<User> {
    return this.httpClient.get<User>('http://demo.testrs.com:8083/');
  }
}
