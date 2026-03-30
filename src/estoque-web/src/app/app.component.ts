import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BaseChartDirective } from 'ng2-charts';
import { Router } from '@angular/router';

type Tab = 'dashboard' | 'produtos' | 'categorias' | 'relatorios' | 'configuracoes';

@Component({
  selector: 'app-root',
  imports: [CommonModule, FormsModule, BaseChartDirective],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly apiBase = '/api';
  activeTab: Tab = 'dashboard';
  token = '';
  user: any = null;
  dashboard: any = null;
  products: any[] = [];
  categories: any[] = [];
  suppliers: any[] = [];
  transactions: any[] = [];
  notifications: string[] = [];

  productForm = { name: '', sku: '', categoryId: 1, supplierId: 1, unitPrice: 0, minStock: 0 };
  categoryForm = { name: '', description: '' };
  transactionForm = { productId: 1, type: 1, quantity: 1, reason: '' };

  chartData = {
    labels: [] as string[],
    datasets: [
      { data: [] as number[], label: 'Entradas', backgroundColor: '#2f6fed' },
      { data: [] as number[], label: 'Saidas', backgroundColor: '#334155' }
    ]
  };

  ngOnInit(): void {
    this.checkAuth();
  }

  checkAuth(): void {
    const token = localStorage.getItem('token');
    const user = localStorage.getItem('user');
    
    if (!token || !user) {
      this.router.navigate(['/login']);
      return;
    }
    
    this.token = token;
    this.user = JSON.parse(user);
    this.loadAll();
    this.startNotificationStream();
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.router.navigate(['/login']);
  }

  setTab(tab: Tab): void {
    this.activeTab = tab;
  }

  loadAll(): void {
    this.request<any>('reports/dashboard').subscribe((res) => {
      this.dashboard = res;
      this.chartData.labels = res.dailyFlow.map((x: any) => x.day);
      this.chartData.datasets[0].data = res.dailyFlow.map((x: any) => x.entradas);
      this.chartData.datasets[1].data = res.dailyFlow.map((x: any) => x.saidas);
    });
    this.request<any>('products?page=1&pageSize=100').subscribe((res) => {
      this.products = res.items ?? [];
      if (this.products.length > 0) {
        this.transactionForm.productId = this.products[0].id;
      }
    });
    this.request<any[]>('categories').subscribe((res) => {
      this.categories = res;
      if (res.length > 0) {
        this.productForm.categoryId = res[0].id;
      }
    });
    this.request<any[]>('suppliers').subscribe((res) => {
      this.suppliers = res;
      if (res.length > 0) {
        this.productForm.supplierId = res[0].id;
      }
    });
    this.request<any[]>('stock-transactions').subscribe((res) => (this.transactions = res));
  }

  createCategory(): void {
    this.request('categories', 'POST', this.categoryForm).subscribe(() => {
      this.categoryForm = { name: '', description: '' };
      this.loadAll();
    });
  }

  createProduct(): void {
    this.request('products', 'POST', this.productForm).subscribe(() => {
      this.productForm = { name: '', sku: '', categoryId: 1, supplierId: 1, unitPrice: 0, minStock: 0 };
      this.loadAll();
    });
  }

  registerTransaction(): void {
    this.request('stock-transactions', 'POST', this.transactionForm).subscribe(() => {
      this.transactionForm = { productId: this.transactionForm.productId, type: 1, quantity: 1, reason: '' };
      this.loadAll();
    });
  }

  private startNotificationStream(): void {
    const source = new EventSource(`${this.apiBase}/notifications/stream`, { withCredentials: false });
    source.addEventListener('stock', (event: any) => {
      this.notifications.unshift(`${new Date().toLocaleTimeString()} - Evento de estoque recebido`);
      this.notifications = this.notifications.slice(0, 5);
      this.loadAll();
    });
  }

  private request<T>(route: string, method: 'GET' | 'POST' = 'GET', body?: unknown) {
    const headers = new HttpHeaders({ Authorization: `Bearer ${this.token}` });
    if (method === 'GET') {
      return this.http.get<T>(`${this.apiBase}/${route}`, { headers });
    }

    return this.http.post<T>(`${this.apiBase}/${route}`, body, { headers });
  }
}
