import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container-fluid py-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 class="mb-1">Produtos</h3>
          <small class="text-secondary">Gerenciamento de produtos do estoque</small>
        </div>
        <button class="btn btn-dark" (click)="showCreateModal = true">Novo Produto</button>
      </div>

      <div class="row g-3">
        <div class="col-lg-4">
          <div class="card p-3">
            <h6>Novo produto</h6>
            <input class="form-control mb-2" [(ngModel)]="productForm.name" placeholder="Nome">
            <input class="form-control mb-2" [(ngModel)]="productForm.sku" placeholder="SKU">
            <select class="form-select mb-2" [(ngModel)]="productForm.categoryId">
              <option *ngFor="let c of categories" [ngValue]="c.id">{{c.name}}</option>
            </select>
            <select class="form-select mb-2" [(ngModel)]="productForm.supplierId">
              <option *ngFor="let s of suppliers" [ngValue]="s.id">{{s.name}}</option>
            </select>
            <input class="form-control mb-2" type="number" [(ngModel)]="productForm.unitPrice" placeholder="Preço unitário">
            <input class="form-control mb-2" type="number" [(ngModel)]="productForm.minStock" placeholder="Estoque mínimo">
            <button class="btn btn-dark" (click)="createProduct()">Salvar</button>
          </div>
        </div>
        <div class="col-lg-8">
          <div class="card p-3">
            <h6>Lista de produtos</h6>
            <table class="table table-sm">
              <thead>
                <tr>
                  <th>Nome</th>
                  <th>SKU</th>
                  <th>Categoria</th>
                  <th>Estoque</th>
                  <th>Preço</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let p of products">
                  <td>{{p.name}}</td>
                  <td>{{p.sku}}</td>
                  <td>{{p.category?.name}}</td>
                  <td>{{p.currentStock}}</td>
                  <td>R$ {{p.unitPrice}}</td>
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
export class ProductsComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly apiBase = 'http://localhost:5177/api';
  
  products: any[] = [];
  categories: any[] = [];
  suppliers: any[] = [];
  showCreateModal = false;
  
  productForm = {
    name: '',
    sku: '',
    categoryId: 1,
    supplierId: 1,
    unitPrice: 0,
    minStock: 0
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
          this.productForm.categoryId = this.products[0].categoryId;
          this.productForm.supplierId = this.products[0].supplierId;
        }
      },
      error: (err) => console.error('Erro ao carregar produtos:', err)
    });

    this.http.get<any[]>(`${this.apiBase}/categories`, { headers }).subscribe({
      next: (res) => {
        this.categories = res;
        if (res.length > 0) {
          this.productForm.categoryId = res[0].id;
        }
      },
      error: (err) => console.error('Erro ao carregar categorias:', err)
    });

    this.http.get<any[]>(`${this.apiBase}/suppliers`, { headers }).subscribe({
      next: (res) => {
        this.suppliers = res;
        if (res.length > 0) {
          this.productForm.supplierId = res[0].id;
        }
      },
      error: (err) => console.error('Erro ao carregar fornecedores:', err)
    });
  }

  createProduct(): void {
    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    
    this.http.post(`${this.apiBase}/products`, this.productForm, { headers }).subscribe({
      next: () => {
        this.productForm = { name: '', sku: '', categoryId: 1, supplierId: 1, unitPrice: 0, minStock: 0 };
        this.loadAll();
      },
      error: (err) => console.error('Erro ao criar produto:', err)
    });
  }
}