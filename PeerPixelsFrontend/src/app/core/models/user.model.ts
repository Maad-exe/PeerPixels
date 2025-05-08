export interface User {
    id: string;
    userName: string;
    displayName: string;
    email: string;
    avatarUrl: string;
    followersCount: number;
    followingCount: number;
    postsCount: number;
    isFollowing: boolean;
  }
  
  export interface UpdateUser {
    displayName?: string;
    avatarUrl?: string;
  }