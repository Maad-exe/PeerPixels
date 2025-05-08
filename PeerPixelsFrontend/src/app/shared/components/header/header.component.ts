import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';
import { User } from '../../../core/models/user.model';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule, NgbDropdownModule],
  template: `
    <nav class="navbar navbar-expand-lg navbar-light bg-light">
      <div class="container">
        <a class="navbar-brand" routerLink="/">PeerPixels</a>
        
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
          <span class="navbar-toggler-icon"></span>
        </button>
        
        <div class="collapse navbar-collapse" id="navbarNav">
          <ul class="navbar-nav ms-auto">
            <ng-container *ngIf="user; else loggedOut">
              <li class="nav-item">
                <a class="nav-link" routerLink="/feed">Feed</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" routerLink="/post/create">Create Post</a>
              </li>
              <li class="nav-item dropdown" ngbDropdown>
                <a class="nav-link dropdown-toggle" id="navbarDropdown" role="button" ngbDropdownToggle>
                  <img [src]="user.avatarUrl" class="rounded-circle" width="25" height="25" alt="Avatar">
                  {{ user.displayName }}
                </a>
                <div class="dropdown-menu" ngbDropdownMenu>
                  <a class="dropdown-item" [routerLink]="['/profile', user.userName]">Profile</a>
                  <div class="dropdown-divider"></div>
                  <a class="dropdown-item" (click)="logout()">Logout</a>
                </div>
              </li>
            </ng-container>
            
            <ng-template #loggedOut>
              <li class="nav-item">
                <a class="nav-link" routerLink="/login">Login</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" routerLink="/register">Register</a>
              </li>
            </ng-template>
          </ul>
        </div>
      </div>
    </nav>
  `
})
export class HeaderComponent {
  user: User | null = null;

  constructor(private authService: AuthService, private router: Router) {
    this.authService.currentUser$.subscribe(user => {
      this.user = user;
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}