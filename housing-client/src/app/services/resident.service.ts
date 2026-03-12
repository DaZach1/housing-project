import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ResidentDto } from '../models/house-dto';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ResidentService {
  private apiUrl = `${environment.apiUrl}/Residents`;

  constructor(private http: HttpClient) {}
  // Создание жильца
  createResident(resident: ResidentDto): Observable<ResidentDto> {
    return this.http.post<ResidentDto>(this.apiUrl, resident);
  }

    // Удаление жильца
  deleteResident(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
  getResidentById(id: number): Observable<ResidentDto> {
    return this.http.get<ResidentDto>(`${this.apiUrl}/${id}`).pipe(
      catchError(error => {
        console.error('Error loading resident:', error);
        return throwError(() => new Error('Failed to load resident'));
      })
    );
  }
  getResidentByUserId(userId: string): Observable<ResidentDto> {
    return this.http.get<ResidentDto>(`${this.apiUrl}/by-user/${userId}`).pipe(
      catchError(error => {
        console.error('Error loading resident by user ID:', error);
        return throwError(() => new Error('Failed to load resident'));
      })
    );
  }
  updateResident(id: number, resident: ResidentDto): Observable<ResidentDto> {
    return this.http.put<ResidentDto>(`${this.apiUrl}/${id}`, resident).pipe(
      catchError(error => {
        console.error('Error updating resident:', error);
        return throwError(() => new Error('Failed to update resident'));
      })
    );
  }
}