import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap, catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private currentUserSubject = new BehaviorSubject<any>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {
    this.loadUserFromStorage();
  }

  private loadUserFromStorage(): void {
    const user = localStorage.getItem('currentUser');
    if (user) {
      this.currentUserSubject.next(JSON.parse(user));
    }
  }

  login(email: string, password: string): Observable<any> {
    // Сначала очищаем предыдущие данные
    this.clearAuthData();
    
    return this.http.post(`${this.apiUrl}/login`, { email, password }).pipe(
      tap((response: any) => {
        this.setAuthData(response);
      }),
      catchError(error => {
        this.clearAuthData();
        return throwError(() => error);
      })
    );
  }


  logout(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/logout`, {}).pipe(
        tap(() => {
            this.clearAuthData();
        }),
        catchError(error => {
            // Даже если серверный logout не сработал, очищаем клиентские данные
            this.clearAuthData();
            return throwError(() => error);
        })
    );
  }

  private setAuthData(response: any): void {
    localStorage.setItem('currentUser', JSON.stringify(response));
    localStorage.setItem('token', response.token);
    this.currentUserSubject.next(response);
  }

  private clearAuthData(): void {
    // Полный список всех возможных ключей
    const authKeys = [
      'currentUser', 'token', 
      'authToken', 'userData', 
      'session', 'refreshToken'
    ];
  
    // Очищаем все хранилища
    authKeys.forEach(key => {
      localStorage.removeItem(key);
      sessionStorage.removeItem(key);
    });
  
    // Сбрасываем Observable
    this.currentUserSubject.next(null);
    
    // Принудительно очищаем кэш API
    if (typeof caches !== 'undefined') {
      caches.keys().then(keys => {
        keys.forEach(key => caches.delete(key));
      });
    }
  }

  get currentUserValue(): any {
    return this.currentUserSubject.value;
  }

  get token(): string | null {
    return localStorage.getItem('token');
  }

  hasRole(role: string): boolean {
    const user = this.currentUserValue;
    return user?.role === role;
  }

  get isManager(): boolean {
    return this.currentUserValue?.role === 'Manager';
  }
}