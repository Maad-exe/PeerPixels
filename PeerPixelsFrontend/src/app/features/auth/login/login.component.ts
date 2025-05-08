import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { Login } from '../../../core/models/auth.model';

declare const google: any;

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  template: `
    <div class="container mt-5">
      <div class="row justify-content-center">
        <div class="col-md-6">
          <div class="card">
            <div class="card-header">Login to PeerPixels</div>
            <div class="card-body">
              <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
                <div class="mb-3">
                  <label for="email" class="form-label">Email</label>
                  <input type="email" formControlName="email" class="form-control" id="email" />
                  <div *ngIf="loginForm.get('email')?.errors && loginForm.get('email')?.touched" class="text-danger">
                    Email is required and must be valid
                  </div>
                </div>
                
                <div class="mb-3">
                  <label for="password" class="form-label">Password</label>
                  <input type="password" formControlName="password" class="form-control" id="password" />
                  <div *ngIf="loginForm.get('password')?.errors && loginForm.get('password')?.touched" class="text-danger">
                    Password is required
                  </div>
                </div>
                
                <div class="mb-3">
                  <button type="submit" [disabled]="loginForm.invalid" class="btn btn-primary w-100">Login</button>
                </div>
              </form>
              
              <hr />
              
              <div class="text-center">
                <p>Or login with:</p>
                <div id="g_id_onload"
                     data-client_id="YOUR_GOOGLE_CLIENT_ID"
                     data-callback="handleCredentialResponse">
                </div>
                <div class="g_id_signin" data-type="standard"></div>
              </div>
              
              <div class="mt-3 text-center">
                <p>Don't have an account? <a routerLink="/register">Register</a></p>
              </div>
              
              <div *ngIf="error" class="alert alert-danger mt-3">
                {{ error }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  returnUrl: string = '/';
  error: string = '';

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });

    // Define the global callback function for Google Sign-In
    (window as any).handleCredentialResponse = this.handleCredentialResponse.bind(this);
  }

  ngOnInit(): void {
    // Get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';

    // Load the Google Sign-In API script
    this.loadGoogleSignInAPI();
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    const loginData: Login = {
      email: this.loginForm.value.email,
      password: this.loginForm.value.password
    };

    this.authService.login(loginData).subscribe({
      next: () => {
        this.router.navigate([this.returnUrl]);
      },
      error: error => {
        this.error = error.error?.message || 'Login failed';
      }
    });
  }

  handleCredentialResponse(response: any): void {
    if (response.credential) {
      this.authService.googleLogin(response.credential).subscribe({
        next: () => {
          this.router.navigate([this.returnUrl]);
        },
        error: error => {
          this.error = error.error?.message || 'Google login failed';
        }
      });
    }
  }

  private loadGoogleSignInAPI(): void {
    const script = document.createElement('script');
    script.src = 'https://accounts.google.com/gsi/client';
    script.async = true;
    script.defer = true;
    document.head.appendChild(script);
  }
}