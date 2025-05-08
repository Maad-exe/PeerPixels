export interface Post {
    id: number;
    userId: string;
    userName: string;
    displayName: string;
    userAvatarUrl: string;
    imageUrl: string;
    caption: string;
    createdAt: string;
  }
  
  export interface CreatePost {
    imageUrl: string;
    caption: string;
  }