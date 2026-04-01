import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export enum StockTransactionType {
  Entrada = 1,
  Saida = 2,
  Ajuste = 3
}

export interface StockTransaction {
  id: number;
  productId: number;
  product?: Product;
  userId: number;
  user?: User;
  type: StockTransactionType;
  quantity: number;
  reason?: string;
  createdAtUtc: string;
}

export interface Product {
  id: number;
  name: string;
  sku: string;
}

export interface User {
  id: number;
  name: string;
  email: string;
}

export interface StockTransactionRequest {
  productId: number;
  type: StockTransactionType;
  quantity: number;
  reason?: string;
}

@Injectable({
  providedIn: 'root'
})
export class StockMovementsService {
  private apiUrl = '/api/stock-transactions';

  constructor(private http: HttpClient) {}

  getAll(): Observable<StockTransaction[]> {
    return this.http.get<StockTransaction[]>(this.apiUrl);
  }

  getById(id: number): Observable<StockTransaction> {
    return this.http.get<StockTransaction>(`${this.apiUrl}/${id}`);
  }

  create(transaction: StockTransactionRequest): Observable<StockTransaction> {
    return this.http.post<StockTransaction>(this.apiUrl, transaction);
  }
}