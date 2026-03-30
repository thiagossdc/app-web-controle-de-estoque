import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="container-fluid py-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 class="mb-1">Relatórios</h3>
          <small class="text-secondary">Análises e exportação de dados</small>
        </div>
        <div class="flex space-x-2">
          <button class="btn btn-primary" (click)="generateReport('inventory')">Exportar Inventário</button>
          <button class="btn btn-success" (click)="generateReport('movements')">Exportar Movimentações</button>
        </div>
      </div>

      <div class="row g-3">
        <div class="col-md-4">
          <div class="card p-3">
            <h6>Total de Produtos</h6>
            <h4 class="text-primary">{{summary.totalProducts}}</h4>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card p-3">
            <h6>Produtos em Estoque Baixo</h6>
            <h4 class="text-danger">{{summary.lowStockProducts}}</h4>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card p-3">
            <h6>Valor Total em Estoque</h6>
            <h4 class="text-success">R$ {{summary.totalValue | number:'1.2-2'}}</h4>
          </div>
        </div>
        <div class="col-12">
          <div class="card p-3">
            <h6>Movimentações Recentes</h6>
            <table class="table table-sm">
              <thead>
                <tr>
                  <th>Data</th>
                  <th>Produto</th>
                  <th>Tipo</th>
                  <th>Quantidade</th>
                  <th>Saldo</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let m of recentMovements">
                  <td>{{m.createdAtUtc | date:'short'}}</td>
                  <td>{{m.product?.name}}</td>
                  <td>{{m.type}}</td>
                  <td>{{m.quantity}}</td>
                  <td>{{m.product?.currentStock}}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .card {
      border: 0;
      border-radius: 12px;
      box-shadow: 0 6px 24px rgba(15, 23, 42, 0.08);
    }
  `]
})
export class ReportsComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly apiBase = 'http://localhost:5177/api';
  
  summary = {
    totalProducts: 0,
    lowStockProducts: 0,
    totalValue: 0
  };
  recentMovements: any[] = [];

  ngOnInit(): void {
    this.loadReports();
  }

  private loadReports(): void {
    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    
    this.http.get<any>(`${this.apiBase}/products?page=1&pageSize=100`, { headers }).subscribe({
      next: (res) => {
        const products = res.items ?? [];
        this.summary.totalProducts = products.length;
        this.summary.lowStockProducts = products.filter((p: any) => p.currentStock <= p.minStock).length;
        this.summary.totalValue = products.reduce((total: number, product: any) => {
          return total + (product.currentStock * product.unitPrice);
        }, 0);
      },
      error: (err) => console.error('Erro ao carregar produtos:', err)
    });

    this.http.get<any[]>(`${this.apiBase}/stock-transactions`, { headers }).subscribe({
      next: (res) => {
        this.recentMovements = res.slice(0, 10);
      },
      error: (err) => console.error('Erro ao carregar movimentações:', err)
    });
  }

  generateReport(type: string): void {
    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    
    this.http.get(`${this.apiBase}/reports/${type}`, { headers, responseType: 'blob' }).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `${type}-report-${new Date().toISOString().slice(0, 10)}.pdf`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
      },
      error: (err) => console.error('Erro ao gerar relatório:', err)
    });
  }
}