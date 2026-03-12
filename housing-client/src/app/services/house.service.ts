import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HouseDto } from '../models/house-dto';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class HouseService {
  private apiUrl = `${environment.apiUrl}/Houses`;

  constructor(private http: HttpClient) {}

  // Создание дома
  createHouse(house: HouseDto): Observable<HouseDto> {
    return this.http.post<HouseDto>(this.apiUrl, house);
  }
  // Удаление дома
  deleteHouse(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
  // Метод для получения всех домов
  getHouses(): Observable<HouseDto[]> {
    return this.http.get<HouseDto[]>(this.apiUrl);
  }

  // Метод для получения одного дома по ID
  getHouseById(id: number): Observable<HouseDto> {
    return this.http.get<HouseDto>(`${this.apiUrl}/${id}`);
  }
  updateHouse(id: number, house: HouseDto): Observable<HouseDto> {
    return this.http.put<HouseDto>(`${this.apiUrl}/${id}`, house).pipe(
      catchError(error => {
        console.error('Error updating house:', error);
        return throwError(() => new Error('Failed to update house'));
      })
    );
}
}