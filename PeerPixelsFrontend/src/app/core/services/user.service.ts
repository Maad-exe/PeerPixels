import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { User, UpdateUser } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private http: HttpClient) {}

  getUserById(id: string): Observable<User> {
    return this.http.get<User>(`${environment.apiUrl}/users/${id}`);
  }

  getUserByUsername(username: string): Observable<User> {
    return this.http.get<User>(`${environment.apiUrl}/users/username/${username}`);
  }

  updateUser(updateUser: UpdateUser): Observable<User> {
    return this.http.put<User>(`${environment.apiUrl}/users`, updateUser);
  }

  getFollowers(userId: string): Observable<User[]> {
    return this.http.get<User[]>(`${environment.apiUrl}/users/${userId}/followers`);
  }

  getFollowing(userId: string): Observable<User[]> {
    return this.http.get<User[]>(`${environment.apiUrl}/users/${userId}/following`);
  }

  followUser(userId: string): Observable<any> {
    return this.http.post<any>(`${environment.apiUrl}/users/follow/${userId}`, {});
  }

  unfollowUser(userId: string): Observable<any> {
    return this.http.delete<any>(`${environment.apiUrl}/users/unfollow/${userId}`);
  }
}