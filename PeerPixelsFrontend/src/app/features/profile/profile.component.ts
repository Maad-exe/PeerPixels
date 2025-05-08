import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { UserService } from '../../core/services/user.service';
import { PostService } from '../../core/services/post.service';
import { AuthService } from '../../core/auth/auth.service';
import { User } from '../../core/models/user.model';
import { Post } from '../../core/models/post.model';
import { PostCardComponent } from '../../shared/components/post-card/post-card.component';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, RouterModule, PostCardComponent],
  template: `
    <div class="container mt-4">
      <div *ngIf="loading" class="text-center">
        <div class="spinner-border" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
      </div>
      
      <ng-container *ngIf="!loading && user">
        <div class="row">
          <div class="col-md-4 text-center">
            <img [src]="user.avatarUrl" class="img-fluid rounded-circle" style="width: 150px; height: 150px; object-fit: cover;" alt="Profile picture">
            
            <div class="mt-3">
              <h3>{{ user.displayName }}</h3>
              <p class="text-muted">{{ '@' + user.userName }}</p>
            </div>
            
            <div class="mt-3">
              <div class="row">
                <div class="col">
                  <strong>{{ user.postsCount }}</strong><br>
                  <span class="text-muted">Posts</span>
                </div>
                <div class="col">
                  <strong>{{ user.followersCount }}</strong><br>
                  <span class="text-muted">Followers</span>
                </div>
                <div class="col">
                  <strong>{{ user.followingCount }}</strong><br>
                  <span class="text-muted">Following</span>
                </div>
              </div>
            </div>
            
            <div class="mt-3" *ngIf="isCurrentUser; else followButtons">
              <a routerLink="/profile/edit" class="btn btn-outline-primary">Edit Profile</a>
            </div>
            
            <ng-template #followButtons>
              <button *ngIf="!user.isFollowing" (click)="followUser()" class="btn btn-primary" [disabled]="followingInProgress">
                <span *ngIf="followingInProgress" class="spinner-border spinner-border-sm me-2"></span>
                Follow
              </button>
              <button *ngIf="user.isFollowing" (click)="unfollowUser()" class="btn btn-outline-primary" [disabled]="followingInProgress">
                <span *ngIf="followingInProgress" class="spinner-border spinner-border-sm me-2"></span>
                Unfollow
              </button>
            </ng-template>
          </div>
          
          <div class="col-md-8">
            <h4 class="mb-3">Posts</h4>
            
            <div *ngIf="posts.length === 0" class="alert alert-info">
              No posts yet.
            </div>
            
            <div *ngFor="let post of posts" class="mb-4">
              <app-post-card [post]="post"></app-post-card>
            </div>
          </div>
        </div>
      </ng-container>
      
      <div *ngIf="!loading && !user" class="alert alert-danger">
        User not found.
      </div>
    </div>
  `
})
export class ProfileComponent implements OnInit {
  user: User | null = null;
  posts: Post[] = [];
  loading: boolean = true;
  isCurrentUser: boolean = false;
  followingInProgress: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private userService: UserService,
    private postService: PostService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const username = params['username'];
      this.loadProfile(username);
    });
  }

  loadProfile(username: string): void {
    this.loading = true;
    
    this.userService.getUserByUsername(username).subscribe({
      next: (user) => {
        this.user = user;
        this.isCurrentUser = this.authService.currentUserValue?.id === user.id;
        this.loadPosts(user.id);
      },
      error: (error) => {
        console.error('Error loading profile', error);
        this.loading = false;
      }
    });
  }

  loadPosts(userId: string): void {
    this.postService.getUserPosts(userId).subscribe({
      next: (posts) => {
        this.posts = posts;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading posts', error);
        this.loading = false;
      }
    });
  }

  followUser(): void {
    if (!this.user) return;
    
    this.followingInProgress = true;
    this.userService.followUser(this.user.id).subscribe({
      next: () => {
        if (this.user) {
          this.user.isFollowing = true;
          this.user.followersCount++;
        }
        this.followingInProgress = false;
      },
      error: (error) => {
        console.error('Error following user', error);
        this.followingInProgress = false;
      }
    });
  }

  unfollowUser(): void {
    if (!this.user) return;
    
    this.followingInProgress = true;
    this.userService.unfollowUser(this.user.id).subscribe({
      next: () => {
        if (this.user) {
          this.user.isFollowing = false;
          this.user.followersCount--;
        }
        this.followingInProgress = false;
      },
      error: (error) => {
        console.error('Error unfollowing user', error);
        this.followingInProgress = false;
      }
    });
  }
}