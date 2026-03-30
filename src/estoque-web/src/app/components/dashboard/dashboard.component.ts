import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartType } from 'chart.js';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  template: `
    <div class="container-fluid py-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 class="mb-1">Dashboard Administrativo</h3>
          <small class="text-secondary">Visão completa do sistema em tempo real</small>
        </div>
        <div class="d-flex align-items-center gap-2">
          <span class="badge text-bg-success">Sistema Online</span>
          <button class="btn btn-outline-primary btn-sm" (click)="refreshData()">
            <i class="bi bi-arrow-clockwise"></i> Atualizar
          </button>
        </div>
      </div>

      <!-- KPIs Principais -->
      <div class="row g-3 mb-4">
        <div class="col-md-3">
          <div class="card kpi-card">
            <div class="card-body">
              <div class="d-flex justify-content-between">
                <div>
                  <h6 class="text-muted mb-1">Total de Produtos</h6>
                  <h3 class="mb-0 text-primary">{{dashboard?.totalProducts || 0}}</h3>
                </div>
                <div class="text-primary">
                  <i class="bi bi-box-seam fs-1"></i>
                </div>
              </div>
              <small class="text-success">
                <i class="bi bi-arrow-up"></i> +{{dashboard?.newProductsThisMonth || 0}} este mês
              </small>
            </div>
          </div>
        </div>
        
        <div class="col-md-3">
          <div class="card kpi-card">
            <div class="card-body">
              <div class="d-flex justify-content-between">
                <div>
                  <h6 class="text-muted mb-1">Categorias</h6>
                  <h3 class="mb-0 text-info">{{dashboard?.totalCategories || 0}}</h3>
                </div>
                <div class="text-info">
                  <i class="bi bi-tags fs-1"></i>
                </div>
              </div>
              <small class="text-muted">
                <i class="bi bi-check-circle"></i> {{dashboard?.activeCategories || 0}} ativas
              </small>
            </div>
          </div>
        </div>
        
        <div class="col-md-3">
          <div class="card kpi-card">
            <div class="card-body">
              <div class="d-flex justify-content-between">
                <div>
                  <h6 class="text-muted mb-1">Estoque Baixo</h6>
                  <h3 class="mb-0 text-warning">{{dashboard?.lowStockProducts || 0}}</h3>
                </div>
                <div class="text-warning">
                  <i class="bi bi-exclamation-triangle fs-1"></i>
                </div>
              </div>
              <small class="text-warning">
                <i class="bi bi-bell"></i> Requer atenção
              </small>
            </div>
          </div>
        </div>
        
        <div class="col-md-3">
          <div class="card kpi-card">
            <div class="card-body">
              <div class="d-flex justify-content-between">
                <div>
                  <h6 class="text-muted mb-1">Movimentações</h6>
                  <h3 class="mb-0 text-success">{{dashboard?.transactionsLast30Days || 0}}</h3>
                </div>
                <div class="text-success">
                  <i class="bi bi-arrow-left-right fs-1"></i>
                </div>
              </div>
              <small class="text-success">
                <i class="bi bi-graph-up"></i> Últimos 30 dias
              </small>
            </div>
          </div>
        </div>
      </div>

      <!-- Gráficos -->
      <div class="row g-3 mb-4">
        <div class="col-lg-8">
          <div class="card">
            <div class="card-header">
              <h6 class="mb-0">Fluxo de Estoque - Últimos 30 Dias</h6>
            </div>
            <div class="card-body">
              <canvas baseChart 
                [data]="stockFlowChartData" 
                [options]="stockFlowChartOptions"
                [type]="'line'">
              </canvas>
            </div>
          </div>
        </div>
        
        <div class="col-lg-4">
          <div class="card">
            <div class="card-header">
              <h6 class="mb-0">Distribuição por Categoria</h6>
            </div>
            <div class="card-body">
              <canvas baseChart 
                [data]="categoryChartData" 
                [options]="categoryChartOptions"
                [type]="'doughnut'">
              </canvas>
            </div>
          </div>
        </div>
      </div>

      <!-- Alertas e Ações Rápidas -->
      <div class="row g-3">
        <div class="col-lg-6">
          <div class="card">
            <div class="card-header">
              <h6 class="mb-0 text-warning">
                <i class="bi bi-exclamation-triangle"></i> Alertas de Estoque
              </h6>
            </div>
            <div class="card-body">
              <div class="list-group list-group-flush">
                <div class="list-group-item d-flex justify-content-between align-items-center" 
                     *ngFor="let product of lowStockProducts">
                  <div>
                    <strong>{{product.name}}</strong>
                    <br>
                    <small class="text-muted">SKU: {{product.sku}}</small>
                  </div>
                  <div class="text-end">
                    <span class="badge text-bg-warning">{{product.currentStock}}</span>
                    <br>
                    <small class="text-muted">mín: {{product.minStock}}</small>
                  </div>
                </div>
                <div class="list-group-item text-center text-muted" *ngIf="lowStockProducts.length === 0">
                  Nenhum produto com estoque baixo
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <div class="col-lg-6">
          <div class="card">
            <div class="card-header">
              <h6 class="mb-0">
                <i class="bi bi-lightning"></i> Ações Rápidas
              </h6>
            </div>
            <div class="card-body">
              <div class="row g-2">
                <div class="col-6">
                  <button class="btn btn-outline-primary w-100" (click)="navigateTo('/products')">
                    <i class="bi bi-plus-circle"></i><br>
                    Novo Produto
                  </button>
                </div>
                <div class="col-6">
                  <button class="btn btn-outline-success w-100" (click)="navigateTo('/stock-movements')">
                    <i class="bi bi-arrow-left-right"></i><br>
                    Movimentação
                  </button>
                </div>
                <div class="col-6">
                  <button class="btn btn-outline-info w-100" (click)="navigateTo('/reports')">
                    <i class="bi bi-graph-up"></i><br>
                    Relatórios
                  </button>
                </div>
                <div class="col-6">
                  <button class="btn btn-outline-warning w-100" (click)="navigateTo('/categories')">
                    <i class="bi bi-tags"></i><br>
                    Categorias
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Atividade Recente -->
      <div class="row g-3 mt-3">
        <div class="col-12">
          <div class="card">
            <div class="card-header">
              <h6 class="mb-0">
                <i class="bi bi-clock-history"></i> Atividade Recente
              </h6>
            </div>
            <div class="card-body">
              <div class="table-responsive">
                <table class="table table-sm">
                  <thead>
                    <tr>
                      <th>Data/Hora</th>
                      <th>Produto</th>
                      <th>Tipo</th>
                      <th>Quantidade</th>
                      <th>Usuário</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr *ngFor="let activity of recentActivities">
                      <td>{{activity.createdAtUtc | date:'dd/MM/yyyy HH:mm'}}</td>
                      <td>{{activity.product?.name}}</td>
                      <td>
                        <span class="badge" [ngClass]="{
                          'text-bg-success': activity.type === 'Entrada',
                          'text-bg-danger': activity.type === 'Saída',
                          'text-bg-warning': activity.type === 'Ajuste'
                        }">
                          {{activity.type}}
                        </span>
                      </td>
                      <td>{{activity.quantity}}</td>
                      <td>{{activity.user?.name}}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .kpi-card {
      border: 0;
      border-radius: 12px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.1);
      transition: transform 0.2s;
    }
    .kpi-card:hover {
      transform: translateY(-2px);
    }
    .card {
      border: 0;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }
    .card-header {
      background: transparent;
      border-bottom: 1px solid #eee;
    }
    .list-group-item {
      border: 0;
      border-bottom: 1px solid #eee;
    }
    .list-group-item:last-child {
      border-bottom: 0;
    }
  `]
})
export class DashboardComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly apiBase = 'http://localhost:5177/api';
  
  dashboard: any = null;
  lowStockProducts: any[] = [];
  recentActivities: any[] = [];
  
  // Stock Flow Chart
  stockFlowChartData: ChartConfiguration['data'] = {
    labels: [],
    datasets: [
      {
        label: 'Entradas',
        data: [],
        borderColor: '#198754',
        backgroundColor: 'rgba(25, 135, 84, 0.1)',
        tension: 0.4,
        fill: true
      },
      {
        label: 'Saídas',
        data: [],
        borderColor: '#dc3545',
        backgroundColor: 'rgba(220, 53, 69, 0.1)',
        tension: 0.4,
        fill: true
      }
    ]
  };
  
  stockFlowChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'top',
      }
    },
    scales: {
      y: {
        beginAtZero: true
      }
    }
  };
  
  // Category Chart
  categoryChartData: ChartConfiguration['data'] = {
    labels: [],
    datasets: [{
      data: [],
      backgroundColor: [
        '#0d6efd',
        '#198754',
        '#ffc107',
        '#dc3545',
        '#0dcaf0',
        '#6f42c1'
      ]
    }]
  };
  
  categoryChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'bottom',
      }
    }
  };

  ngOnInit(): void {
    this.loadDashboard();
    this.loadLowStockProducts();
    this.loadRecentActivities();
  }

  private loadDashboard(): void {
    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    
    this.http.get<any>(`${this.apiBase}/reports/dashboard`, { headers }).subscribe({
      next: (res) => {
        this.dashboard = res;
        this.updateCharts(res);
      },
      error: (err) => console.error('Erro ao carregar dashboard:', err)
    });
  }

  private loadLowStockProducts(): void {
    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    
    this.http.get<any>(`${this.apiBase}/products?page=1&pageSize=100`, { headers }).subscribe({
      next: (res) => {
        this.lowStockProducts = (res.items || []).filter((p: any) => p.currentStock <= p.minStock).slice(0, 5);
      },
      error: (err) => console.error('Erro ao carregar produtos:', err)
    });
  }

  private loadRecentActivities(): void {
    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    
    this.http.get<any[]>(`${this.apiBase}/stock-transactions`, { headers }).subscribe({
      next: (res) => {
        this.recentActivities = res.slice(0, 10);
      },
      error: (err) => console.error('Erro ao carregar atividades:', err)
    });
  }

  private updateCharts(data: any): void {
    // Update stock flow chart
    if (data.dailyFlow) {
      this.stockFlowChartData.labels = data.dailyFlow.map((x: any) => x.day);
      this.stockFlowChartData.datasets[0].data = data.dailyFlow.map((x: any) => x.entradas);
      this.stockFlowChartData.datasets[1].data = data.dailyFlow.map((x: any) => x.saidas);
    }
    
    // Update category chart
    if (data.categoryDistribution) {
      this.categoryChartData.labels = data.categoryDistribution.map((x: any) => x.name);
      this.categoryChartData.datasets[0].data = data.categoryDistribution.map((x: any) => x.count);
    }
  }

  refreshData(): void {
    this.loadDashboard();
    this.loadLowStockProducts();
    this.loadRecentActivities();
  }

  navigateTo(route: string): void {
    window.location.href = route;
  }
}