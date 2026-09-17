import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateMedicine, Medicine, SaleRecord } from './models';

@Injectable({ providedIn: 'root' })
export class PharmacyService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5000/api';

  getMedicines(search = ''): Observable<Medicine[]> {
    let params = new HttpParams();
    if (search.trim()) params = params.set('search', search.trim());
    return this.http.get<Medicine[]>(`${this.apiUrl}/medicines`, { params });
  }

  addMedicine(medicine: CreateMedicine): Observable<Medicine> {
    return this.http.post<Medicine>(`${this.apiUrl}/medicines`, medicine);
  }

  getSales(): Observable<SaleRecord[]> {
    return this.http.get<SaleRecord[]>(`${this.apiUrl}/sales`);
  }

  sellMedicine(medicineId: number, quantity: number, soldBy = 'admin'): Observable<SaleRecord> {
    return this.http.post<SaleRecord>(`${this.apiUrl}/sales`, {
      medicineId, quantity, soldBy
    });
  }
}
