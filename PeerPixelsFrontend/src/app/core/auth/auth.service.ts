import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { AuthResponse, GoogleAuth, Login, Register } from '../models/auth.model';
import { User } from '../models/user.model';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser$: Observable<User | null>;
  private jwtHelper = new JwtHelperService();

  constructor(private http: HttpClient) {
    const user = this.getCurrentUserFromStorage();
    this.currentUserSubject = new BehaviorSubject<User | null>(user);
    this.currentUser$ = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  register(registerData: Register): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/register`, registerData)
      .pipe(
        tap(response => {
          if (response.succeeded) {
            this.setSession(response);
            this.currentUserSubject.next(response.user);
          }
        }),
        catchError(error => throwError(() => error))
      );
  }

  login(loginData: Login): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, loginData)
      .pipe(
        tap(response => {
          if (response.succeeded) {
            this.setSession(response);
            this.currentUserSubject.next(response.user);
          }
        }),
        catchError(error => throwError(() => error))
      );
  }

  googleLogin(idToken: string): Observable<AuthResponse> {
    const googleAuth: GoogleAuth = { idToken };
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/google`, googleAuth)
      .pipe(
        tap(response => {
          if (response.succeeded) {
            this.setSession(response);
            this.currentUserSubject.next(response.user);
          }
        }),
        catchError(error => throwError(() => error))
      );
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem('expires_at');
    this.currentUserSubject.next(null);
  }

  isLoggedIn(): boolean {
    const token = localStorage.getItem('token');
    return !!token && !this.jwtHelper.isTokenExpired(token);
  }

  private setSession(authResult: AuthResponse): void {
    localStorage.setItem('token', authResult.token);
    localStorage.setItem('user', JSON.stringify(authResult.user));
    
    const expiresAt = this.jwtHelper.getTokenExpirationDate(authResult.token)?.getTime() || 0;
    localStorage.setItem('expires_at', JSON.stringify(expiresAt));
  }

  private getCurrentUserFromStorage(): User | null {
    const token = localStorage.getItem('token');
    if (token && !this.jwtHelper.isTokenExpired(token)) {
      const user = localStorage.getItem('user');
      return user ? JSON.parse(user) : null;
    }
    return null;
  }

  updateCurrentUser(user: User): void {
    const currentUser = this.currentUserSubject.value;
    if (currentUser) {
      const updatedUser = { ...currentUser, ...user };
      this.currentUserSubject.next(updatedUser);
      localStorage.setItem('user', JSON.stringify(updatedUser));
    }
  }
}