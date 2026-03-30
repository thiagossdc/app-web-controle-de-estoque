import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-stock-movements',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container-fluid py-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 class="mb-1">Movimentações de Estoque</h3>
          <small class="text-secondary">Registro de entradas, saídas e ajustes</small>
        </div>
        <button class="btn btn-dark" (click)="showCreateModal = true">Nova Movimentação</button>
      </div>

      <div class="row g-3">
        <div class="col-lg-4">
          <div class="card p-3">
            <h6>Registrar movimentação</h6>
            <select class="form-select mb-2" [(ngModel)]="transactionForm.productId">
              <option *ngFor="let p of products" [ngValue]="p.id">{{p.name}}</option>
            </select>
            <select class="form-select mb-2" [(ngModel)]="transactionForm.type">
              <option [ngValue]="1">Entrada</option>
              <option [ngValue]="2">Saída</option>
              <option [ngValue]="3">Ajuste</option>
            </select>
            <input class="form-control mb-2" type="number" [(ngModel)]="transactionForm.quantity" placeholder="Quantidade">
            <input class="form-control mb-2" [(ngModel)]="transactionForm.reason" placeholder="Motivo">
            <button class="btn btn-dark" (click)="registerTransaction()">Confirmar</button>
          </div>
        </div>
        <div class="col-lg-8">
          <div class="card p-3">
            <h6>Histórico de movimentações</h6>
            <table class="table table-sm">
              <thead>
                <tr>
                  <th>Produto</th>
                  <th>Tipo</th>
                  <th>Qtd</th>
                  <th>Data</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let t of transactions">
                  <td>{{t.product?.name}}</td>
                  <td>{{t.type}}</td>
                  <td>{{t.quantity}}</td>
                  <td>{{t.createdAtUtc | date:'short'}}</td>
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
export class StockMovementsComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly apiBase = 'http://localhost:5177/api';
  
  products: any[] = [];
  transactions: any[] = [];
  showCreateModal = false;
  
  transactionForm = {
    productId: 1,
    type: 1,
    quantity: 1,
    reason: ''
  };

  ngOnInit(): void {
    this.loadAll();
  }

  private loadAll(): void {
    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    
    this.http.get<any>(`${this.apiBase}/products?page=1&pageSize=100`, { headers }).subscribe({
      next: (res) => {
        this.products = res.items ?? [];
        if (this.products.length > 0) {
          this.transactionForm.productId = this.products[0].id;
        }
      },
      error: (err) => console.error('Erro ao carregar produtos:', err)
    });

    this.http.get<any[]>(`${this.apiBase}/stock-transactions`, { headers }).subscribe({
      next: (res) => {
        this.transactions = res;
      },
      error: (err) => console.error('Erro ao carregar movimentações:', err)
    });
  }

  registerTransaction(): void {
    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    
    this.http.post(`${this.apiBase}/stock-transactions`, this.transactionForm, { headers }).subscribe({
      next: () => {
        this.transactionForm = { productId: this.transactionForm.productId, type: 1, quantity: 1, reason: '' };
        this.loadAll();
      },
      error: (err) => console.error('Erro ao registrar movimentação:', err)
    });
  }
}