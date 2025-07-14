// User Types
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: 'volunteer' | 'organization';
}

// Mission Types
export interface Mission {
  id: string;
  title: string;
  description: string;
  location: string;
  date: string;
  time: string;
  createdBy: string;
  createdByName: string;
  participants: string[];
  maxParticipants?: number;
}

// Auth Types
export interface RegisterData {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  role: 'volunteer' | 'organization';
}

export interface CreateMissionData {
  title: string;
  description: string;
  location: string;
  date: string;
  time: string;
  maxParticipants?: number;
}

// Onboarding Types
export interface OnboardingData {
  id: number;
  title: string;
  subtitle: string;
  description: string;
}

// Component Props Types
export interface VolontarLogoProps {
  size?: number;
  showText?: boolean;
  textColor?: string;
}

// Navigation Types
export type RootStackParamList = {
  index: undefined;
  'onboarding/index': undefined;
  'auth/welcome': undefined;
  'auth/login': { role?: string };
  '(tabs)': undefined;
};
