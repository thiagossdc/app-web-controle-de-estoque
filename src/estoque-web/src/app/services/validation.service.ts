import { Injectable } from '@angular/core';
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export interface ValidationResult {
  isValid: boolean;
  errors: string[];
}

@Injectable({
  providedIn: 'root'
})
export class ValidationService {

  // Validadores customizados
  static skuValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      
      const sku = control.value.toUpperCase();
      const skuPattern = /^[A-Z]{2,4}-\d{3,6}-[A-Z]{2,4}$/;
      
      if (!skuPattern.test(sku)) {
        return { 
          invalidSku: { 
            message: 'SKU deve seguir o formato: XX-000-XX (ex: NB-14-CP)' 
          } 
        };
      }
      
      return null;
    };
  }

  static priceValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      
      const price = parseFloat(control.value);
      
      if (isNaN(price) || price <= 0) {
        return { 
          invalidPrice: { 
            message: 'Preço deve ser um número positivo maior que zero' 
          } 
        };
      }
      
      if (price > 999999.99) {
        return { 
          invalidPrice: { 
            message: 'Preço não pode ser maior que R$ 999.999,99' 
          } 
        };
      }
      
      return null;
    };
  }

  static stockValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      
      const stock = parseInt(control.value);
      
      if (isNaN(stock) || stock < 0) {
        return { 
          invalidStock: { 
            message: 'Estoque deve ser um número inteiro não negativo' 
          } 
        };
      }
      
      if (stock > 999999) {
        return { 
          invalidStock: { 
            message: 'Estoque não pode ser maior que 999.999' 
          } 
        };
      }
      
      return null;
    };
  }

  static phoneValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      
      const phone = control.value.replace(/\D/g, '');
      
      if (phone.length < 10 || phone.length > 11) {
        return { 
          invalidPhone: { 
            message: 'Telefone deve ter 10 ou 11 dígitos' 
          } 
        };
      }
      
      if (phone.length === 11 && !phone.startsWith('1')) {
        return { 
          invalidPhone: { 
            message: 'Telefone com 11 dígitos deve começar com 1' 
          } 
        };
      }
      
      return null;
    };
  }

  static emailValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      
      const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
      
      if (!emailPattern.test(control.value)) {
        return { 
          invalidEmail: { 
            message: 'Email deve ter um formato válido' 
          } 
        };
      }
      
      return null;
    };
  }

  static passwordStrengthValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (!control.value) return null;
      
      const password = control.value;
      const errors: string[] = [];
      
      if (password.length < 8) {
        errors.push('Senha deve ter pelo menos 8 caracteres');
      }
      
      if (!/[A-Z]/.test(password)) {
        errors.push('Senha deve conter pelo menos uma letra maiúscula');
      }
      
      if (!/[a-z]/.test(password)) {
        errors.push('Senha deve conter pelo menos uma letra minúscula');
      }
      
      if (!/\d/.test(password)) {
        errors.push('Senha deve conter pelo menos um número');
      }
      
      if (!/[!@#$%^&*(),.?":{}|<>]/.test(password)) {
        errors.push('Senha deve conter pelo menos um caractere especial');
      }
      
      if (errors.length > 0) {
        return { 
          weakPassword: { 
            message: errors.join('. ') 
          } 
        };
      }
      
      return null;
    };
  }

  // Métodos de validação
  validateProduct(product: any): ValidationResult {
    const errors: string[] = [];
    
    if (!product.name || product.name.trim().length < 2) {
      errors.push('Nome do produto deve ter pelo menos 2 caracteres');
    }
    
    if (!product.sku || !this.isValidSku(product.sku)) {
      errors.push('SKU deve seguir o formato: XX-000-XX');
    }
    
    if (!product.categoryId || product.categoryId <= 0) {
      errors.push('Categoria é obrigatória');
    }
    
    if (!product.supplierId || product.supplierId <= 0) {
      errors.push('Fornecedor é obrigatório');
    }
    
    if (!product.unitPrice || product.unitPrice <= 0) {
      errors.push('Preço unitário deve ser maior que zero');
    }
    
    if (product.minStock < 0) {
      errors.push('Estoque mínimo não pode ser negativo');
    }
    
    return {
      isValid: errors.length === 0,
      errors
    };
  }

  validateCategory(category: any): ValidationResult {
    const errors: string[] = [];
    
    if (!category.name || category.name.trim().length < 2) {
      errors.push('Nome da categoria deve ter pelo menos 2 caracteres');
    }
    
    if (category.description && category.description.length > 500) {
      errors.push('Descrição não pode ter mais de 500 caracteres');
    }
    
    return {
      isValid: errors.length === 0,
      errors
    };
  }

  validateSupplier(supplier: any): ValidationResult {
    const errors: string[] = [];
    
    if (!supplier.name || supplier.name.trim().length < 2) {
      errors.push('Nome do fornecedor deve ter pelo menos 2 caracteres');
    }
    
    if (!supplier.contactName || supplier.contactName.trim().length < 2) {
      errors.push('Nome do contato deve ter pelo menos 2 caracteres');
    }
    
    if (!supplier.email || !this.isValidEmail(supplier.email)) {
      errors.push('Email deve ter um formato válido');
    }
    
    if (!supplier.phone || !this.isValidPhone(supplier.phone)) {
      errors.push('Telefone deve ter um formato válido');
    }
    
    return {
      isValid: errors.length === 0,
      errors
    };
  }

  validateStockTransaction(transaction: any): ValidationResult {
    const errors: string[] = [];
    
    if (!transaction.productId || transaction.productId <= 0) {
      errors.push('Produto é obrigatório');
    }
    
    if (!transaction.type || ![1, 2, 3].includes(transaction.type)) {
      errors.push('Tipo de movimentação inválido');
    }
    
    if (!transaction.quantity || transaction.quantity <= 0) {
      errors.push('Quantidade deve ser maior que zero');
    }
    
    if (transaction.quantity > 999999) {
      errors.push('Quantidade não pode ser maior que 999.999');
    }
    
    if (!transaction.reason || transaction.reason.trim().length < 3) {
      errors.push('Motivo deve ter pelo menos 3 caracteres');
    }
    
    return {
      isValid: errors.length === 0,
      errors
    };
  }

  validateUser(user: any): ValidationResult {
    const errors: string[] = [];
    
    if (!user.name || user.name.trim().length < 2) {
      errors.push('Nome deve ter pelo menos 2 caracteres');
    }
    
    if (!user.email || !this.isValidEmail(user.email)) {
      errors.push('Email deve ter um formato válido');
    }
    
    if (!user.password || user.password.length < 8) {
      errors.push('Senha deve ter pelo menos 8 caracteres');
    }
    
    if (user.password && !this.isStrongPassword(user.password)) {
      errors.push('Senha deve conter letras maiúsculas, minúsculas, números e caracteres especiais');
    }
    
    if (!user.role || ![1, 2].includes(user.role)) {
      errors.push('Perfil de usuário inválido');
    }
    
    return {
      isValid: errors.length === 0,
      errors
    };
  }

  // Métodos auxiliares
  private isValidSku(sku: string): boolean {
    const skuPattern = /^[A-Z]{2,4}-\d{3,6}-[A-Z]{2,4}$/;
    return skuPattern.test(sku.toUpperCase());
  }

  private isValidEmail(email: string): boolean {
    const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    return emailPattern.test(email);
  }

  private isValidPhone(phone: string): boolean {
    const cleanPhone = phone.replace(/\D/g, '');
    return cleanPhone.length >= 10 && cleanPhone.length <= 11;
  }

  private isStrongPassword(password: string): boolean {
    const hasUpperCase = /[A-Z]/.test(password);
    const hasLowerCase = /[a-z]/.test(password);
    const hasNumbers = /\d/.test(password);
    const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(password);
    
    return hasUpperCase && hasLowerCase && hasNumbers && hasSpecialChar;
  }

  // Formatação de dados
  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  formatPhone(phone: string): string {
    const cleanPhone = phone.replace(/\D/g, '');
    
    if (cleanPhone.length === 11) {
      return `(${cleanPhone.slice(0, 2)}) ${cleanPhone.slice(2, 7)}-${cleanPhone.slice(7)}`;
    } else if (cleanPhone.length === 10) {
      return `(${cleanPhone.slice(0, 2)}) ${cleanPhone.slice(2, 6)}-${cleanPhone.slice(6)}`;
    }
    
    return phone;
  }

  formatSku(sku: string): string {
    return sku.toUpperCase().replace(/[^A-Z0-9-]/g, '');
  }

  // Sanitização
  sanitizeInput(input: string): string {
    return input
      .trim()
      .replace(/[<>]/g, '') // Remove caracteres potencialmente perigosos
      .replace(/\s+/g, ' '); // Remove espaços extras
  }

  sanitizeHtml(html: string): string {
    const div = document.createElement('div');
    div.textContent = html;
    return div.innerHTML;
  }
}