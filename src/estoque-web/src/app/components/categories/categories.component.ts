import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container-fluid py-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 class="mb-1">Categorias</h3>
          <small class="text-secondary">Gerenciamento de categorias de produtos</small>
        </div>
        <button class="btn btn-dark" (click)="showCreateModal = true">Nova Categoria</button>
      </div>

      <div class="row g-3">
        <div class="col-lg-4">
          <div class="card p-3">
            <h6>Nova categoria</h6>
            <input class="form-control mb-2" [(ngModel)]="categoryForm.name" placeholder="Nome">
            <textarea class="form-control mb-2" [(ngModel)]="categoryForm.description" placeholder="Descrição"></textarea>
            <button class="btn btn-dark" (click)="createCategory()">Salvar</button>
          </div>
        </div>
        <div class="col-lg-8">
          <div class="card p-3">
            <h6>Categorias cadastradas</h6>
            <ul class="list-group">
              <li class="list-group-item" *ngFor="let c of categories">
                <strong>{{c.name}}</strong> - {{c.description}}
              </li>
            </ul>
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
export class CategoriesComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly apiBase = 'http://localhost:5177/api';
  
  categories: any[] = [];
  showCreateModal = false;
  
  categoryForm = {
    name: '',
    description: ''
  };

  ngOnInit(): void {
    this.loadCategories();
  }

  private loadCategories(): void {
    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    
    this.http.get<any[]>(`${this.apiBase}/categories`, { headers }).subscribe({
      next: (res) => {
        this.categories = res;
      },
      error: (err) => console.error('Erro ao carregar categorias:', err)
    });
  }

  createCategory(): void {
    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({ Authorization: `Bearer ${token}` });
    
    this.http.post(`${this.apiBase}/categories`, this.categoryForm, { headers }).subscribe({
      next: () => {
        this.categoryForm = { name: '', description: '' };
        this.loadCategories();
      },
      error: (err) => console.error('Erro ao criar categoria:', err)
    });
  }
}