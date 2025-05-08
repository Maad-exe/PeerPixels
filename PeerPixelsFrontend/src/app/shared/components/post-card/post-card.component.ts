import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Post } from '../../../core/models/post.model';

@Component({
  selector: 'app-post-card',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="card">
      <div class="card-header d-flex align-items-center">
        <img [src]="post.userAvatarUrl" class="rounded-circle me-2" style="width: 32px; height: 32px; object-fit: cover;" alt="User Avatar">
        <a [routerLink]="['/profile', post.userName]" class="text-decoration-none">
          <strong>{{ post.displayName }}</strong>
          <small class="text-muted">{{ '@' + post.userName }}</small>
        </a>
      </div>
      <img [src]="post.imageUrl" class="card-img-top" alt="Post image">
      <div class="card-body">
        <p class="card-text">{{ post.caption }}</p>
        <small class="text-muted">{{ formatDate(post.createdAt) }}</small>
      </div>
    </div>
  `
})
export class PostCardComponent {
  @Input() post!: Post;

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleString();
  }
}