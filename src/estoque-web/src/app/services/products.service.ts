import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Product {
  id: number;
  name: string;
  sku: string;
  categoryId: number;
  category?: Category;
  supplierId: number;
  supplier?: Supplier;
  unitPrice: number;
  currentStock: number;
  minStock: number;
}

export interface Category {
  id: number;
  name: string;
  description?: string;
}

export interface Supplier {
  id: number;
  name: string;
  contactName: string;
  email: string;
  phone: string;
}

export interface PagedResponse<T> {
  items: T[];
  totalItems: number;
  page: number;
  pageSize: number;
}

export interface ProductQuery {
  search?: string;
  page?: number;
  pageSize?: number;
}

@Injectable({
  providedIn: 'root'
})
export class ProductsService {
  private apiUrl = '/api/products';

  constructor(private http: HttpClient) {}

  getAll(query: ProductQuery = {}): Observable<PagedResponse<Product>> {
    let params = new HttpParams();
    
    if (query.search) {
      params = params.set('search', query.search);
    }
    if (query.page) {
      params = params.set('page', query.page.toString());
    }
    if (query.pageSize) {
      params = params.set('pageSize', query.pageSize.toString());
    }

    return this.http.get<PagedResponse<Product>>(this.apiUrl, { params });
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  create(product: Partial<Product>): Observable<Product> {
    return this.http.post<Product>(this.apiUrl, product);
  }

  update(id: number, product: Partial<Product>): Observable<Product> {
    return this.http.put<Product>(`${this.apiUrl}/${id}`, product);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}