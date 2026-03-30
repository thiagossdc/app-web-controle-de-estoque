import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="container-fluid py-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 class="mb-1">Configurações</h3>
          <small class="text-secondary">Configurações do sistema e notificações</small>
        </div>
      </div>

      <div class="row g-3">
        <div class="col-lg-6">
          <div class="card p-3">
            <h6>Usuário Logado</h6>
            <p class="mb-1"><strong>Email:</strong> {{userEmail}}</p>
            <p class="mb-1"><strong>Perfil:</strong> {{userRole}}</p>
            <small class="text-secondary">Use credenciais de admin para cadastros e funcionário para operações do dia a dia.</small>
          </div>
        </div>
        <div class="col-lg-6">
          <div class="card p-3">
            <h6>Notificações em Tempo Real</h6>
            <ul class="list-group">
              <li class="list-group-item" *ngFor="let n of notifications">{{n}}</li>
              <li class="list-group-item text-muted" *ngIf="notifications.length === 0">Nenhuma notificação no momento</li>
            </ul>
          </div>
        </div>
        <div class="col-12">
          <div class="card p-3">
            <h6>Configurações do Sistema</h6>
            <div class="row">
              <div class="col-md-6">
                <div class="mb-3">
                  <label class="form-label">URL da API</label>
                  <input type="text" class="form-control" [(ngModel)]="apiUrl" readonly>
                </div>
              </div>
              <div class="col-md-6">
                <div class="mb-3">
                  <label class="form-label">Versão do Sistema</label>
                  <input type="text" class="form-control" value="1.0.0" readonly>
                </div>
              </div>
            </div>
            <button class="btn btn-outline-primary" (click)="testConnection()">Testar Conexão</button>
            <span class="ms-2" [class.text-success]="connectionStatus === 'success'" [class.text-danger]="connectionStatus === 'error'">
              {{connectionMessage}}
            </span>
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
export class SettingsComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly apiBase = 'http://localhost:5177/api';
  
  userEmail = '';
  userRole = '';
  notifications: string[] = [];
  apiUrl = 'http://localhost:5177/api';
  connectionStatus = '';
  connectionMessage = '';

  ngOnInit(): void {
    this.loadUserInfo();
    this.startNotificationStream();
  }

  private loadUserInfo(): void {
    const token = localStorage.getItem('token');
    if (!token) return;

    // Decodificar token JWT para obter informações do usuário
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      this.userEmail = payload.email || 'admin@estoque.com';
      this.userRole = payload.role || 'Admin';
    } catch {
      this.userEmail = 'admin@estoque.com';
      this.userRole = 'Admin';
    }
  }

  private startNotificationStream(): void {
    const token = localStorage.getItem('token');
    if (!token) return;

    const source = new EventSource(`${this.apiBase}/notifications/stream`);
    source.addEventListener('stock', (event: any) => {
      this.notifications.unshift(`${new Date().toLocaleTimeString()} - Evento de estoque recebido`);
      this.notifications = this.notifications.slice(0, 5);
    });
  }

  testConnection(): void {
    this.http.get(`${this.apiBase}/health`).subscribe({
      next: () => {
        this.connectionStatus = 'success';
        this.connectionMessage = 'Conexão bem sucedida!';
      },
      error: () => {
        this.connectionStatus = 'error';
        this.connectionMessage = 'Falha na conexão';
      }
    });
  }
}