import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DailyFlow {
  day: string;
  entradas: number;
  saidas: number;
}

export interface DashboardReport {
  totalProducts: number;
  totalCategories: number;
  lowStockProducts: number;
  transactionsLast30Days: number;
  dailyFlow: DailyFlow[];
}

@Injectable({
  providedIn: 'root'
})
export class ReportsService {
  private apiUrl = '/api/reports';

  constructor(private http: HttpClient) {}

  getDashboard(): Observable<DashboardReport> {
    return this.http.get<DashboardReport>(`${this.apiUrl}/dashboard`);
  }
}