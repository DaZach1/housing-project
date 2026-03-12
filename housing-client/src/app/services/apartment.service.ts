import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ApartmentDto } from '../models/house-dto';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ApartmentService {
  private apiUrl = `${environment.apiUrl}/Apartments`;
  

  constructor(private http: HttpClient) {}
  
  createApartment(apartment: ApartmentDto): Observable<ApartmentDto> {
    return this.http.post<ApartmentDto>(this.apiUrl, apartment);
  }

  deleteApartment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // Метод для получения квартиры по ID
  getApartmentById(id: number): Observable<ApartmentDto> {
    return this.http.get<ApartmentDto>(`${this.apiUrl}/${id}`);
  }

  // Метод для обновления квартиры
  updateApartment(id: number, apartment: ApartmentDto): Observable<ApartmentDto> {
    // Создаем копию без residents
    const updateData = {
      ...apartment,
      residents: undefined
    };
    
    return this.http.put<ApartmentDto>(
      `${this.apiUrl}/${id}`,
      updateData,
      {
        headers: { 'Content-Type': 'application/json' }
      }
    );
  }
}