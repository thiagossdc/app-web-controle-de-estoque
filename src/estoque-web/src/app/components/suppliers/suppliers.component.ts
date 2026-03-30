import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-suppliers',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container-fluid py-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 class="mb-1">Fornecedores</h3>
          <small class="text-secondary">Gerenciamento de fornecedores</small>
        </div>
        <button class="btn btn-dark" (click)="showCreateModal = true">Novo Fornecedor</button>
      </div>

      <div class="row g-3">
        <div class="col-lg-4">
          <div class="card p-3">
            <h6>Novo fornecedor</h6>
            <input class="form-control mb-2" [(ngModel)]="supplierForm.name" placeholder="Nome">
            <input class="form-control mb-2" [(ngModel)]="supplierForm.contactName" placeholder="Nome do contato">
            <input class="form-control mb-2" [(ngModel)]="supplierForm.email" placeholder="Email">
            <input class="form-control mb-2" [(ngModel)]="supplierForm.phone" placeholder="Telefone">
            <button class="btn btn-dark" (click)="createSupplier()">Salvar</button>
          </div>
        </div>
        <div class="col-lg-8">
          <div class="card p-3">
            <h6>Fornecedores cadastrados</h6>
            <table class="table table-sm">
              <thead>
                <tr>
                  <th>Nome</th>
                  <th>Contato</th>
                  <th>Email</th>
                  <th>Telefone</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let s of suppliers">
                  <td>{{s.name}}</td>
                  <td>{{s.contactName}}</td>
                  <td>{{s.email}}</td>
                  <td>{{s.phone}}</td>
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
export class SuppliersComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly apiBase = 'http://localhost:5177/api';
  
  suppliers: any[] = [];
  showCreateModal = false;
  
  supplierForm = {
    name: '',
    contactName: '',
    email: '',
    phone: ''
  };

  ngOnInit(): void {
    this.loadSuppliers();
  }

  private loadSuppliers(): void {
    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    
    this.http.get<any[]>(`${this.apiBase}/suppliers`, { headers }).subscribe({
      next: (res) => {
        this.suppliers = res;
      },
      error: (err) => console.error('Erro ao carregar fornecedores:', err)
    });
  }

  createSupplier(): void {
    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    
    this.http.post(`${this.apiBase}/suppliers`, this.supplierForm, { headers }).subscribe({
      next: () => {
        this.supplierForm = { name: '', contactName: '', email: '', phone: '' };
        this.loadSuppliers();
      },
      error: (err) => console.error('Erro ao criar fornecedor:', err)
    });
  }
}