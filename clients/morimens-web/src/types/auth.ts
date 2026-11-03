export interface AuthTokens {
  accessToken:  string;
  refreshToken: string;
  expiresIn:    number;
  userId:       string;
  username:     string;
  email:        string;
}

export interface UserProfile {
  userId:      string;
  username:    string;
  email:       string;
  displayName: string;
  avatarUrl?:  string;
  hasDiscord:  boolean;
  settings:    UserSettings;
}

export interface UserSettings {
  theme:                 'dark' | 'light';
  defaultAwakenerId?:    number;
  defaultCritMultiplier: number;
  defaultIsCritical:     boolean;
}

export interface SavedSimulation {
  id:        number;
  name:      string;
  createdAt: string;
  updatedAt: string;
  inputs?:   unknown;
  result?:   unknown;
}
