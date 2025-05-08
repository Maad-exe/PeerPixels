
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="container mt-5">
      <div class="row">
        <div class="col-md-6 mx-auto">
          <div class="card">
            <div class="card-header">
              <h4>Register</h4>
            </div>
            <div class="card-body">
              <form [formGroup]="registerForm" (ngSubmit)="onSubmit()">
                <div class="mb-3">
                  <label for="userName" class="form-label">Username</label>
                  <input type="text" class="form-control" id="userName" formControlName="userName">
                  <div *ngIf="registerForm.get('userName')?.invalid && registerForm.get('userName')?.touched" class="text-danger">
                    Username is required
                  </div>
                </div>

                <div class="mb-3">
                  <label for="email" class="form-label">Email</label>
                  <input type="email" class="form-control" id="email" formControlName="email">
                  <div *ngIf="registerForm.get('email')?.invalid && registerForm.get('email')?.touched" class="text-danger">
                    Valid email is required
                  </div>
                </div>

                <div class="mb-3">
                  <label for="password" class="form-label">Password</label>
                  <input type="password" class="form-control" id="password" formControlName="password">
                  <div *ngIf="registerForm.get('password')?.invalid && registerForm.get('password')?.touched" class="text-danger">
                    Password must be at least 6 characters
                  </div>
                </div>

                <div class="mb-3">
                  <label for="displayName" class="form-label">Display Name</label>
                  <input type="text" class="form-control" id="displayName" formControlName="displayName">
                </div>

                <button type="submit" class="btn btn-primary" [disabled]="registerForm.invalid || loading">
                  <span *ngIf="loading" class="spinner-border spinner-border-sm me-2"></span>
                  Register
                </button>
                
                <div *ngIf="error" class="alert alert-danger mt-3">
                  {{ error }}
                </div>
              </form>
              <div class="mt-3">
                <p>Already have an account? <a routerLink="/login">Login</a></p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class RegisterComponent {
  registerForm: FormGroup;
  loading = false;
  error: string | null = null;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.formBuilder.group({
      userName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      displayName: ['']
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      return;
    }
    
    this.loading = true;
    this.error = null;
    
    this.authService.register(this.registerForm.value).subscribe({
      next: () => {
        this.router.navigate(['/feed']);
      },
      error: (error) => {
        this.error = error?.error?.message || 'Registration failed. Please try again.';
        this.loading = false;
      }
    });
  }
}