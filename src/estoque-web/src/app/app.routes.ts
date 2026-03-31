import { Routes } from '@angular/router';
import { inject } from '@angular/core';
import { Router } from '@angular/router';

// Função de guarda de autenticação
const authGuard = () => {
  const router = inject(Router);
  const token = localStorage.getItem('token');
  
  if (token) {
    return true;
  }
  
  router.navigate(['/login']);
  return false;
};

// Função de guarda de login (redireciona para dashboard se já logado)
const loginGuard = () => {
  const router = inject(Router);
  const token = localStorage.getItem('token');
  
  if (!token) {
    return true;
  }
  
  router.navigate(['/']);
  return false;
};

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./components/login/login.component').then(m => m.LoginComponent),
    title: 'Login - Sistema de Estoque',
    canActivate: [loginGuard]
  },
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full'
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./components/dashboard/dashboard.component').then(m => m.DashboardComponent),
    title: 'Dashboard - Sistema de Estoque',
    canActivate: [authGuard]
  },
  {
    path: 'produtos',
    loadComponent: () => import('./components/products/products.component').then(m => m.ProductsComponent),
    title: 'Produtos - Sistema de Estoque',
    canActivate: [authGuard]
  },
  {
    path: 'categorias',
    loadComponent: () => import('./components/categories/categories.component').then(m => m.CategoriesComponent),
    title: 'Categorias - Sistema de Estoque',
    canActivate: [authGuard]
  },
  {
    path: 'fornecedores',
    loadComponent: () => import('./components/suppliers/suppliers.component').then(m => m.SuppliersComponent),
    title: 'Fornecedores - Sistema de Estoque',
    canActivate: [authGuard]
  },
  {
    path: 'movimentacoes',
    loadComponent: () => import('./components/stock-movements/stock-movements.component').then(m => m.StockMovementsComponent),
    title: 'Movimentações - Sistema de Estoque',
    canActivate: [authGuard]
  },
  {
    path: 'relatorios',
    loadComponent: () => import('./components/reports/reports.component').then(m => m.ReportsComponent),
    title: 'Relatórios - Sistema de Estoque',
    canActivate: [authGuard]
  },
  {
    path: 'configuracoes',
    loadComponent: () => import('./components/settings/settings.component').then(m => m.SettingsComponent),
    title: 'Configurações - Sistema de Estoque',
    canActivate: [authGuard]
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];
